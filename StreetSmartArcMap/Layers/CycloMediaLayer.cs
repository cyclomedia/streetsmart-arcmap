﻿/*
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

using ESRI.ArcGIS.ADF.Connection.Local;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Logic.Model;
using StreetSmartArcMap.Logic.Model.Shape;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using Point = ESRI.ArcGIS.Geometry.Point;
using ShapePoint = StreetSmartArcMap.Logic.Model.Shape.Point;

namespace StreetSmartArcMap.Layers
{
    // ===========================================================================
    // Delegates
    // ===========================================================================
    public delegate void CycloMediaLayerAddedDelegate(CycloMediaLayer cycloMediaLayer);

    public delegate void CycloMediaLayerChangedDelegate(CycloMediaLayer cycloMediaLayer);

    public delegate void CycloMediaLayerRemoveDelegate(CycloMediaLayer cycloMediaLayer);

    public delegate void HistoricalDateDelegate(SortedDictionary<int, int> yearMonth);

    public abstract class CycloMediaLayer : IDisposable
    {
        #region members

        // =========================================================================
        // Members
        // =========================================================================
        public static event CycloMediaLayerAddedDelegate LayerAddedEvent;

        public static event CycloMediaLayerChangedDelegate LayerChangedEvent;

        public static event CycloMediaLayerRemoveDelegate LayerRemoveEvent;

        public static event HistoricalDateDelegate HistoricalDateChanged;

        private static SortedDictionary<int, int> _yearMonth;

        private readonly CycloMediaGroupLayer _cycloMediaGroupLayer;
        private readonly object _getDataLock;

        private IFeatureClass _featureClass;
        private ILayer _layer;
        private IEnvelope _lastextent;
        private List<XElement> _addData;
        private Thread _getDataThread;
        private Thread _refreshDataThread;
        private Thread _refreshDataThread2;
        private bool _isVisibleInGlobespotter;

        #endregion members

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public abstract string[] FieldNames { get; }

        public abstract string Name { get; }
        public abstract string FcName { get; }
        public abstract Color Color { get; set; }
        public abstract Color Outline { get; set; }
        public abstract int SizeLayer { get; }
        public abstract double MinimumScale { get; set; }
        public abstract bool UseDateRange { get; }
        public abstract string WfsRequest { get; }

        public bool Visible { get; set; }
        public bool IsRemoved { get; set; }

        public ILayer Layer
        {
            get { return _layer ?? (_layer = GetLayer()); }
        }

        public IGeometryDef GeometryDef
        {
            get
            {
                IGeometryDef result = null;

                if (FeatureClass != null)
                {
                    // ReSharper disable UseIndexedProperty
                    IFields fields = FeatureClass.Fields;
                    int fieldId = fields.FindField(FeatureClass.ShapeFieldName);
                    IField field = fields.get_Field(fieldId);
                    result = field.GeometryDef;
                    // ReSharper restore UseIndexedProperty
                }

                return result;
            }
        }

        public ISpatialReference SpatialReference
        {
            get { return (GeometryDef == null) ? null : GeometryDef.SpatialReference; }
        }

        public string EpsgCode
        {
            get
            {
                Configuration.Configuration config = Configuration.Configuration.Instance;
                string defaultRecordingSrs = config.DefaultRecordingSrs;
                int factoryCode;

                if ((string.IsNullOrEmpty(defaultRecordingSrs)) || (!int.TryParse(defaultRecordingSrs, out factoryCode)))
                {
                    factoryCode = (SpatialReference == null) ? 0 : SpatialReference.FactoryCode;
                }

                return string.Format("EPSG:{0}", factoryCode);
            }
        }

        public bool IsVisible
        {
            get { return (_layer != null) && _layer.Visible; }
            set
            {
                if (_layer != null)
                {
                    _layer.Visible = value;
                }
            }
        }

        public bool IsVisibleInStreetSmart
        {
            get { return (_isVisibleInGlobespotter && IsVisible); }
            set
            {
                _isVisibleInGlobespotter = value;
                OnContentChanged();
            }
        }

        public bool InsideScale
        {
            get
            {
                IMap map = ArcUtils.Map;
                return (map != null) && (Math.Floor(map.MapScale) <= (MinimumScale = Layer.MinimumScale));
            }
        }

        public IFeatureClass FeatureClass
        {
            get { return _featureClass ?? (_featureClass = OpenFeatureClassInWorkspace()); }
        }

        protected abstract IMappedFeature CreateMappedFeature(XElement mappedFeatureElement);

        protected abstract bool Filter(IMappedFeature mappedFeature);

        protected abstract void PostEntryStep();

        protected static SortedDictionary<int, int> YearMonth
        {
            get { return _yearMonth ?? (_yearMonth = new SortedDictionary<int, int>()); }
        }

        #endregion properties

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        protected CycloMediaLayer(CycloMediaGroupLayer layer)
        {
            _getDataLock = new object();
            _addData = null;
            _getDataThread = null;
            _cycloMediaGroupLayer = layer;
            _isVisibleInGlobespotter = true;
            IActiveView activeView = ArcUtils.ActiveView;
            Visible = false;
            IsRemoved = true;

            if (activeView != null)
            {
                _lastextent = activeView.Extent;
            }
        }

        #endregion constructor

        #region functions (public)

        // =========================================================================
        // Functions (Public)
        // =========================================================================
        public void AddToLayers()
        {
            IList<CycloMediaLayer> layers = _cycloMediaGroupLayer.Layers;

            if (!layers.Contains(this))
            {
                layers.Add(this);
            }

            CreateFeatureLayer();
            Refresh();
        }

        public void CreateFeatureLayer()
        {
            if (Layer == null)
            {
                _layer = new FeatureLayer()
                {
                    Name = Name,
                    MinimumScale = MinimumScale,
                    Visible = true
                };

                IFeatureClass featureClass = OpenFeatureClassInWorkspace();

                if (featureClass != null)
                {
                    var featureLayer = _layer as IFeatureLayer;
                    featureLayer.FeatureClass = featureClass;
                }

                CreateUniqueValueRenderer();
            }
            else
            {
                CreateUniqueValueRenderer();
            }

            if (_cycloMediaGroupLayer != null)
            {
                IGroupLayer groupLayer = _cycloMediaGroupLayer.GroupLayer;

                if (groupLayer != null)
                {
                    ILayer layer = GetLayer();

                    if (layer == null)
                    {
                        groupLayer.Add(_layer);
                    }
                }
            }

            if (LayerAddedEvent != null)
            {
                LayerAddedEvent(this);
            }

            IsRemoved = false;
            var avEvents = ArcUtils.ActiveViewEvents;

            if (avEvents != null)
            {
                avEvents.ViewRefreshed += OnViewRefreshed;
                avEvents.ContentsChanged += OnContentChanged;
                avEvents.ItemDeleted += OnItemDeleted;
                avEvents.AfterDraw += OnAfterDraw;
            }
        }

        public void OnAfterDraw(IDisplay adisplay, esriViewDrawPhase phase)
        {
            if (phase == esriViewDrawPhase.esriViewGeoSelection)
            {
                _refreshDataThread2 = new Thread(DrawLayer);
                _refreshDataThread2.Start();
            }
        }

        public void Dispose()
        {
            IGroupLayer groupLayer = _cycloMediaGroupLayer.GroupLayer;

            if (groupLayer != null)
            {
                groupLayer.Delete(_layer);
            }

            Remove();
        }

        public void Refresh()
        {
            IActiveView activeView = ArcUtils.ActiveView;

            if (activeView != null)
            {
                _lastextent = activeView.Extent.Envelope;
                _lastextent.Expand(5, 5, true);
                activeView.PartialRefresh(esriViewDrawPhase.esriViewForeground, Layer, null);
                activeView.ContentsChanged();
            }
        }

        public string GetFeatureFromPoint(int x, int y)
        {
            string result = string.Empty;
            IActiveView activeView = ArcUtils.ActiveView;

            if ((activeView != null) && InsideScale)
            {
                try
                {
                    IMappedFeature mapFeature = CreateMappedFeature(null);
                    string objectId = mapFeature.ObjectId;
                    IScreenDisplay screenDisplay = activeView.ScreenDisplay;
                    IDisplayTransformation dispTrans = screenDisplay.DisplayTransformation;
                    IPoint pointLu = dispTrans.ToMapPoint(x - SizeLayer, y - SizeLayer);
                    IPoint pointRd = dispTrans.ToMapPoint(x + SizeLayer, y + SizeLayer);

                    IEnvelope envelope = (IEnvelope)new Envelope()
                    {
                        XMin = pointLu.X,
                        XMax = pointRd.X,
                        YMin = pointLu.Y,
                        YMax = pointRd.Y
                    };
                    envelope.SpatialReference = ArcUtils.SpatialReference;
                    envelope.Project(SpatialReference);

                    ISpatialFilter spatialFilter = new SpatialFilter()
                    {
                        Geometry = envelope,
                        GeometryField = FeatureClass.ShapeFieldName,
                        SpatialRel = esriSpatialRelEnum.esriSpatialRelContains,
                        SubFields = objectId
                    };

                    var existsResult = FeatureClass.Search(spatialFilter, false);
                    try
                    {
                        IFeature feature;

                        // ReSharper disable UseIndexedProperty
                        while ((feature = existsResult.NextFeature()) != null)
                        {
                            int imId = existsResult.FindField(objectId);
                            result = (string)feature.get_Value(imId);
                        }
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(existsResult);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message, "CycloMediaLayer.GetFeatureFromPoint");
                }
                // ReSharper restore UseIndexedProperty
            }

            return result;
        }

        public IMappedFeature GetLocationInfo(string imageId)
        {
            IMappedFeature result = CreateMappedFeature(null);
            var fields = result.Fields;
            string shapeFieldName = result.ShapeFieldName;

            IQueryFilter filter = new QueryFilter()
            {
                WhereClause = string.Format("ImageId = '{0}'", imageId),
                SubFields = string.Format("{0}, {1}", fields.Aggregate(string.Empty, (current, field) => string.Format
                  ("{0}{1}{2}", current, string.IsNullOrEmpty(current) ? string.Empty : ", ", field.Key)), shapeFieldName)
            };

            var existsResult = FeatureClass.Search(filter, false);
            try
            {
                IFeature feature = existsResult.NextFeature();
                // ReSharper disable UseIndexedProperty

                if (feature != null)
                {
                    foreach (var field in result.Fields)
                    {
                        string name = field.Key;
                        int nameId = existsResult.FindField(name);
                        object item = feature.get_Value(nameId);
                        result.UpdateItem(name, item);
                    }

                    var point = feature.Shape as IPoint;
                    result.UpdateItem(shapeFieldName, point);
                }
                else
                {
                    result = null;
                }
            }
            finally
            {
                Marshal.ReleaseComObject(existsResult);
            }
            // ReSharper restore UseIndexedProperty
            return result;
        }

        public IFeature GetFeature(string imageId)
        {
            IMappedFeature mappedFeature = CreateMappedFeature(null);
            var fields = mappedFeature.Fields;
            string shapeFieldName = mappedFeature.ShapeFieldName;

            IQueryFilter filter = new QueryFilterClass
            {
                WhereClause = string.Format("ImageId = '{0}'", imageId),
                SubFields = string.Format("{0}, {1}", fields.Aggregate("ObjectId", (current, field) => string.Format
                  ("{0}{1}{2}", current, string.IsNullOrEmpty(current) ? string.Empty : ", ", field.Key)), shapeFieldName)
            };

            var existsResult = FeatureClass.Search(filter, false);
            try
            {
                return existsResult.NextFeature();
            }
            finally
            {
                Marshal.ReleaseComObject(existsResult);
            }
        }

        public void AddZToSketch(IEditSketch3 sketch)
        {
            var editor = ArcUtils.Editor;
            var editorZ = editor as IEditorZ;
            IPoint point = sketch.LastPoint;

            if ((editorZ != null) && (point != null) && sketch.ZAware)
            {
                double z = GetHeight(point.X, point.Y);
                editorZ.UseZOffset = true;
                editorZ.ZOffset = z;

                IGeometry geometry = sketch.Geometry;
                var ptColl = geometry as IPointCollection4;
                ISketchOperation2 sketchOp = new SketchOperationClass();
                sketchOp.Start(editor);
                IPoint pointc = null;

                if ((!geometry.IsEmpty) && (geometry is IPoint) && (ptColl == null))
                {
                    pointc = geometry as IPoint;
                    ptColl = new MultipointClass();
                    ptColl.AddPoint(pointc);
                }

                if (ptColl != null)
                {
                    int nrPoints = ptColl.PointCount;

                    for (int i = 0; i < nrPoints; i++)
                    {
                        IPoint pointC = ptColl.Point[i];
                        // ReSharper disable CompareOfFloatsByEqualityOperator

                        if (pointC.Z == 0)
                        {
                            IPoint newPoint = new PointClass { X = pointC.X, Y = pointC.Y, Z = z };
                            ptColl.UpdatePoint(i, newPoint);
                        }

                        // ReSharper restore CompareOfFloatsByEqualityOperator
                    }
                }

                sketch.Geometry = pointc ?? (ptColl as IGeometry);
                geometry = sketch.Geometry;

                if (geometry != null)
                {
                    sketchOp.Finish(geometry.Envelope, esriSketchOperationType.esriSketchOperationGeneral, geometry);
                }
            }
        }

        public IFeatureClass CreateFeatureClass()
        {
            IFeatureClass result = null;
            IFieldsEdit fieldsEdit = new FieldsClass();
            fieldsEdit = CreateField(fieldsEdit, "ObjectId", esriFieldType.esriFieldTypeOID);

            IMappedFeature mapFeature = CreateMappedFeature(null);
            esriGeometryType esriGeometryType = mapFeature.EsriGeometryType;
            Dictionary<string, esriFieldType> fields = mapFeature.Fields;

            if ((esriGeometryType != esriGeometryType.esriGeometryNull) && (fields.Count >= 1))
            {
                string shapeFieldName = mapFeature.ShapeFieldName;
                ISpatialReference spatialReference = ArcUtils.SpatialReference;
                fieldsEdit = CreateGeometryField(fieldsEdit, shapeFieldName, spatialReference, esriGeometryType);
                fieldsEdit = fields.Aggregate(fieldsEdit, (current, field) => CreateField(current, field.Key, field.Value));

                foreach (var fieldName in FieldNames)
                {
                    if ((!string.IsNullOrEmpty(fieldName)) &&
                        (!fields.Aggregate(false, (current, field) => (field.Key == fieldName) || current)))
                    {
                        CreateField(fieldsEdit, fieldName, esriFieldType.esriFieldTypeString);
                    }
                }

                IFeatureWorkspace featureWorkspace = _cycloMediaGroupLayer.FeatureWorkspace;
                result = featureWorkspace.CreateFeatureClass(FcName, fieldsEdit, null, null, esriFeatureType.esriFTSimple,
                                                             shapeFieldName, string.Empty);
            }

            return result;
        }

        public void MakeEmpty()
        {
            MakeEmpty(FeatureClass);
        }

        public virtual void UpdateColor(Color color, int? year)
        {
            if (year == null)
            {
                Color = color;
                ArcUtils.SetColorToLayer(Layer, color);
                Refresh();
            }
        }

        public virtual DateTime? GetDate()
        {
            return null;
        }

        public virtual double GetHeight(double x, double y)
        {
            return 0.0;
        }

        #endregion functions (public)

        #region functions (protected)

        // =========================================================================
        // Functions (Protected)
        // =========================================================================
        protected virtual void Remove()
        {
            var avEvents = ArcUtils.ActiveViewEvents;
            IsRemoved = true;

            if (LayerRemoveEvent != null)
            {
                LayerRemoveEvent(this);
            }

            if (avEvents != null)
            {
                avEvents.ViewRefreshed -= OnViewRefreshed;
                avEvents.ContentsChanged -= OnContentChanged;
                avEvents.ItemDeleted -= OnItemDeleted;
                avEvents.AfterDraw -= OnAfterDraw;
            }
        }

        #endregion functions (protected)

        #region functions (private)

        // =========================================================================
        // Functions (Private)
        // =========================================================================
        private void CreateUniqueValueRenderer()
        {
            var markerSymbol = new SimpleMarkerSymbol
            {
                // ReSharper disable CSharpWarnings::CS0618
                Color = Converter.ToRGBColor(Color),
                Size = SizeLayer
                // ReSharper restore CSharpWarnings::CS0618
            };

            IUniqueValueRenderer renderer = new UniqueValueRendererClass
            {
                DefaultSymbol = markerSymbol as ISymbol,
                DefaultLabel = string.Empty,
                UseDefaultSymbol = this is WfsLayer,
                FieldCount = FieldNames.Length
            };

            for (int i = 0; i < FieldNames.Length; i++)
            {
                // ReSharper disable UseIndexedProperty
                renderer.set_Field(i, FieldNames[i]);
                // ReSharper restore UseIndexedProperty
            }

            var geoFeatureLayer = _layer as IGeoFeatureLayer;

            if (geoFeatureLayer != null)
            {
                geoFeatureLayer.Renderer = renderer as IFeatureRenderer;
            }
        }

        private IFeatureClass OpenFeatureClassInWorkspace()
        {
            IFeatureClass result = null;
            IFeatureWorkspace featureWorkspace = _cycloMediaGroupLayer.FeatureWorkspace;
            var workspace = featureWorkspace as IWorkspace2;
            // ReSharper disable UseIndexedProperty

            if (workspace != null)
            {
                result = workspace.get_NameExists(esriDatasetType.esriDTFeatureClass, FcName)
                           ? featureWorkspace.OpenFeatureClass(FcName)
                           : CreateFeatureClass();
            }

            // ReSharper restore UseIndexedProperty
            MakeEmpty(result);
            return result;
        }

        private ILayer GetLayer()
        {
            ILayer result = null;
            IMap map = ArcUtils.Map;

            if (map != null)
            {
                // ReSharper disable UseIndexedProperty
                var layers = map.get_Layers();
                bool leave = false;
                ILayer layer;

                while (((layer = layers.Next()) != null) && (!leave))
                {
                    if (layer.Name == Name)
                    {
                        if (layer is IFeatureLayer)
                        {
                            if (layer.Valid)
                            {
                                result = layer;
                                leave = true;
                            }
                            else
                            {
                                map.DeleteLayer(layer);
                            }
                        }
                        else
                        {
                            _cycloMediaGroupLayer.GroupLayer.Delete(layer);
                        }
                    }
                }

                // ReSharper restore UseIndexedProperty
            }

            return result;
        }

        private void MakeEmpty(IFeatureClass featureClass)
        {
            if (featureClass != null)
            {
                try
                {
                    IFeatureWorkspace featureWorkspace = _cycloMediaGroupLayer.FeatureWorkspace;
                    var workspaceEdit = featureWorkspace as IWorkspaceEdit;
                    var spatialCacheManager = featureWorkspace as ISpatialCacheManager3;

                    if (workspaceEdit != null)
                    {
                        workspaceEdit.StartEditing(false);

                        var existsResult = featureClass.Search(null, false);
                        try
                        {
                            IFeature feature;

                            while ((feature = existsResult.NextFeature()) != null)
                            {
                                feature.Delete();
                            }

                            workspaceEdit.StopEditing(true);

                            if (spatialCacheManager != null)
                            {
                                IActiveView activeView = ArcUtils.ActiveView;
                                IEnvelope envelope = activeView.Extent;
                                spatialCacheManager.FillCache(envelope);
                            }
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(existsResult);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message, "CycloMediaLayer.MakeEmpty");
                }
            }
        }

        /// <summary>
        /// Saves the spatial and attribute data to a geodatabase.
        /// </summary>
        /// <param name="featureMemberElements">The root node of the feature element list</param>
        /// <param name="envelope"></param>
        public void SaveFeatureMembers(IEnumerable<XElement> featureMemberElements, IEnvelope envelope)
        {
            // ReSharper disable PossibleMultipleEnumeration

            if ((featureMemberElements != null) && featureMemberElements.Any())
            {
                if (FeatureClass == null)
                {
                    IMappedFeature feature = CreateMappedFeature(null);
                    XName name = feature.Name;

                    foreach (var featureMemberElement in featureMemberElements)
                    {
                        IEnumerable<XElement> mappedFeatureElements = featureMemberElement.Descendants(name);

                        foreach (var mappedFeatureElement in mappedFeatureElements)
                        {
                            CreateMappedFeature(mappedFeatureElement);
                        }
                    }

                    if (FeatureClass != null)
                    {
                        var featureLayer = _layer as FeatureLayer;

                        if (featureLayer != null)
                        {
                            featureLayer.FeatureClass = FeatureClass;
                        }
                    }
                }

                IMappedFeature mapFeature = CreateMappedFeature(null);
                var fieldIds = mapFeature.Fields.ToDictionary(field => FeatureClass.FindField(field.Key), field => field.Key);
                string idField = mapFeature.ObjectId;
                XName objectName = mapFeature.Name;
                IFeatureWorkspace featureWorkspace = _cycloMediaGroupLayer.FeatureWorkspace;
                var workspaceEdit = featureWorkspace as IWorkspaceEdit;
                var spatialCacheManager = featureWorkspace as ISpatialCacheManager3;

                if (workspaceEdit != null)
                {
                    var exists = new Dictionary<string, IFeature>();

                    if (FeatureClass != null)
                    {
                        ISpatialFilter spatialFilter = new SpatialFilterClass
                        {
                            Geometry = envelope,
                            GeometryField = FeatureClass.ShapeFieldName,
                            SpatialRel = esriSpatialRelEnum.esriSpatialRelContains,
                            SubFields = idField
                        };

                        workspaceEdit.StartEditing(false);

                        try
                        {
                            var existsResult = FeatureClass.Search(spatialFilter, false);
                            try
                            {
                                var existsCount = FeatureClass.FeatureCount(spatialFilter);

                                var imId = existsResult.FindField(idField);
                                // ReSharper disable UseIndexedProperty

                                for (int i = 0; i < existsCount; i++)
                                {
                                    IFeature feature = existsResult.NextFeature();
                                    var recValue = feature.get_Value(imId) as string;

                                    if ((recValue != null) && (!exists.ContainsKey(recValue)))
                                    {
                                        exists.Add(recValue, feature);
                                    }
                                }
                            }
                            finally
                            {
                                Marshal.ReleaseComObject(existsResult);
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.Message, "CycloMediaLayer.SaveFeatureMembers");
                        }
                    }

                    if (FeatureClass != null)
                    {
                        foreach (XElement featureMemberElement in featureMemberElements)
                        {
                            IEnumerable<XElement> mappedFeatureElements = featureMemberElement.Descendants(objectName);

                            foreach (var mappedFeatureElement in mappedFeatureElements)
                            {
                                IMappedFeature mappedFeature = CreateMappedFeature(mappedFeatureElement);

                                if (mappedFeature.Shape != null)
                                {
                                    if (mappedFeature.Shape.Type == ShapeType.Point)
                                    {
                                        if (!exists.ContainsKey((string)mappedFeature.FieldToItem(idField)))
                                        {
                                            var shapePoint = mappedFeature.Shape as ShapePoint;

                                            if ((shapePoint != null) && Filter(mappedFeature))
                                            {
                                                IPoint point = new Point();
                                                point.PutCoords(shapePoint.X, shapePoint.Y);
                                                IFeature feature = FeatureClass.CreateFeature();
                                                feature.Shape = point;

                                                foreach (var fieldId in fieldIds)
                                                {
                                                    if (fieldId.Key < 0)
                                                        throw new ApplicationException($"Unable to set fields. Key '{fieldId.Key}' with value '{fieldId.Value}' is invalid.");
                                                    else
                                                        feature.set_Value(fieldId.Key, mappedFeature.FieldToItem(fieldId.Value));
                                                }

                                                feature.Store();
                                            }
                                        }
                                        else
                                        {
                                            if (Filter(mappedFeature))
                                            {
                                                exists.Remove((string)mappedFeature.FieldToItem(idField));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var feature in exists)
                    {
                        feature.Value.Delete();
                    }

                    workspaceEdit.StopEditing(true);

                    if ((spatialCacheManager != null) && (envelope != null))
                    {
                        spatialCacheManager.FillCache(envelope);
                    }
                    // ReSharper restore UseIndexedProperty
                }
            }

            // ReSharper restore PossibleMultipleEnumeration
        }

        #endregion functions (private)

        #region event handlers

        // =========================================================================
        // Event handlers
        // =========================================================================
        private void OnViewRefreshed(IActiveView view, esriViewDrawPhase phase, object data, IEnvelope envelope)
        {
            try
            {
                if (InsideScale)
                {
                    IActiveView activeView = ArcUtils.ActiveView;

                    if ((activeView != null) && (_layer != null) && (_cycloMediaGroupLayer != null))
                    {
                        const double epsilon = 0.0;
                        var extent = activeView.Extent;

                        if ((_addData != null) && (_addData.Count >= 1))
                        {
                            SaveFeatureMembers(_addData, _lastextent);
                            _addData.Clear();
                        }

                        PostEntryStep();

                        if (((Math.Abs(extent.XMax - _lastextent.XMax) > epsilon) ||
                             (Math.Abs(extent.YMin - _lastextent.YMin) > epsilon)
                             || (Math.Abs(extent.XMin - _lastextent.XMin) > epsilon) ||
                             (Math.Abs(extent.YMax - _lastextent.YMax) > epsilon)))
                        {
                            if ((_addData == null) || (_addData.Count == 0))
                            {
                                if ((_getDataThread == null) || (!_getDataThread.IsAlive))
                                {
                                    _lastextent = extent;
                                    IEnvelope getEnv = extent.Envelope;
                                    getEnv.Project(SpatialReference);
                                    _getDataThread = new Thread(GetDataWfs);
                                    _getDataThread.Start(getEnv);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (_addData != null)
                    {
                        _addData.Clear();
                    }

                    if (YearMonth.Count >= 1)
                    {
                        YearMonth.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "CycloMediaLayer.OnViewRefreshed");
            }
        }

        private void OnContentChanged()
        {
            try
            {
                Color outline;
                Color = ArcUtils.GetColorFromLayer(_layer, out outline);
                Outline = outline;

                LayerChangedEvent?.Invoke(this);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "CycloMediaLayer.OnContentChanged");
            }
        }

        private void OnItemDeleted(object item)
        {
            try
            {
                if (item != null)
                {
                    var featureLayer = item as ILayer;

                    if (featureLayer != null)
                    {
                        if (_layer == featureLayer)
                        {
                            _cycloMediaGroupLayer.Layers.Remove(this);
                            Remove();
                            _layer = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "CycloMediaLayer.OnItemDeleted");
            }
        }

        #endregion event handlers

        #region thread functions

        // =========================================================================
        // Thread functions
        // =========================================================================
        private void GetDataWfs(object context)
        {
            try
            {
                lock (_getDataLock)
                {
                    var extent = context as IEnvelope;

                    if (extent != null)
                    {
                        Web web = Web.Instance;
                        _addData = web.GetByBbox(extent, this);

                        if (_addData.Count >= 1)
                        {
                            _refreshDataThread = new Thread(RefreshDataWfs);
                            _refreshDataThread.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "CycloMediaLayer.GetDataWfs");
            }
        }

        private void RefreshDataWfs()
        {
            try
            {
                lock (_getDataLock)
                {
                    IActiveView activeView = ArcUtils.ActiveView;

                    if (activeView != null)
                    {
                        activeView.PartialRefresh(esriViewDrawPhase.esriViewForeground, Layer, null);
                    }
                }

                DrawLayer();
                //Viewer.Redraw();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "CycloMediaLayer.RefreshDataWfs");
            }
        }

        private void DrawLayer()
        {
            lock (_getDataLock)
            {
                IActiveView view = ArcUtils.ActiveView;
                IScreenDisplay display = view.ScreenDisplay;
                display.StartDrawing(display.hDC, (short) esriScreenCache.esriNoScreenCache);
                ITrackCancel cancel = new TrackCancelClass();
                Layer.Draw(esriDrawPhase.esriDPGeography, display, cancel);
                display.FinishDrawing();
            }
        }

        #endregion thread functions

        #region functions (static)

        // =========================================================================
        // Functions (Static)
        // =========================================================================
        protected static void ChangeHistoricalDate()
        {
            if (HistoricalDateChanged != null)
            {
                HistoricalDateChanged(YearMonth);
            }
        }

        public static void ResetYears()
        {
            YearMonth.Clear();
        }

        private static IFieldsEdit CreateField(IFieldsEdit fieldsEdit, string fieldName, esriFieldType esriFieldType)
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            IFieldEdit fieldEdit = new FieldClass();
            fieldEdit.Type_2 = esriFieldType;
            fieldEdit.Name_2 = fieldName;
            fieldEdit.AliasName_2 = fieldName;
            fieldsEdit.AddField(fieldEdit);
            return fieldsEdit;
            // ReSharper restore UseObjectOrCollectionInitializer
        }

        private static IFieldsEdit CreateGeometryField(IFieldsEdit fieldsEdit, string fieldName,
                                                       ISpatialReference spatialReference, esriGeometryType esriGeometryType)
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            IGeometryDefEdit geometryDefEdit = new GeometryDefClass();
            geometryDefEdit.GeometryType_2 = esriGeometryType;
            geometryDefEdit.SpatialReference_2 = spatialReference;

            IFieldEdit fieldEdit = new FieldClass();
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.Name_2 = fieldName;
            fieldEdit.AliasName_2 = fieldName;
            fieldEdit.GeometryDef_2 = geometryDefEdit;
            fieldsEdit.AddField(fieldEdit);
            return fieldsEdit;
            // ReSharper restore UseObjectOrCollectionInitializer
        }

        #endregion functions (static)
    }
}