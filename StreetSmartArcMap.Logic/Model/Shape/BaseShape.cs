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

using System.Xml.Linq;

namespace StreetSmartArcMap.Logic.Model.Shape
{
  /// <summary>
  /// Base class for GSML2 shapes (points, lines, polygons)
  /// </summary>
  public class BaseShape : IShape
  {
    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    /// <summary>
    /// The shape id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The spatial reference name
    /// </summary>
    public string SrsName { get; set; }

    /// <summary>
    /// The shape type
    /// </summary>
    public ShapeType Type { get; set; }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    /// <summary>
    /// Default empty constructor
    /// </summary>
    public void Update()
    {
      // empty
    }

    /// <summary>
    /// Constructor with xml parsing
    /// </summary>
    /// <param name="mappedFeatureElement">xml</param>
    public void Update(XElement mappedFeatureElement)
    {
      // empty
    }

    #endregion

    #region functions

    // =========================================================================
    // Functions
    // =========================================================================
    /// <summary>
    /// A template method used to save the entity
    /// </summary>
    public void Save()
    {
      // empty
    }

    #endregion
  }
}
