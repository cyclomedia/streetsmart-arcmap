/*
 * Integration in ArcMap for StreetSmart
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

namespace StreetSmartArcMap.Client
{
    internal class Urls
    {
        /// <summary>
        /// This file contains default URLs
        /// </summary>

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        /// <summary>
        /// The base url, used for Configuration and Recordings
        /// </summary>
        public static string BaseUrl = "https://atlas.cyclomedia.com";
        /// <summary>
        /// The API Base URL, used for SpatialReferences and the API
        /// </summary>
        public static string ApiBaseUrl = "https://streetsmart.cyclomedia.com/api/v18.10";
        /// <summary>
        /// The spatialreferences url
        /// </summary>
        public static string SpatialReferencesUrl = "/assets/srs/SpatialReference.xml";
        /// <summary>
        /// The API Url
        /// </summary>
        public static string ApiUrl = "/api-dotnet.html";
        /// <summary>
        /// The configuration url
        /// </summary>
        public static string ConfigurationUrl = "/configuration";
        /// <summary>
        /// The recording service url
        /// </summary>
        public static string RecordingsServiceUrl = "/recordings/wfs";
        #endregion properties
    }
}