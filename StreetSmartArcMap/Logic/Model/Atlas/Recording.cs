﻿/*
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

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using StreetSmartArcMap.Logic.Model.Shape;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

using Point = StreetSmartArcMap.Logic.Model.Shape.Point;

namespace StreetSmartArcMap.Logic.Model.Atlas
{
    public class Recording : IMappedFeature
    {
        #region members

        // =========================================================================
        // Members
        // =========================================================================
        private readonly CultureInfo _ci;

        #endregion members

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public Dictionary<string, esriFieldType> Fields
        {
            get
            {
                return new Dictionary<string, esriFieldType>
                 {
                   {"Id", esriFieldType.esriFieldTypeString},
                   {"ImageId", esriFieldType.esriFieldTypeString},
                   {"RecordedAt", esriFieldType.esriFieldTypeDate},
                   {"Height", esriFieldType.esriFieldTypeDouble},
                   {"LatitudePrecision", esriFieldType.esriFieldTypeDouble},
                   {"LongitudePrecision", esriFieldType.esriFieldTypeDouble},
                   {"HeightPrecision", esriFieldType.esriFieldTypeDouble},
                   {"Orientation", esriFieldType.esriFieldTypeDouble},
                   {"OrientationPrecision", esriFieldType.esriFieldTypeDouble},
                   {"GroundLevelOffset", esriFieldType.esriFieldTypeDouble},
                   {"RecorderDirection", esriFieldType.esriFieldTypeDouble},
                   {"ProductType", esriFieldType.esriFieldTypeString},
                   {"IsAuthorized", esriFieldType.esriFieldTypeString},
                   {"ExpiredAt", esriFieldType.esriFieldTypeDate},
                   {"Year", esriFieldType.esriFieldTypeInteger},
                   {"PIP", esriFieldType.esriFieldTypeString},
                   {"PIP1Yaw", esriFieldType.esriFieldTypeDouble},
                   {"PIP2Yaw", esriFieldType.esriFieldTypeDouble},
                   {"HasDepthMap", esriFieldType.esriFieldTypeString}
                 };
            }
        }

        public string ObjectId
        {
            get { return "ImageId"; }
        }

        public XName Name
        {
            get { return (Namespaces.AtlasNs + "Recording"); }
        }

        public string ShapeFieldName
        {
            get { return "Location"; }
        }

        public esriGeometryType EsriGeometryType
        {
            get { return esriGeometryType.esriGeometryPoint; }
        }

        public string Id { get; private set; }
        public string ImageId { get; private set; }
        public DateTime? RecordedAt { get; private set; }
        public IShape Shape { get; private set; }
        public Height Height { get; private set; }
        public double? LatitudePrecision { get; private set; }
        public double? LongitudePrecision { get; private set; }
        public double? HeightPrecision { get; private set; }
        public double? Orientation { get; private set; }
        public double? OrientationPrecision { get; private set; }
        public double? GroundLevelOffset { get; private set; }
        public double? RecorderDirection { get; private set; }
        public ProductType ProductType { get; private set; }
        public Images Images { get; private set; }
        public bool? IsAuthorized { get; private set; }
        public DateTime? ExpiredAt { get; private set; }
        public bool? HasDepthMap { get; set; }
        public int? Year { get; set; }

        #endregion properties

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        /// <summary>
        /// Default empty constructor
        /// </summary>
        public Recording()
        {
            _ci = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Constructor with xml parsing
        /// </summary>
        /// <param name="mappedFeatureElement">xml</param>
        public Recording(XElement mappedFeatureElement)
        {
            _ci = CultureInfo.InvariantCulture;
            Update(mappedFeatureElement);
        }

        #endregion constructor

        #region functions

        // =========================================================================
        // Functions
        // =========================================================================
        /// <summary>
        /// xml parsing
        /// </summary>
        /// <param name="mappedFeatureElement">xml</param>
        public void Update(XElement mappedFeatureElement)
        {
            if (mappedFeatureElement != null)
            {
                XAttribute mappedFeatureAttribute = mappedFeatureElement.Attribute(Namespaces.GmlNs + "id");
                XElement imageIdElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "imageId");
                XElement recordedAtElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "recordedAt");
                XElement locationElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "location");
                XElement heightElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "height");
                XElement latPrecElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "latitudePrecision");
                XElement lonPrecElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "longitudePrecision");
                XElement heigPrecElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "heightPrecision");
                XElement orientElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "orientation");
                XElement orPrecElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "orientationPrecision");
                XElement grLevOffElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "groundLevelOffset");
                XElement recDirElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "recorderDirection");
                XElement prodTypeElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "productType");
                XElement imagesElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "Images");
                XElement isAuthorizedElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "isAuthorized");
                XElement expiredAtElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "expiredAt");
                XElement hasDepthMapElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "hasDepthMap");
                XElement hasYearElement = mappedFeatureElement.Element(Namespaces.AtlasNs + "year");

                ExpiredAt = (expiredAtElement == null) ? (DateTime?)null : DateTime.Parse(expiredAtElement.Value.Trim());
                Id = (mappedFeatureAttribute == null) ? null : mappedFeatureAttribute.Value.Trim();
                ImageId = (imageIdElement == null) ? null : imageIdElement.Value.Trim();
                RecordedAt = (recordedAtElement == null) ? (DateTime?)null : DateTimeOffset.Parse(recordedAtElement.Value.Trim(), null).DateTime;
                Height = (heightElement == null) ? null : new Height(heightElement);
                LatitudePrecision = (latPrecElement == null) ? (double?)null : double.Parse(latPrecElement.Value.Trim(), _ci);
                LongitudePrecision = (lonPrecElement == null) ? (double?)null : double.Parse(lonPrecElement.Value.Trim(), _ci);
                HeightPrecision = (heigPrecElement == null) ? (double?)null : double.Parse(heigPrecElement.Value.Trim(), _ci);
                Orientation = (orientElement == null) ? (double?)null : double.Parse(orientElement.Value.Trim(), _ci);
                OrientationPrecision = (orPrecElement == null) ? (double?)null : double.Parse(orPrecElement.Value.Trim(), _ci);
                GroundLevelOffset = (grLevOffElement == null) ? (double?)null : double.Parse(grLevOffElement.Value.Trim(), _ci);
                RecorderDirection = (recDirElement == null) ? (double?)null : double.Parse(recDirElement.Value.Trim(), _ci);
                Images = (imagesElement == null) ? new Images() : new Images(imagesElement);
                IsAuthorized = (isAuthorizedElement == null) ? (bool?)null : bool.Parse(isAuthorizedElement.Value.Trim());
                HasDepthMap = (hasDepthMapElement == null) ? (bool?)null : bool.Parse(hasDepthMapElement.Value.Trim());
                Year = (hasYearElement == null) ? (int?) null : int.Parse(hasYearElement.Value.Trim());

                ProductType = (prodTypeElement == null)
                                ? ProductType.None
                                : (ProductType)Enum.Parse(typeof(ProductType), prodTypeElement.Value.Trim());

                if (locationElement != null)
                {
                    if (locationElement.Element(Namespaces.GmlNs + "Point") != null)
                    {
                        Shape = new Point(locationElement);
                    }
                }
            }
        }

        /// <summary>
        /// This function returns the value of a field.
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <returns>The value</returns>
        public object FieldToItem(string name)
        {
            object result = null;

            switch (name)
            {
                case "Id":
                    result = Id;
                    break;

                case "ImageId":
                    result = ImageId;
                    break;

                case "RecordedAt":
                    result = RecordedAt;
                    break;

                case "Height":
                    result = (Height == null) ? (double?)null : Height.Value;
                    break;

                case "LatitudePrecision":
                    result = LatitudePrecision;
                    break;

                case "LongitudePrecision":
                    result = LongitudePrecision;
                    break;

                case "HeightPrecision":
                    result = HeightPrecision;
                    break;

                case "Orientation":
                    result = Orientation;
                    break;

                case "OrientationPrecision":
                    result = OrientationPrecision;
                    break;

                case "GroundLevelOffset":
                    result = GroundLevelOffset;
                    break;

                case "RecorderDirection":
                    result = RecorderDirection;
                    break;

                case "ProductType":
                    result = ProductType;
                    break;

                case "IsAuthorized":
                    result = IsAuthorized.ToString();
                    break;

                case "ExpiredAt":
                    result = ExpiredAt;
                    break;

                case "Year":
                    result = Year;

                    if (Year == null && RecordedAt != null)
                    {
                        var thisDateTime = (DateTime)RecordedAt;
                        result = thisDateTime.Year;
                    }
                    break;

                case "PIP":
                    result = (Images.Count >= 2).ToString();
                    break;

                case "PIP1Yaw":
                    result = (Images.Count >= 1) ? Images[0].Yaw : null;
                    break;

                case "PIP2Yaw":
                    result = (Images.Count >= 2) ? Images[1].Yaw : null;
                    break;

                case "HasDepthMap":
                    result = HasDepthMap?.ToString() ?? "False";
                    break;
            }

            return result;
        }

        /// <summary>
        /// This function updates the value of a field.
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="item">The item of the field</param>
        public void UpdateItem(string name, object item)
        {
            if (item != null)
            {
                switch (name)
                {
                    case "Id":
                        Id = (string)item;
                        break;

                    case "ImageId":
                        ImageId = (string)item;
                        break;

                    case "RecordedAt":
                        RecordedAt = (DateTime?)item;
                        break;

                    case "Height":
                        Height = new Height { Value = (double)item };
                        break;

                    case "LatitudePrecision":
                        LatitudePrecision = (double?)item;
                        break;

                    case "LongitudePrecision":
                        LongitudePrecision = (double?)item;
                        break;

                    case "HeightPrecision":
                        HeightPrecision = (double?)item;
                        break;

                    case "Orientation":
                        Orientation = (double?)item;
                        break;

                    case "OrientationPrecision":
                        OrientationPrecision = (double?)item;
                        break;

                    case "GroundLevelOffset":
                        GroundLevelOffset = (double?)item;
                        break;

                    case "RecorderDirection":
                        RecorderDirection = (double?)item;
                        break;

                    case "ProductType":
                        ProductType = (ProductType)Enum.Parse(typeof(ProductType), (string)item);
                        break;

                    case "IsAuthorized":
                        IsAuthorized = bool.Parse((string)item);
                        break;

                    case "ExpiredAt":
                        ExpiredAt = (DateTime?)item;
                        break;

                    case "Year":
                        Year = (int?) item;
                        break;

                    case "PIP":
                        // empty
                        break;

                    case "PIP1Yaw":
                        // empty
                        break;

                    case "PIP2Yaw":
                        // empty
                        break;

                    case "HasDepthMap":
                        HasDepthMap = bool.Parse((string)item);
                        break;

                    case "Location":
                        var point = item as IPoint;

                        if (point != null)
                        {
                            Shape = new Point
                            {
                                X = point.X,
                                Y = point.Y,
                                Z = point.Z,
                                Type = ShapeType.Point,
                                SrsName = (point.SpatialReference == null) ? string.Empty : point.SpatialReference.Name
                            };
                        }
                        break;
                }
            }
        }

        #endregion functions
    }
}