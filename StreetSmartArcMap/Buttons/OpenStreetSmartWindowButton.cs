/*
 * Integration in ArcMap for StreetSmart
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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Logic.Configuration;
using ESRI.ArcGIS.Carto;
using StreetSmartArcMap.AddIns;

namespace StreetSmartArcMap.Buttons
{
    public class OpenStreetSmartWindowButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private Configuration Config => Configuration.Instance;

        private IDockableWindow window { get; set; }

        public OpenStreetSmartWindowButton()
        {
            //
            var dockWindowManager = ArcMap.Application as IDockableWindowManager;

            UID windowId = new UIDClass { Value = "Cyclomedia_StreetSmartArcMap_DockableWindows_StreetSmartDockableWindow" };

            window = dockWindowManager.GetDockableWindow(windowId);
        }

        protected override async void OnClick()
        {
            if (Config.Agreement)
            {
                if (!window.IsVisible())
                    window.Show(true);

                StreetSmartApiWrapper.Instance.SetOverlayDrawDistance(Configuration.Instance.OverlayDrawDistanceInMeters, ArcMap.Document.FocusMap.DistanceUnits);

                await StreetSmartApiWrapper.Instance.Open(Configuration.Instance.ApiSRS, "Lange Haven 145, Schiedam");
            }
        }

        protected override void OnUpdate()
        {
            Enabled = StreetSmartExtension.GetExtension().IsEnabled;

            base.OnUpdate();
        }
    }
}
