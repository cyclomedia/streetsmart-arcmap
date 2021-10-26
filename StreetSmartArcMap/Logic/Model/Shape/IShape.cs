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

namespace StreetSmartArcMap.Logic.Model.Shape
{
    /// <summary>
    /// GSML2 shape elmenent interface definition.
    /// </summary>
    public interface IShape
    {
        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        string Id { get; set; }

        string SrsName { get; set; }
        ShapeType Type { get; set; }

        #endregion properties
    }
}