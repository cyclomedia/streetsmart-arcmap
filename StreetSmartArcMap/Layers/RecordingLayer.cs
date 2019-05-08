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
using System.Xml.Linq;

namespace StreetSmartArcMap.Layers
{
    public class RecordingLayer : CycloMediaLayer
    {
        #region members

        // =========================================================================
        // Members
        // =========================================================================
        private static Color _color;

        private static double _minimumScale;
        private static SortedDictionary<int, Color> _yearToColor;
        private static List<int> _yearPip;
        private static List<int> _yearForbidden;

        public override string Name { get { return "Recent Recordings"; } }
        public override string FcName { get { return "FCRecentRecordings"; } }

        #endregion members

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        private static SortedDictionary<int, Color> YearToColor
        {
            get { return _yearToColor ?? (_yearToColor = new SortedDictionary<int, Color>()); }
        }

        private static List<int> YearPip
        {
            get { return _yearPip ?? (_yearPip = new List<int>()); }
        }

        private static List<int> YearForbidden
        {
            get { return _yearForbidden ?? (_yearForbidden = new List<int>()); }
        }

        public override Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public override double MinimumScale
        {
            get { return _minimumScale; }
            set { _minimumScale = value; }
        }

        public override string[] FieldNames
        {
            get { return new[] { "HasDepthMap" }; } //, "PIP", "IsAuthorized"
        }

        public override int SizeLayer { get { return 7; } }
        public override bool UseDateRange { get { return false; } }

        public override string WfsRequest
        {
            get
            {
                return
                  "<wfs:GetFeature service=\"WFS\" version=\"1.1.0\" resultType=\"results\" outputFormat=\"text/xml; subtype=gml/3.1.1\" xmlns:wfs=\"http://www.opengis.net/wfs\">" +
                  "<wfs:Query typeName=\"atlas:Recording\" srsName=\"{0}\" xmlns:atlas=\"http://www.cyclomedia.com/atlas\"><ogc:Filter xmlns:ogc=\"http://www.opengis.net/ogc\">" +
                  "<ogc:And><ogc:BBOX><gml:Envelope srsName=\"{0}\" xmlns:gml=\"http://www.opengis.net/gml\"><gml:lowerCorner>{1} {2}</gml:lowerCorner>" +
                  "<gml:upperCorner>{3} {4}</gml:upperCorner></gml:Envelope></ogc:BBOX><ogc:PropertyIsNull><ogc:PropertyName>expiredAt</ogc:PropertyName></ogc:PropertyIsNull>" +
                  "</ogc:And></ogc:Filter></wfs:Query></wfs:GetFeature>";
            }
        }

        #endregion properties

        #region functions (protected)

        // =========================================================================
        // Functions (Protected)
        // =========================================================================
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
            IActiveView activeView = ArcUtils.ActiveView;
            IEnvelope envelope = activeView.Extent;

            var geoFeatureLayer = Layer as IGeoFeatureLayer;

            if (geoFeatureLayer != null)
            {
                var featureRenderer = geoFeatureLayer.Renderer;
                var uniqueValueRenderer = featureRenderer as IUniqueValueRenderer;

                if (uniqueValueRenderer != null)
                {
                    var hasDepthMarker = new SimpleMarkerSymbol()
                    {
                        Color = Converter.ToRGBColor(Color.FromArgb(170, 152, 194, 60)),
                        Size = SizeLayer
                    };
                    var className = string.Format("{0}", true);
                    uniqueValueRenderer.AddValue(className, string.Empty, hasDepthMarker as ISymbol);

                    uniqueValueRenderer.Label[className] = Properties.Resources.WithDepthMap;
                    var hasNoDepthMarker = new SimpleMarkerSymbol()
                    {
                        Color = Converter.ToRGBColor(Color.FromArgb(170, 128, 176, 255)),
                        Size = SizeLayer
                    };

                    className = string.Format("{0}", false);
                    uniqueValueRenderer.AddValue(className, string.Empty, hasNoDepthMarker as ISymbol);

                    uniqueValueRenderer.Label[className] = Properties.Resources.WithoutDepthMap;
                    activeView.ContentsChanged();
                }
            }
        }

        protected override void Remove()
        {
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
            IEnvelope envelope = activeView.Extent;

            ISpatialFilter spatialFilter = new SpatialFilter()
            {
                Geometry = envelope,
                GeometryField = FeatureClass.ShapeFieldName,
                SpatialRel = esriSpatialRelEnum.esriSpatialRelContains,
                SubFields = objectId
            };

            var existsResult = FeatureClass.Search(spatialFilter, false);
            IFeature feature = existsResult.NextFeature();

            if (feature != null)
            {
                // ReSharper disable UseIndexedProperty
                int imId = existsResult.FindField(objectId);
                object value = feature.get_Value(imId);
                result = (DateTime)value;
                // ReSharper restore UseIndexedProperty
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
                IEnvelope envelope = (IEnvelope)new Envelope() { XMin = xMin, XMax = xMax, YMin = yMin, YMax = yMax };

                ISpatialFilter spatialFilter = new SpatialFilter
                {
                    Geometry = envelope,
                    GeometryField = FeatureClass.ShapeFieldName,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelContains,
                    SubFields = string.Format("{0},{1}", height, groundLevelOffset)
                };

                var existsResult = FeatureClass.Search(spatialFilter, false);
                IFeature feature;
                int count = 0;

                // ReSharper disable UseIndexedProperty
                while ((feature = existsResult.NextFeature()) != null)
                {
                    int heightId = existsResult.FindField(height);
                    int groundLevelOffsetId = existsResult.FindField(groundLevelOffset);
                    var heightValue = (double)feature.get_Value(heightId);
                    var groundLevelOffsetValue = (double)feature.get_Value(groundLevelOffsetId);
                    result = result + heightValue - groundLevelOffsetValue;
                    count++;
                }

                result = result / Math.Max(count, 1);
                // ReSharper restore UseIndexedProperty
            }

            return result;
        }

        #endregion functions (protected)

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        static RecordingLayer()
        {
            _color = Color.Transparent;
            _minimumScale = 2000.0;
        }

        public RecordingLayer(CycloMediaGroupLayer layer)
          : base(layer)
        {
            // empty
        }

        #endregion constructor
    }
}