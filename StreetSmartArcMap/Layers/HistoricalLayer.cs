/*
 * Integration in ArcMap for Cycloramas
 * Copyright (c) 2019 - 2020, CycloMedia, All rights reserved.
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
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using StreetSmartArcMap.Logic.Model;
using StreetSmartArcMap.Logic.Model.Atlas;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace StreetSmartArcMap.Layers
{
    public class HistoricalLayer : CycloMediaLayer
    {
        #region members

        private static Color _color;
        private static Color _outline;

        private static double _minimumScale;
        private static SortedDictionary<int, Color> _yearToColor;
        private static List<int> _yearPip;
        private static List<int> _yearForbidden;

        public override string Name => "Historical Recordings";
        public override string FcName => "FCHistoricalRecordings";

        #endregion members

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        private static SortedDictionary<int, Color> YearToColor => _yearToColor ?? (_yearToColor = new SortedDictionary<int, Color>());

        private static List<int> YearPip => _yearPip ?? (_yearPip = new List<int>());

        private static List<int> YearForbidden => _yearForbidden ?? (_yearForbidden = new List<int>());

        public override Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public override Color Outline
        {
            get { return _outline; }
            set { _outline = value; }
        }

        public override double MinimumScale
        {
            get { return _minimumScale; }
            set { _minimumScale = value; }
        }

        public override string[] FieldNames
        {
            get { return new[] { "Year" }; } //, "PIP", "IsAuthorized"
        }

        public override int SizeLayer => 7;
        public override bool UseDateRange => true;

        public int AddedYear { get; private set; }

        public override string WfsRequest =>
            "<wfs:GetFeature service=\"WFS\" version=\"1.1.0\" resultType=\"results\" outputFormat=\"text/xml; subtype=gml/3.1.1\" " +
            "xmlns:wfs=\"http://www.opengis.net/wfs\"><wfs:Query typeName=\"atlas:Recording\" srsName=\"{0}\" " +
            "xmlns:atlas=\"http://www.cyclomedia.com/atlas\"><ogc:Filter xmlns:ogc=\"http://www.opengis.net/ogc\"><ogc:And> " +
            "<ogc:BBOX><gml:Envelope srsName=\"{0}\" xmlns:gml=\"http://www.opengis.net/gml\"><gml:lowerCorner>{1} {2}</gml:lowerCorner> " +
            "<gml:upperCorner>{3} {4}</gml:upperCorner></gml:Envelope></ogc:BBOX><ogc:PropertyIsEqualTo>" +
            "<ogc:PropertyName>year</ogc:PropertyName><ogc:Literal>{5}</ogc:Literal></ogc:PropertyIsEqualTo><ogc:PropertyIsBetween>" +
            "<ogc:PropertyName>expiredAt</ogc:PropertyName><ogc:LowerBoundary><ogc:Literal>1995-01-01</ogc:Literal></ogc:LowerBoundary>" +
            "<ogc:UpperBoundary><ogc:Literal>2100-01-01</ogc:Literal></ogc:UpperBoundary></ogc:PropertyIsBetween></ogc:And>" +
            "</ogc:Filter></wfs:Query></wfs:GetFeature>";

        #endregion properties

        #region functions (protected)

        protected override IMappedFeature CreateMappedFeature(XElement mappedFeatureElement)
        {
            return new Recording(mappedFeatureElement);
        }

        protected override bool Filter(IMappedFeature mappedFeature)
        {
            var recording = mappedFeature as Recording;
            bool result = (recording != null);

            if (result)
            {
                DateTime? recordedAt = recording.RecordedAt;
                result = (recordedAt != null);

                if (result)
                {
                    var dateTime = (DateTime)recordedAt;
                    int year = dateTime.Year;
                    int month = dateTime.Month;

                    if (!YearMonth.ContainsKey(year))
                    {
                        YearMonth.Add(year, month);
                        ChangeHistoricalDate();
                    }
                    else
                    {
                        YearMonth[year] = month;
                    }
                }
            }

            return result;
        }

        protected override void PostEntryStep()
        {
            if (Year != 0 && Year != AddedYear)
            {
                var geoFeatureLayer = Layer as IGeoFeatureLayer;
                var featureRenderer = geoFeatureLayer?.Renderer;
                var uniqueValueRenderer = featureRenderer as IUniqueValueRenderer;

                var historyMarker = new SimpleMarkerSymbol()
                {
                    Color = Converter.ToRGBColor(Color.FromArgb(170, 236, 122, 8)),
                    Size = SizeLayer
                };

                var className = $"{Year}";
                uniqueValueRenderer?.AddValue(className, string.Empty, historyMarker as ISymbol);

                if (uniqueValueRenderer != null)
                {
                    uniqueValueRenderer.Label[className] = className;
                }

                if (AddedYear != 0)
                {
                    uniqueValueRenderer?.RemoveValue($"{AddedYear}");
                }

                IActiveView activeView = ArcUtils.ActiveView;
                activeView.ContentsChanged();
                AddedYear = Year;
            }

            if (Year == 0 && !IsRemoved && InitComplete)
            {
                RemoveFromGroup();
                TurnRecordingLayerOn();
            }
        }

        protected override void Remove()
        {
            AddedYear = 0;
            base.Remove();
            YearToColor.Clear();
            YearPip.Clear();
            YearForbidden.Clear();
        }

        public override DateTime? GetDate()
        {
            DateTime? result = null;
            const string objectId = "RecordedAt";
            IActiveView activeView = ArcUtils.ActiveView;

            ISpatialFilter spatialFilter = new SpatialFilter
            {
                Geometry = activeView.Extent,
                GeometryField = FeatureClass.ShapeFieldName,
                SpatialRel = esriSpatialRelEnum.esriSpatialRelContains,
                SubFields = objectId
            };

            var existsResult = FeatureClass.Search(spatialFilter, false);

            try
            {
                IFeature feature = existsResult.NextFeature();

                if (feature != null)
                {
                    // ReSharper disable UseIndexedProperty
                    int imId = existsResult.FindField(objectId);
                    object value = feature.get_Value(imId);
                    result = (DateTime) value;
                    // ReSharper restore UseIndexedProperty
                }
            }
            finally
            {
                Marshal.ReleaseComObject(existsResult);
            }

            return result;
        }

        public override double GetHeight(double x, double y)
        {
            double result = 0.0;
            IActiveView activeView = ArcUtils.ActiveView;

            if (activeView != null)
            {
                const string height = "Height";
                const string groundLevelOffset = "GroundLevelOffset";

                const double searchBox = 25.0;
                double xMin = x - searchBox;
                double xMax = x + searchBox;
                double yMin = y - searchBox;
                double yMax = y + searchBox;
                IEnvelope envelope = (IEnvelope) new Envelope {XMin = xMin, XMax = xMax, YMin = yMin, YMax = yMax};

                ISpatialFilter spatialFilter = new SpatialFilter
                {
                    Geometry = envelope,
                    GeometryField = FeatureClass.ShapeFieldName,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelContains,
                    SubFields = $"{height},{groundLevelOffset}"
                };

                var existsResult = FeatureClass.Search(spatialFilter, false);

                try
                {
                    IFeature feature;
                    int count = 0;

                    // ReSharper disable UseIndexedProperty
                    while ((feature = existsResult.NextFeature()) != null)
                    {
                        int heightId = existsResult.FindField(height);
                        int groundLevelOffsetId = existsResult.FindField(groundLevelOffset);
                        var heightValue = (double) feature.get_Value(heightId);
                        var groundLevelOffsetValue = (double) feature.get_Value(groundLevelOffsetId);
                        result = result + heightValue - groundLevelOffsetValue;
                        count++;
                    }

                    result = result / Math.Max(count, 1);
                }
                finally
                {
                    Marshal.ReleaseComObject(existsResult);
                }
                // ReSharper restore UseIndexedProperty
            }

            return result;
        }

        #endregion functions (protected)

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        static HistoricalLayer()
        {
            _color = Color.Transparent;
            _minimumScale = 2000.0;
        }

        public HistoricalLayer(CycloMediaGroupLayer layer) : base(layer)
        {
            AddedYear = 0;
        }

        #endregion constructor
    }
}