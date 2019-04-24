/*
 * Integration in ArcMap for Cycloramas
 * Copyright (c) 2014, CycloMedia, All rights reserved.
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

namespace StreetSmart.Utilities
{
  class Urls
  {
    /// <summary>
    /// This file contains default URLs
    /// </summary>

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    /// <summary>
    /// The base url
    /// </summary>
    public static string BaseUrl = "https://atlas.cyclomedia.com";

    /// <summary>
    /// The recording service url
    /// </summary>
    public static string RecordingsServiceUrl = "https://atlas.cyclomedia.com/recordings/wfs";

    /// <summary>
    /// The Api URL
    /// </summary>
    public static string ApiUrl = "https://streetsmart.cyclomedia.com/api/v18.10";

    /// <summary>
    /// The spatialreferences url
    /// </summary>
    public static string SpatialReferencesUrl = "/assets/srs/SpatialReference.xml";

    #endregion
  }
}
