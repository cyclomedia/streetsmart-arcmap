/*
 * Integration in ArcMap for Cycloramas
 * Copyright (c) 2019, CycloMedia, All rights reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using StreetSmart.Common.Factories;
using StreetSmart.Common.Interfaces.Data;
using StreetSmart.Common.Interfaces.GeoJson;
using StreetSmart.Common.Interfaces.SLD;
using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace StreetSmartArcMap.Layers
{
    #region delegates

    // ===========================================================================
    // Delegates
    // ===========================================================================
    public delegate void VectorLayerAddDelegate(VectorLayer layer);

    public delegate void VectorLayerChangedDelegate(VectorLayer layer);

    public delegate void VectorLayerRemoveDelegate(VectorLayer layer);

    public delegate void FeatureStartEditDelegate(IList<ESRI.ArcGIS.Geometry.IGeometry> geometries);

    public delegate void FeatureUpdateEditDelegate(ESRI.ArcGIS.Geodatabase.IFeature feature);

    public delegate void FeatureDeleteDelegate(ESRI.ArcGIS.Geodatabase.IFeature feature);

    public delegate void StopEditDelegate();

    public delegate void StartMeasurementDelegate(TypeOfLayer typeOfLayer);

    public delegate void SketchCreateDelegate(IEditSketch3 sketch);

    public delegate void SketchModifiedDelegate(ESRI.ArcGIS.Geometry.IGeometry geometry);

    public delegate void SketchFinishedDelegate();

    #endregion delegates

    public class VectorLayer
    {
        #region constants

        // =========================================================================
        // Constants
        // =========================================================================
        private const string WfsHeader =
          "<wfs:FeatureCollection xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:wfs=\"http://www.opengis.net/wfs\" xmlns:gml=\"http://www.opengis.net/gml\">";

        private const string WfsFinished = "</wfs:FeatureCollection>";

        #endregion constants

        #region members

        // =========================================================================
        // Members
        // =========================================================================
        public static event VectorLayerAddDelegate LayerAddEvent;

        public static event VectorLayerRemoveDelegate LayerRemoveEvent;

        public static event VectorLayerChangedDelegate LayerChangedEvent;

        public static event FeatureStartEditDelegate FeatureStartEditEvent;

        public static event FeatureUpdateEditDelegate FeatureUpdateEditEvent;

        public static event FeatureDeleteDelegate FeatureDeleteEvent;

        public static event StopEditDelegate StopEditEvent;

        public static event StartMeasurementDelegate StartMeasurementEvent;

        public static event SketchCreateDelegate SketchCreateEvent;

        public static event SketchModifiedDelegate SketchModifiedEvent;

        public static event SketchFinishedDelegate SketchFinishedEvent;

        public IStyledLayerDescriptor Sld { get; private set; }

        private static IList<ESRI.ArcGIS.Geodatabase.IFeature> _editFeatures;
        private static IList<VectorLayer> _layers;
        private static Timer _editToolCheckTimer;
        private static ICommandItem _beforeTool;
        private static readonly LogClient LogClient;
        private static readonly object LockObject;
        private static bool _doSelection;
        private static Timer _nextSelectionTimer;

        private IFeatureClass _featureClass;
        private ILayer _layer;
        private bool _isVisibleInStreetSmart;

        public IOverlay Overlay;

        private static string CurrentMeasurementID = "";

        private Configuration.Configuration Config => Configuration.Configuration.Instance;
        #endregion members

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        static VectorLayer()
        {
            _editToolCheckTimer = null;
            _beforeTool = null;
            LogClient = new LogClient(typeof(VectorLayer));
            LockObject = new object();
            _doSelection = true;
        }

        #endregion constructor

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        private IFeatureCollection _contents;

        public bool ContentsChanged { get; private set; }
        public string Name => _layer != null ? _layer.Name : string.Empty;
        public bool IsVisible => _layer != null && _layer.Visible;

        public TypeOfLayer TypeOfLayer => GetTypeOfLayer(_featureClass.ShapeType);
        public ISpatialReference SpatialReference => GeometryDef.SpatialReference;
        public static IList<ESRI.ArcGIS.Geodatabase.IFeature> EditFeatures => _editFeatures ?? (_editFeatures = new List<ESRI.ArcGIS.Geodatabase.IFeature>());
        public static IList<VectorLayer> Layers => _layers ?? (_layers = DetectVectorLayers(false));
        public static Dictionary<VectorLayer, ESRI.ArcGIS.Geodatabase.IFeature> MeasurementsList = new Dictionary<VectorLayer, ESRI.ArcGIS.Geodatabase.IFeature>();

        public bool IsVisibleInStreetSmart
        {
            get
            {
                return (_isVisibleInStreetSmart && IsVisible);
            }
            set
            {
                if (IsVisibleInStreetSmart != value)
                {
                    _isVisibleInStreetSmart = value;

                    SetVisibility(value);
                }
            }
        }

        public IGeometryDef GeometryDef
        {
            get
            {
                IFields fields = _featureClass.Fields;
                int fieldId = fields.FindField(_featureClass.ShapeFieldName);
                IField field = fields.get_Field(fieldId);

                return field.GeometryDef;
            }
        }

        public bool HasZ
        {
            get
            {
                Configuration.Configuration config = Configuration.Configuration.Instance;
                SpatialReference spatRel = config.SpatialReference;
                ISpatialReference gsSpatialReference = spatRel?.SpatialRef ?? ArcUtils.SpatialReference;

                bool zCoord = gsSpatialReference.ZCoordinateUnit != null;
                bool sameFactoryCode = SpatialReference.FactoryCode == gsSpatialReference.FactoryCode;

                return (spatRel == null || spatRel.CanMeasuring) && GeometryDef.HasZ;
            }
        }

        #endregion properties

        #region functions (static)

        // =========================================================================
        // Functions (Static)
        // =========================================================================

        public static TypeOfLayer GetTypeOfLayer(esriGeometryType type)
        {
            switch (type)
            {
                case esriGeometryType.esriGeometryPoint:
                    return TypeOfLayer.Point;

                case esriGeometryType.esriGeometryLine:
                    return TypeOfLayer.Line;

                case esriGeometryType.esriGeometryPolyline:
                    return TypeOfLayer.Line;

                case esriGeometryType.esriGeometryPolygon:
                    return TypeOfLayer.Polygon;

                default:
                    return TypeOfLayer.None;
            }
        }

        public static VectorLayer GetLayer(ILayer layer)
        {
            if (layer == null)
                return null;

            return Layers.Aggregate<VectorLayer, VectorLayer>(null,
                                                              (current, layerCheck) =>
                                                              (layerCheck._layer?.Name == layer?.Name) ? layerCheck : current);
        }

        public static VectorLayer GetLayer(IFeatureClass featureClass)
        {
            return Layers.Aggregate<VectorLayer, VectorLayer>(null,
                                                              (current, layerCheck) =>
                                                              (layerCheck._featureClass.FeatureClassID == featureClass.FeatureClassID)
                                                                ? layerCheck
                                                                : current);
        }

        public static VectorLayer GetLayer(ESRI.ArcGIS.Geodatabase.IFeature feature)
        {
            var featureClass = feature?.Class as IFeatureClass;

            if (featureClass != null)
                return GetLayer(featureClass);
            else
                return null;
        }

        public static IList<VectorLayer> DetectVectorLayers(bool initEvents)
        {
            _layers = new List<VectorLayer>();
            IMap map = ArcUtils.Map;

            if (map != null)
            {
                var layers = map.get_Layers();
                ILayer layer;

                while ((layer = layers.Next()) != null)
                {
                    AvItemAdded(layer);
                }
            }

            var editor = ArcUtils.Editor;

            if (editor.EditState != esriEditState.esriStateNotEditing)
            {
                //OnSelectionChanged();
            }

            if (initEvents)
            {
                AddEvents();
                var docEvents = ArcUtils.MxDocumentEvents;

                if (docEvents != null)
                {
                    docEvents.OpenDocument += OpenDocument;
                    docEvents.CloseDocument += CloseDocument;
                }
            }

            return _layers;
        }

        private static void AddEvents()
        {
            var avEvents = ArcUtils.ActiveViewEvents;
            var editEvents = ArcUtils.EditorEvents;
            var editEvents5 = ArcUtils.EditorEvents5;

            if (avEvents != null)
            {
                avEvents.ItemAdded += AvItemAdded;
                avEvents.ItemDeleted += AvItemDeleted;
                avEvents.ContentsChanged += AvContentChanged;
                avEvents.ViewRefreshed += AvViewRefreshed;
                //TODO: STREET-2002
                avEvents.AfterDraw += AvEvents_AfterDraw;
            }

            if (editEvents != null)
            {
                editEvents.OnStartEditing += OnStartEditing;
                editEvents.OnChangeFeature += OnChangeFeature;
                editEvents.OnSelectionChanged += OnSelectionChanged;
                editEvents.OnStopEditing += OnStopEditing;
                editEvents.OnDeleteFeature += OnDeleteFeature;
                editEvents.OnSketchModified += OnSketchModified;
                editEvents.OnSketchFinished += OnSketchFinished;
            }
        }

        private static List<ESRI.ArcGIS.Geometry.IPoint> GetGeometryPoints(ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            var points = new List<ESRI.ArcGIS.Geometry.IPoint>();

            var type = VectorLayer.GetTypeOfLayer(geometry.GeometryType);

            switch (type)
            {
                case TypeOfLayer.Point:
                    var point = geometry as ESRI.ArcGIS.Geometry.IPoint;

                    if (point != null)
                    {
                        points.Add(point);
                    }

                    break;
                case TypeOfLayer.Line:
                    var polyline = geometry as Polyline;

                    if (polyline != null)
                    {
                        for (int i = 0; i < polyline.PointCount; i++)
                        {
                            points.Add(polyline.Point[i]);
                        }
                    }

                    break;
                case TypeOfLayer.Polygon:
                    var polygon = geometry as Polygon;

                    if (polygon != null)
                    {
                        for (int i = 0; i < polygon.PointCount; i++)
                        {
                            points.Add(polygon.Point[i]);
                        }
                    }

                    break;
                default:
                    break;
            }

            return points;
        }

        private static double GetLabelOffset(IDisplay display)
        {
            IDisplayTransformation dispTrans = display.DisplayTransformation;

            double distance = dispTrans.FromPoints(8);

            return (distance * 3) / 4;
        }

        private static ESRI.ArcGIS.Geometry.IPolygon GetLabelBox(IDisplay display, ESRI.ArcGIS.Geometry.IPoint point)
        {
            IDisplayTransformation dispTrans = display.DisplayTransformation;

            var polygon = new PolygonClass();

            //TODO: STREET-2002
            polygon.AddPoint(new PointClass { X = point.X - dispTrans.FromPoints(8), Y = point.Y - dispTrans.FromPoints(8) });
            polygon.AddPoint(new PointClass { X = point.X + dispTrans.FromPoints(8), Y = point.Y - dispTrans.FromPoints(8) });
            polygon.AddPoint(new PointClass { X = point.X + dispTrans.FromPoints(8), Y = point.Y + dispTrans.FromPoints(8) });
            polygon.AddPoint(new PointClass { X = point.X - dispTrans.FromPoints(8), Y = point.Y + dispTrans.FromPoints(8) });

            polygon.Close();

            return polygon;
        }

        private static void AvEvents_AfterDraw(IDisplay display, esriViewDrawPhase phase)
        {
            var sketch = ArcUtils.Editor as IEditSketch3;

            if (sketch != null && sketch.Geometry != null)
            {
                display.StartDrawing(display.hDC, (short)esriScreenCache.esriNoScreenCache);

                var fontDisp = new stdole.StdFontClass
                {
                    Bold = true,
                    Name = "Arial",
                    Italic = false,
                    Underline = false,
                    Size = 8
                };

                var offset = GetLabelOffset(display);

                RgbColor white = new RgbColorClass { Red = 255, Green = 255, Blue = 255 };
                RgbColor black = new RgbColorClass { Red = 0, Green = 0, Blue = 0 };
                ISymbol textSymbol = new TextSymbolClass { Font = fontDisp as stdole.IFontDisp, Color = black,  };
                display.SetSymbol(textSymbol);

                ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass { Color = white };
                ISymbol boxSymbol = simpleFillSymbol as ISymbol;
                boxSymbol.ROP2 = esriRasterOpCode.esriROPWhite;

                var points = GetGeometryPoints(sketch.Geometry);
                
                for (int i = 0; i < points.Count; i++)
                {
                    if (sketch.GeometryType == esriGeometryType.esriGeometryPolygon && i == points.Count - 1)
                        break; // a polygon always has the starting/end point twice, so skip the end point label for polygons.

                    var point = points[i];
                    var originPoint = new PointClass { X = point.X + offset, Y = point.Y + offset };
                    var labelPoint = new PointClass { X = point.X + offset, Y = point.Y + offset/2 };
                    var labelText = (i+1).ToString();

                    display.SetSymbol(boxSymbol);
                    display.DrawPolygon(GetLabelBox(display, originPoint));

                    display.SetSymbol(textSymbol);
                    display.DrawText(labelPoint, labelText);
                }

                display.FinishDrawing();
            }
        }

        private static bool CheckEditTask()
        {
            bool result = false;
            IEditor3 editor = ArcUtils.Editor;

            if (editor != null)
            {
                IEditTask task = editor.CurrentTask;

                if (task != null)
                {
                    var uniqueName = task as IEditTaskName;

                    if (uniqueName != null)
                    {
                        string name = uniqueName.UniqueName;
                        result = (name == "GarciaUI_CreateNewFeatureTask") || (name == "GarciaUI_ReshapeFeatureTask") || (name == "GarciaUI_ModifyFeatureTask");
                    }
                }
            }

            return result;
        }

        public static void UpdateEditGeometry(IEditor3 editor, ICommand command)
        {
            var sketch = editor as IEditSketch3;

            if (sketch != null)
            {
                var geometry = sketch.Geometry;

                if (!(command is IEditTool) && editor.EditState != esriEditState.esriStateNotEditing && StartMeasurementEvent != null)
                {
                    var typeOfLayer = GetTypeOfLayer(geometry.GeometryType);

                    StartMeasurementEvent(typeOfLayer);
                }
            }
        }

        private static ESRI.ArcGIS.Geometry.Point ConvertToPoint(ICoordinate coord, bool withZ)
        {
            if (coord.X.HasValue && coord.Y.HasValue)
            {
                var newEditPoint = new ESRI.ArcGIS.Geometry.Point();
                newEditPoint.X = coord.X.Value;
                newEditPoint.Y = coord.Y.Value;
                if (withZ)
                {
                    (newEditPoint as IZAware).ZAware = true;
                    newEditPoint.Z = coord.Z.HasValue ? coord.Z.Value : 0;
                }

                return newEditPoint;
            }
            return null;
        }

        private static Polyline ConvertToPolyline(List<ICoordinate> coords, bool withZ)
        {
            Polyline polyline = new Polyline();
            ((PolylineClass)(polyline)).ZAware = withZ;

            foreach (var coord in coords)
            {
                var point = ConvertToPoint(coord, withZ);

                polyline.AddPoint(point);
            }

            return polyline;
        }

        public static void FinishMeasurement()
        {
            var sketch = ArcUtils.Editor as IEditSketch3;

            if (sketch != null && sketch.Geometry != null && !sketch.Geometry.IsEmpty)
                sketch.FinishSketch();
        }

        public static bool IsNewMeasurement(IFeatureCollection features)
        {
            if (features != null && features.Features.Count > 0)
            {
                var id = features.Features.First().Properties.Where(p => p.Key == "Id").Select(p => p.Value.ToString()).FirstOrDefault();

                if (CurrentMeasurementID != id)
                {
                    CurrentMeasurementID = id;

                    return true;
                }
            }

            return false;
        }

        public static void CreateMeasurement(IFeatureCollection features)
        {
            var activeView = ArcUtils.ActiveView;
            var display = activeView.ScreenDisplay;
            var editor = ArcUtils.Editor as IEditLayers;
            var layer = VectorLayer.GetLayer(editor.CurrentLayer);
            if (layer == null)
                return; // nothing to draw in!

            var isNew = IsNewMeasurement(features);

            //Type is unknown when measurement is closed in Street Smart or a new one is started.
            if (features.Type == FeatureType.Unknown)
            {
                FinishMeasurement();

                ArcUtils.Editor.StopEditing(true);
            }

            if (!StreetSmartApiWrapper.Instance.BusyForMeasurement)
            {
                StreetSmartApiWrapper.Instance.BusyForMeasurement = true;

                foreach (var feature in features.Features)
                {
                    switch (feature.Geometry.Type)
                    {
                        case GeometryType.Point: // always add a new point
                            var coord = feature.Geometry as ICoordinate;
                            var point = ConvertToPoint(coord, layer.HasZ);
                            if (point != null)
                            {
                                var newEditFeature = layer._featureClass.CreateFeature();
                                newEditFeature.Shape = point;
                                newEditFeature.Store();
                            }

                            break;

                        case GeometryType.LineString:
                            var coords = feature.Geometry as List<ICoordinate>;

                            if (coords != null)
                            {
                                var sketch = editor as IEditSketch3;

                                if (isNew)
                                {
                                    if (sketch != null && sketch.Geometry != null && !sketch.Geometry.IsEmpty)
                                    {
                                        // New measurement from Street Smart
                                        sketch.FinishSketch();
                                    }
                                    else
                                    {
                                        // New measurement from map
                                    }
                                }
                                else
                                {
                                    sketch.Geometry = ConvertToPolyline(coords, layer.HasZ) as ESRI.ArcGIS.Geometry.IGeometry;
                                    sketch.RefreshSketch();
                                }
                            }

                            break;

                        case GeometryType.Polygon:
                            var polygon = feature.Geometry as StreetSmart.Common.Interfaces.GeoJson.IPolygon;
                            if (polygon != null)
                            {
                                var sketch = editor as IEditSketch3;
                                var newPolygon = new ESRI.ArcGIS.Geometry.Polygon();

                                foreach (var coordinate in polygon[0])
                                {
                                    var pointInPolygon = ConvertToPoint(coordinate, layer.HasZ);

                                    newPolygon.AddPoint(pointInPolygon);
                                }

                                if (sketch != null && newPolygon.PointCount > 0)
                                {
                                    (newPolygon as IZAware).ZAware = true;

                                    sketch.Geometry = (ESRI.ArcGIS.Geometry.IGeometry)newPolygon;
                                    sketch.RefreshSketch();
                                }
                            }

                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                StreetSmartApiWrapper.Instance.BusyForMeasurement = false;
            }
        }

        #endregion functions (static)

        #region functions (public)

        // =========================================================================
        // Functions (Public)
        // =========================================================================

        public void SetVisibility(bool isVisible)
        {
            OnLayerChanged(this);

            IEditor3 editor = ArcUtils.Editor;

            if (editor != null)
            {
                UpdateCurrentLayer(editor as IEditLayers, isVisible);
                UpdateEditFeature(editor as IEditSketch3, isVisible);
            }
        }

        public void UpdateCurrentLayer(IEditLayers editor, bool isVisible)
        {
            if (editor != null && editor.CurrentLayer != null)
            {
                VectorLayer vectorLayer = GetLayer(editor.CurrentLayer);

                if (vectorLayer == this)
                {
                    if (isVisible)
                    {
                        OnSketchModified();
                    }
                    else if (CheckEditTask() && SketchFinishedEvent != null)
                    {
                        SketchFinishedEvent();
                        AvContentChanged();
                    }
                }
            }
        }

        public void UpdateEditFeature(IEditSketch3 editor, bool isVisible)
        {
            foreach (var editFeature in EditFeatures)
            {
                VectorLayer vectorLayer = GetLayer(editFeature);

                if (vectorLayer == this)
                {
                    if (isVisible)
                    {
                        var sketch = editor as IEditSketch3;

                        if (sketch?.Geometry != null)
                        {
                            var typeOfLayer = GetTypeOfLayer(sketch.Geometry.GeometryType);

                            StartMeasurementEvent(typeOfLayer);
                        }
                    }
                    else
                    {
                        FeatureDeleteEvent(editFeature);
                    }
                }
            }
        }

        public Color GetColor(out Color outline)
        {
            outline = Color.Black;

            if (_layer != null)
                return ArcUtils.GetColorFromLayer(_layer, out outline);
            else
                return Color.Black;
        }

        public ISymbolizer CreateSymbolizer(Color color, Color outline)
        {
            switch (TypeOfLayer)
            {
                case TypeOfLayer.Point:
                    return SLDFactory.CreateStylePoint(SymbolizerType.Circle, 10, color, 75, outline, 0);

                case TypeOfLayer.Line:
                    return SLDFactory.CreateStyleLine(color);

                case TypeOfLayer.Polygon:
                    return SLDFactory.CreateStylePolygon(color, 75);

                default:
                    return default(ISymbolizer);
            }
        }

        public IStyledLayerDescriptor CreateSld(IFeatureCollection featureCollection, Color color, Color outline)
        {
            // according to specs, we only need color.
            if (featureCollection.Features.Count >= 1)
            {
                Sld = SLDFactory.CreateEmptyStyle();

                var symbolizer = CreateSymbolizer(color, outline);
                var rule = SLDFactory.CreateRule(symbolizer);

                SLDFactory.AddRuleToStyle(Sld, rule);
            }

            return Sld;
        }

        public IFeatureCollection GenerateJson(IList<IRecording> recordingLocations)
        {
            var spatRel = Config.SpatialReference;
            string srsName = (spatRel == null) ? ArcUtils.EpsgCode : spatRel.SRSName;
            string layerName = _layer.Name;
            int srs = int.Parse(srsName.Replace("EPSG:", string.Empty));
            var featureCollection = GeoJsonFactory.CreateFeatureCollection(srs);

            var featureLayer = _layer as IFeatureLayer;
            if (featureLayer != null)
            {
                IFeatureClass fc = featureLayer.FeatureClass;

                ESRI.ArcGIS.Geometry.IGeometry geometryBag = new GeometryBag();
                var geometryCollection = geometryBag as IGeometryCollection;
                ISpatialReference gsSpatialReference = (spatRel == null) ? ArcUtils.SpatialReference : spatRel.SpatialRef;

                double distance = CalculateDistance(gsSpatialReference);

                foreach (var recordingLocation in recordingLocations)
                {
                    var envelope = CreateEnvelope(recordingLocation, distance, gsSpatialReference, SpatialReference);

                    geometryCollection.AddGeometry(envelope);
                }

                ITopologicalOperator unionedPolygon = (ITopologicalOperator)new Polygon();
                unionedPolygon.ConstructUnion(geometryBag as IEnumGeometry);
                var polygon = unionedPolygon as ESRI.ArcGIS.Geometry.IPolygon;

                ISpatialFilter spatialFilter = new SpatialFilter
                {
                    Geometry = polygon,
                    GeometryField = _featureClass.ShapeFieldName,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
                };

                UpdateFeatures(featureCollection, spatialFilter, srs, gsSpatialReference);
            }

            ContentsChanged = featureCollection != _contents;
            _contents = featureCollection;
            return featureCollection;
        }

        public double CalculateDistance(ISpatialReference sourceSpatialReference)
        {
            double distance = Config.OverlayDrawDistanceInMeters * 1.0;

            var projCoord = sourceSpatialReference as IProjectedCoordinateSystem;
            if (projCoord == null)
            {
                var geoCoord = sourceSpatialReference as IGeographicCoordinateSystem;

                if (geoCoord != null)
                    distance *= geoCoord.CoordinateUnit.ConversionFactor;
            }
            else
            {
                distance /= projCoord.CoordinateUnit.ConversionFactor;
            }

            return distance;
        }

        public IEnvelope2 CreateEnvelope(IRecording recordingLocation, double distance, ISpatialReference sourceSpatialReference, ISpatialReference targetSpatialReference)
        {
            double x = recordingLocation.XYZ.X.Value;
            double y = recordingLocation.XYZ.Y.Value;

            var result = new Envelope()
            {
                XMin = x - distance,
                XMax = x + distance,
                YMin = y - distance,
                YMax = y + distance,
            } as IEnvelope2;

            result.SpatialReference = sourceSpatialReference;
            result.Project(targetSpatialReference);

            return result;
        }

        public void UpdateFeatures(IFeatureCollection features, ISpatialFilter spatialFilter, int srs, ISpatialReference spatialReference)
        {
            var featureCursor = _featureClass.Search(spatialFilter, false);
            var featureCount = _featureClass.FeatureCount(spatialFilter);
            var shapeId = featureCursor.FindField(_featureClass.ShapeFieldName);

            for (int i = 0; i < featureCount; i++)
            {
                var feature = featureCursor.NextFeature();

                if (!EditFeatures.Contains(feature))
                {
                    var fields = feature.Fields;
                    var fieldValues = new Dictionary<string, string> { { "FEATURECLASSNAME", _featureClass.AliasName } };

                    for (int j = 0; j < fields.FieldCount; j++)
                    {
                        var field = fields.Field[j];
                        string name = field.Name;
                        int id = featureCursor.FindField(name);

                        string value = (id != shapeId)
                          ? feature.get_Value(id).ToString()
                          : _featureClass.ShapeType.ToString().Replace("esriGeometry", string.Empty);
                        fieldValues.Add(name, value);
                    }

                    var shapeVar = feature.get_Value(shapeId);
                    var geometry = shapeVar as ESRI.ArcGIS.Geometry.IGeometry;
                    if (geometry != null)
                    {
                        var cyclSpatialRef = new SpatialReferenceEnvironmentClass().CreateSpatialReference(srs);
                        geometry.Project((Config.ApiSRS == null) ? spatialReference : cyclSpatialRef);

                        var pointCollection = geometry as IPointCollection4;

                        if (pointCollection != null)
                        {
                            var pointCollectionJson = new List<ICoordinate>();
                            for (int j = 0; j < pointCollection.PointCount; j++)
                            {
                                ESRI.ArcGIS.Geometry.IPoint point = pointCollection.Point[j];

                                if (point != null)
                                {
                                    if (!HasZ)
                                    {
                                        point.Z = double.NaN;
                                    }
                                }
                                ICoordinate pJson = CoordinateFactory.Create(point.X, point.Y, point.Z);
                                pointCollectionJson.Add(pJson);
                            }

                            var points = new List<IList<ICoordinate>> { pointCollectionJson };

                            if (TypeOfLayer == TypeOfLayer.Line)
                            {
                                if (points.Count > 0)
                                {
                                    var geomJson = GeoJsonFactory.CreateLineFeature(points.FirstOrDefault());
                                    features.Features.Add(geomJson);
                                }
                            }
                            else if (TypeOfLayer == TypeOfLayer.Polygon)
                            {
                                var geomJson = GeoJsonFactory.CreatePolygonFeature(points);
                                features.Features.Add(geomJson);
                            }
                        }
                        else
                        {
                            var point = geometry as ESRI.ArcGIS.Geometry.IPoint;

                            if (point != null)
                            {
                                if (!HasZ)
                                {
                                    point.Z = double.NaN;
                                }
                                shapeVar = point;
                                ICoordinate pJson = CoordinateFactory.Create(point.X, point.Y);
                                var geomJson = GeoJsonFactory.CreatePointFeature(pJson);
                                features.Features.Add(geomJson);
                            }
                        }
                    }

                    foreach (var fieldValue in fieldValues)
                    {
                        if (features.Features.Count >= 1)
                        {
                            var properties = features.Features[features.Features.Count - 1].Properties;

                            if (!properties.ContainsKey(fieldValue.Key))
                                properties.Add(fieldValue.Key, fieldValue.Value);
                        }
                    }
                }
            }
        }

        #endregion functions (public)

        #region event handlers

        // =========================================================================
        // Event handlers
        // =========================================================================
        private static void OpenDocument()
        {
            try
            {
                DetectVectorLayers(false);
                //AddEvents();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.OpenDocument");
            }
        }

        private static void CloseDocument()
        {
            try
            {
                var avEvents = ArcUtils.ActiveViewEvents;

                if (avEvents != null)
                {
                    avEvents.ItemAdded -= AvItemAdded;
                    avEvents.ItemDeleted -= AvItemDeleted;
                    avEvents.ContentsChanged -= AvContentChanged;
                }

                while (Layers.Count >= 1)
                {
                    AvItemDeleted(Layers[0]);
                    Layers.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.CloseDocument");
            }
        }

        private static void AvItemAdded(object item)
        {
            try
            {
                if (item != null)
                {
                    var featureLayer = item as IFeatureLayer;
                    var extension = StreetSmartExtension.GetExtension();

                    if ((featureLayer != null) && (extension != null))
                    {
                        CycloMediaGroupLayer cycloGrouplayer = extension.CycloMediaGroupLayer;

                        if (cycloGrouplayer != null)
                        {
                            if (!cycloGrouplayer.IsKnownName(featureLayer.Name))
                            {
                                var dataset = item as IDataset;

                                if (dataset != null)
                                {
                                    var featureWorkspace = dataset.Workspace as IFeatureWorkspace;

                                    if (featureWorkspace != null)
                                    {
                                        var featureClass = featureLayer.FeatureClass;

                                        var vectorLayer = new VectorLayer
                                        {
                                            _featureClass = featureClass,
                                            _layer = featureLayer,
                                            IsVisibleInStreetSmart = StoredLayers.Instance.Get(featureLayer.Name)
                                        };

                                        _layers.Add(vectorLayer);

                                        LayerAddEvent?.Invoke(vectorLayer);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.AvItemAdded");
            }
        }

        private static void AvItemDeleted(object item)
        {
            try
            {
                if (item != null)
                {
                    var featureLayer = item as IFeatureLayer;

                    if (featureLayer != null)
                    {
                        int i = 0;

                        while (Layers.Count > i)
                        {
                            if (Layers[i]._layer == featureLayer)
                            {
                                LayerRemoveEvent?.Invoke(Layers[i]);

                                Layers.RemoveAt(i);
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.AvItemDeleted");
            }
        }

        private static void AvContentChanged()
        {
            OnLayerChanged(null);

            _layers = new List<VectorLayer>();
            IMap map = ArcUtils.Map;

            if (map != null)
            {
                var layers = map.get_Layers();
                ILayer layer;

                while ((layer = layers.Next()) != null)
                {
                    AvItemAdded(layer);
                }
            }
        }

        private static void OnStartEditing()
        {
            LogClient.Info("On StartEditing");

            ArcUtils.Editor?.Map?.ClearSelection();
        }

        private static void OnLayerChanged(VectorLayer layer)
        {
            try
            {
                LayerChangedEvent?.Invoke(layer);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.AvContentChanged");
            }
        }

        private static void OnChangeFeature(IObject obj)
        {
            try
            {
                var feature = obj as ESRI.ArcGIS.Geodatabase.IFeature;
                LogClient.Info(string.Format("On Change Feature: {0}", ((feature != null) ? feature.Class.AliasName : string.Empty)));

                if ((FeatureUpdateEditEvent != null) && (feature != null))
                {
                    if (!EditFeatures.Contains(feature))
                    {
                        EditFeatures.Add(feature);
                    }

                    VectorLayer vectorLayer = GetLayer(feature);

                    if ((vectorLayer != null) && (vectorLayer.IsVisibleInStreetSmart))
                    {
                        FeatureUpdateEditEvent(feature);
                        AvContentChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnChangeFeature", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnChangeFeature");
            }
        }

        private static async void OnSelectionChanged()
        {
            try
            {
                IApplication application = ArcMap.Application;
                ICommandItem tool = application.CurrentTool;
                string name = tool.Name;

                IEditor3 editor = ArcUtils.Editor;
                LogClient.Info("On Selection Changed");

                if (editor != null)
                {
                    IEnumFeature editSelection = editor.EditSelection;

                    if (editSelection != null)
                    {
                        editSelection.Reset();
                        EditFeatures.Clear();
                        ESRI.ArcGIS.Geodatabase.IFeature feature;

                        while ((feature = editSelection.Next()) != null)
                        {
                            EditFeatures.Add(feature);
                        }
                        //if (EditFeatures.Count > 0 && !StreetSmartApiWrapper.Instance.BusyForMeasurement)
                        //{
                           
                        //    await StreetSmartApiWrapper.Instance.CreateMeasurement(GetTypeOfLayer(EditFeatures[0].Shape.GeometryType));
                        //    StreetSmartApiWrapper.Instance.UpdateActiveMeasurement(EditFeatures[0].Shape);
                        //}
                        if (FeatureStartEditEvent != null)
                        {
                            var geometries = new List<ESRI.ArcGIS.Geometry.IGeometry>();
                            editSelection.Reset();
                            bool isPointLayer = false;

                            while ((feature = editSelection.Next()) != null)
                            {
                                VectorLayer vectorLayer = GetLayer(feature);

                                if ((vectorLayer != null) && (vectorLayer.IsVisibleInStreetSmart))
                                {
                                    geometries.Add(feature.Shape);
                                    //isPointLayer = isPointLayer || (Measurement.GetTypeOfLayer(feature.Shape) == TypeOfLayer.Point); // TODO: Measurement
                                }
                            }

                            if ((_doSelection && (name != "Query_SelectFeatures")) || (!isPointLayer))
                            {
                                //TODO: I think this is breaking more then it fixes...
                                //FeatureStartEditEvent?.Invoke(geometries);

                                AvContentChanged();

                                _doSelection = false;

                                if (_nextSelectionTimer == null)
                                {
                                    var checkEvent = new AutoResetEvent(true);
                                    const int timeOutTime = 1500; 

                                    var checkTimerCallBack = new TimerCallback(CheckTimerCallBack);

                                    _nextSelectionTimer = new Timer(checkTimerCallBack, checkEvent, timeOutTime, -1);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnSelectionChanged", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnSelectionChanged");
            }
        }

        private static void CheckTimerCallBack(object state)
        {
            _doSelection = true;
            _nextSelectionTimer = null;
        }

        private static void OnDeleteFeature(IObject obj)
        {
            try
            {
                var feature = obj as ESRI.ArcGIS.Geodatabase.IFeature;
                LogClient.Info(string.Format("On Delete Feature: {0}", ((feature != null) ? feature.Class.AliasName : string.Empty)));

                if (FeatureDeleteEvent != null && feature != null)
                {
                    if (EditFeatures.Contains(feature))
                    {
                        EditFeatures.Remove(feature);
                    }

                    VectorLayer vectorLayer = GetLayer(feature);

                    if ((vectorLayer != null) && (vectorLayer.IsVisibleInStreetSmart))
                    {
                        FeatureDeleteEvent(feature);
                        AvContentChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnDeleteFeature", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnDeleteFeature");
            }
        }

        private static void OnStopEditing(bool save)
        {
            try
            {
                LogClient.Info(string.Format("On Stop Editing: {0}", save));
                EditFeatures.Clear();
                _doSelection = true;

                if (StopEditEvent != null)
                {
                    StopEditEvent();
                    AvContentChanged();
                }

                if (_editToolCheckTimer != null)
                {
                    _editToolCheckTimer.Dispose();
                    _editToolCheckTimer = null;
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnStopEditing", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnStopEditing");
            }
        }

        private static void OnSketchModified()
        {
            try
            {
                LogClient.Info("On Sketch Modified");
                _doSelection = true;

                if (_editToolCheckTimer == null)
                {
                    var checkEvent = new AutoResetEvent(true);
                    var checkTimerCallBack = new TimerCallback(EditToolCheck);
                    const int checkTime = 1000;
                    _editToolCheckTimer = new Timer(checkTimerCallBack, checkEvent, checkTime, checkTime);
                }

                IEditor3 editor = ArcUtils.Editor;

                if (editor != null)
                {
                    var sketch = editor as IEditSketch3;
                    var editLayers = editor as IEditLayers;

                    if (sketch != null && editLayers != null)
                    {
                        ILayer currentLayer = editLayers.CurrentLayer;
                        VectorLayer vectorLayer = GetLayer(currentLayer);

                        if (vectorLayer != null && vectorLayer.IsVisibleInStreetSmart && CheckEditTask())
                        {
                            var geometry = sketch.Geometry;
                            var lastPoint = sketch.LastPoint;

                            if (lastPoint != null && lastPoint.Z == 0 && SketchCreateEvent != null && sketch.ZAware)
                                SketchCreateEvent(sketch);

                            if (!StreetSmartApiWrapper.Instance.BusyForMeasurement)
                                SketchModifiedEvent?.Invoke(geometry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnSketchModified", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnSketchModified");
            }
        }

        private static void OnSketchFinished()
        {
            try
            {
                IEditor3 editor = ArcUtils.Editor;
                LogClient.Info("On Sketch Finished");
                _doSelection = true;

                if (editor != null)
                {
                    var editLayers = editor as IEditLayers;

                    if (editLayers != null)
                    {
                        ILayer currentLayer = editLayers.CurrentLayer;
                        VectorLayer vectorLayer = GetLayer(currentLayer);

                        if (CheckEditTask() && vectorLayer != null && vectorLayer.IsVisibleInStreetSmart)
                        {
                            if (SketchFinishedEvent != null)
                            {
                                SketchFinishedEvent();
                                AvContentChanged();
                            }

                            OnSelectionChanged();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogClient.Error("VectorLayer.OnSketchFinished", ex.Message, ex);
                Trace.WriteLine(ex.Message, "VectorLayer.OnSketchFinished");
            }
        }

        #endregion event handlers

        #region thread functions

        // =========================================================================
        // Thread functions
        // =========================================================================
        private static void EditToolCheck(object context)
        {
            try
            {
                lock (LockObject)
                {
                    IApplication application = ArcMap.Application;
                    IActiveView activeView = ArcUtils.ActiveView;

                    if ((application != null) && (activeView != null))
                    {
                        ICommandItem tool = application.CurrentTool;

                        if ((tool != null) && ((_beforeTool == null) || (_beforeTool.Name != tool.Name)))
                        {
                            activeView.Refresh();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.OnEditToolCheck");
            }
        }

        private static void AvViewRefreshed(IActiveView view, esriViewDrawPhase phase, object data, IEnvelope envelope)
        {
            try
            {
                IEditor3 editor = ArcUtils.Editor;
                IApplication application = ArcMap.Application;

                if ((editor != null) && (application != null))
                {
                    var editLayers = editor as IEditLayers;

                    if (editLayers != null)
                    {
                        ILayer currentLayer = editLayers.CurrentLayer;
                        VectorLayer vectorLayer = (currentLayer == null) ? null : GetLayer(currentLayer);
                        ICommandItem tool = application.CurrentTool;

                        if ((tool != null) && ((_beforeTool == null) || (_beforeTool.Name != tool.Name)))
                        {
                            _beforeTool = tool;

                            if ((vectorLayer != null) && (vectorLayer.IsVisibleInStreetSmart))
                            {
                                ICommandItem editorMenu = application.Document.CommandBars.Find("Editor_EditTool");
                                string editorCategory = (editorMenu != null) ? editorMenu.Category : string.Empty;
                                ICommand command = tool.Command;
                                string category = tool.Category;

                                if (category == editorCategory)
                                {
                                    UpdateEditGeometry(editor, command);
                                }
                            }
                            else
                            {
                                SketchFinishedEvent?.Invoke();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "VectorLayer.AvViewRefreshed");
            }
        }

        #endregion thread functions
    }
}