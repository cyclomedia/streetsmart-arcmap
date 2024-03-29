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

using System.Globalization;
using System.Xml.Linq;

namespace StreetSmartArcMap.Logic.Model.Capabilities
{
    public class Point
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
        public double X { get; set; }

        public double Y { get; set; }

        #endregion properties

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        /// <summary>
        /// Default empty constructor
        /// </summary>
        public Point()
        {
            _ci = CultureInfo.InvariantCulture;
            X = 0.0;
            Y = 0.0;
        }

        /// <summary>
        /// Constructor with xml parsing
        /// </summary>
        /// <param name="element">xml</param>
        public Point(XElement element)
        {
            _ci = CultureInfo.InvariantCulture;
            Update(element);
        }

        #endregion constructor

        #region functions

        // =========================================================================
        // Functions
        // =========================================================================
        /// <summary>
        /// xml parsing
        /// </summary>
        /// <param name="element">xml</param>
        public void Update(XElement element)
        {
            if (element != null)
            {
                string position = element.Value.Trim();
                string[] values = position.Split(' ');
                X = (values.Length >= 1) ? double.Parse(values[0], _ci) : 0.0;
                Y = (values.Length >= 2) ? double.Parse(values[1], _ci) : 0.0;
            }
        }

        #endregion functions
    }
}