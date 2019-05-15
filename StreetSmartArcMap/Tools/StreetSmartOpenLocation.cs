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

using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Layers;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Logic.Model;
using StreetSmartArcMap.Logic.Model.Atlas;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ESRI.ArcGIS.Desktop.AddIns;

namespace StreetSmartArcMap.Tools
{
    public class StreetSmartOpenLocation : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        private Cursor _thisCursor;
        private readonly LogClient _logClient;

        public StreetSmartOpenLocation()
        {
            _logClient = new LogClient(typeof(StreetSmartOpenLocation));
        }

        private Keys keyPressed = Keys.None;
        protected override void OnKeyDown(KeyEventArgs arg)
        {
            keyPressed = arg.KeyCode;
            base.OnKeyDown(arg);
        }

        protected override void OnKeyUp(KeyEventArgs arg)
        {
            keyPressed = Keys.None;
            base.OnKeyUp(arg);
        }

        protected override async void OnMouseUp(MouseEventArgs arg)
        {
            try
            {
                CycloMediaLayer layer;
                string imageId = GetImageIdFromPoint(arg, out layer);

                if ((!string.IsNullOrEmpty(imageId)) && (layer != null))
                {
                    IMappedFeature mappedFeature = layer.GetLocationInfo(imageId);
                    var recording = mappedFeature as Recording;

                    if (recording != null)
                    {
                        if ((recording.IsAuthorized == null) || ((bool)recording.IsAuthorized))
                        {
                            var extension = StreetSmartExtension.GetExtension();
                            extension.ShowStreetSmart();

                            await StreetSmartApiWrapper.Instance.Open(Configuration.Configuration.Instance.ApiSRS, imageId, (keyPressed == Keys.ShiftKey));



                            ArcMap.Document?.ActiveView?.ScreenDisplay?.Invalidate(ArcMap.Document.ActiveView.Extent, true, (short)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);
                        }
                        else
                        {
                            MessageBox.Show(Properties.Resources.ErrorNotAuthorized);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logClient.Error("GsOpenLocation.OnMouseUp", ex.Message, ex);
                Trace.WriteLine(ex.Message, "GsOpenLocation.OnMouseUp");
            }

            base.OnMouseUp(arg);
        }

        protected override void OnActivate()
        {
            if (_thisCursor == null)
            {
                Type thisType = GetType();
                string cursorPath = string.Format(@"StreetSmartArcMap.Images.{0}.cur", thisType.Name);
                Assembly thisAssembly = Assembly.GetAssembly(thisType);
                Stream cursorStream = thisAssembly.GetManifestResourceStream(cursorPath);

                if (cursorStream != null)
                {
                    _thisCursor = new Cursor(cursorStream);
                }
            }

            base.OnActivate();
        }

        protected override void OnUpdate()
        {
            try
            {
                var enabled = Enabled;

                var extension = StreetSmartExtension.GetExtension();
                Enabled = ((ArcMap.Application != null) && extension.Enabled && extension.CycloMediaGroupLayer != null && extension.CycloMediaGroupLayer.Layers.Any(l => l is RecordingLayer));

                if (enabled && !Enabled)
                    OnDeactivate();
            }
            catch (Exception ex)
            {
                _logClient.Error("StreetSmartOpenLocation.OnUpdate", ex.Message, ex);
                Trace.WriteLine(ex.Message, "StreetSmartOpenLocation.OnUpdate");
            }

            base.OnUpdate();
        }

        protected override void OnMouseMove(MouseEventArgs arg)
        {
            try
            {
                CycloMediaLayer layer;
                // TODO: coordinate conversion??
                string imageId = GetImageIdFromPoint(arg, out layer);
                Cursor = (string.IsNullOrEmpty(imageId) || (layer == null)) ? _thisCursor : Cursors.Arrow;
            }
            catch (Exception ex)
            {
                _logClient.Error("StreetSmartOpenLocation.OnMouseMove", ex.Message, ex);
                Trace.WriteLine(ex.Message, "StreetSmartOpenLocation.OnMouseMove");
            }

            base.OnMouseMove(arg);
        }


        #region Functions

        /// <summary>
        /// This function calculates the imageId for location on the screen
        /// </summary>
        /// <param name="arg">The mouse arguments</param>
        /// <param name="layer">The layer where the point has been found</param>
        /// <returns>The imageId of the point</returns>
        private static string GetImageIdFromPoint(MouseEventArgs arg, out CycloMediaLayer layer)
        {
            layer = null;
            string result = string.Empty;
            var extension = StreetSmartExtension.GetExtension();

            if (extension.InsideScale())
            {
                int x = arg.X;
                int y = arg.Y;
                CycloMediaGroupLayer cycloMediaGroupLayer = extension.CycloMediaGroupLayer;
                result = cycloMediaGroupLayer.GetFeatureFromPoint(x, y, out layer);
            }

            return result;
        }

        #endregion
    }
}