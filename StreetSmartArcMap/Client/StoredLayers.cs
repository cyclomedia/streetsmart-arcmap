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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using StreetSmartArcMap.Utilities;
using StreetSmartArcMap.Logic.Utilities;

namespace StreetSmartArcMap.Client
{
    [XmlRoot("StoredLayers")]
    public class StoredLayers : List<StoredLayer>
    {
        public const bool DefaultVisibility = false;

        #region members

        // =========================================================================
        // Members
        // =========================================================================
        private static readonly XmlSerializer XmlStoredLayers;

        private static StoredLayers _storedLayers;

        #endregion

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        static StoredLayers()
        {
            XmlStoredLayers = new XmlSerializer(typeof(StoredLayers));
        }

        #endregion

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public StoredLayer[] StoredLayer
        {
            get { return ToArray(); }
            set
            {
                if (value != null)
                {
                    AddRange(value);
                }
            }
        }

        public static StoredLayers Instance
        {
            get
            {
                if ((_storedLayers == null) || (_storedLayers.Count == 0))
                {
                    try
                    {
                        Load();
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }
                }

                return _storedLayers ?? (_storedLayers = Create());
            }
        }

        public static string FileName
        {
            get { return Path.Combine(FileUtils.FileDir, "StreetSmartStoredLayers.xml"); }
        }

        #endregion

        #region functions (public)

        // =========================================================================
        // Functions (Public)
        // =========================================================================
        public void Save()
        {
            using (FileStream output = File.Open(FileName, FileMode.Create))
            {
                XmlStoredLayers.Serialize(output, this);
            }
        }

        public StoredLayer GetLayer(string name)
        {
            return this.Aggregate<StoredLayer, StoredLayer>
              (null, (current, storedLayer) => (storedLayer.Name == name) ? storedLayer : current);
        }

        public bool Get(string name)
        {
            StoredLayer storedLayer = GetLayer(name);

            if (storedLayer != null)
            {
                return storedLayer.Visible;
            }
            else
            {
                Update(name, DefaultVisibility);

                return DefaultVisibility;
            }
        }

        public void Update(string name, bool visible)
        {
            if (name != null)
            {
                StoredLayer storedLayer = GetLayer(name);

                if (storedLayer == null)
                {
                    storedLayer = new StoredLayer { Name = name, Visible = visible };
                    Add(storedLayer);
                }
                else
                {
                    storedLayer.Visible = visible;
                }

                Save();
            }
        }

        #endregion

        #region functions (static)

        // =========================================================================
        // Functions (Static)
        // =========================================================================
        public static StoredLayers Load()
        {
            if (File.Exists(FileName))
            {
                using (FileStream input = new FileStream(FileName, FileMode.OpenOrCreate))
                {
                    _storedLayers = (StoredLayers)XmlStoredLayers.Deserialize(input);
                }
            }

            return _storedLayers;
        }

        #endregion

        #region functions (private static)

        // =========================================================================
        // Functions (Private static)
        // =========================================================================
        private static StoredLayers Create()
        {
            var result = new StoredLayers();
            result.Save();
            return result;
        }

        #endregion
    }
}
