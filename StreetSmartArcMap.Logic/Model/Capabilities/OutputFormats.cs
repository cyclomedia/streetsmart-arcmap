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

using System.Xml.Linq;

namespace StreetSmartArcMap.Logic.Model.Capabilities
{
    public class OutputFormats
    {
        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public string Format { get; set; }

        #endregion properties

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        /// <summary>
        /// Default empty constructor
        /// </summary>
        public OutputFormats()
        {
            // empty
        }

        /// <summary>
        /// Constructor with xml parsing
        /// </summary>
        /// <param name="element">xml</param>
        public OutputFormats(XElement element)
        {
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
            XElement outputformatElement = element.Element(Namespaces.WfsNs + "OutputFormats");

            if (outputformatElement != null)
            {
                XElement formatElement = outputformatElement.Element(Namespaces.WfsNs + "Format");

                if (formatElement != null)
                {
                    Format = formatElement.Value;
                }
            }
        }

        #endregion functions
    }
}