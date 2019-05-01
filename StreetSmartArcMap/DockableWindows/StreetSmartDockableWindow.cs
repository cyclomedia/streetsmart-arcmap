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

using ESRI.ArcGIS.ArcMapUI;
using StreetSmartArcMap.Logic;
using System;
using System.Windows.Forms;

namespace StreetSmartArcMap.DockableWindows
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class StreetSmartDockableWindow : UserControl
    {
        private Configuration.Configuration Config => Configuration.Configuration.Instance;
        private StreetSmartApiWrapper API => StreetSmartApiWrapper.Instance;

        public StreetSmartDockableWindow(object hook)
        {
            InitializeComponent();
            this.Hook = hook;

            API.InitApi(Config);
            API.OnViewerChangeEvent += API_OnViewerChangeEvent;
            this.Controls.Add(API.StreetSmartGUI);

            IDocumentEvents_Event docEvents = (IDocumentEvents_Event)ArcMap.Document;
            docEvents.MapsChanged += DocEvents_MapsChanged;
        }
        private delegate void viewerChangeDelegate(ViewersChangeEventArgs args);
        private void API_OnViewerChangeEvent(ViewersChangeEventArgs args)
        {
            if (InvokeRequired)
            {
                var d = new viewerChangeDelegate(API_OnViewerChangeEvent);
                Invoke(d, new object[] { args });
            }
            else
            {
                if (args.NumberOfViewers == 0)
                    this.Visible = false;
            }
        }

        private void DocEvents_MapsChanged()
        {
            API.SetOverlayDrawDistance(Config.OverlayDrawDistanceInMeters, ArcMap.Document.FocusMap.DistanceUnits);
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook { get; set; }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private StreetSmartDockableWindow m_windowUI;

            public AddinImpl()
            {
                //
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new StreetSmartDockableWindow(this.Hook);

                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                m_windowUI?.Dispose(disposing);

                base.Dispose(disposing);
            }
        }
    }
}