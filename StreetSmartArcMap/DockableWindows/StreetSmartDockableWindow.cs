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

using ESRI.ArcGIS.ADF.Connection.Local;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Layers;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Logic.Model;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WinPoint = System.Drawing.Point;

namespace StreetSmartArcMap.DockableWindows
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class StreetSmartDockableWindow : UserControl
    {
        #region Constants

        private const double Size = 96.0;
        private const byte Alpha = 192;

        #endregion Constants

        private Dictionary<string, ViewingCone> ConePerViewerDict = new Dictionary<string, ViewingCone>();

        private Configuration.Configuration Config => Configuration.Configuration.Instance;
        private StreetSmartApiWrapper API => StreetSmartApiWrapper.Instance;

        public StreetSmartDockableWindow(object hook)
        {
            InitializeComponent();
            this.Hook = hook;

            API.InitApi(Config);
            API.OnViewerChangeEvent += API_OnViewerChangeEvent;
            API.OnViewingConeChanged += API_OnViewingConeChanged;
            this.Controls.Add(API.StreetSmartGUI);

            IDocumentEvents_Event docEvents = (IDocumentEvents_Event)ArcMap.Document;
            docEvents.MapsChanged += DocEvents_MapsChanged;

            var avEvents = ArcUtils.ActiveViewEvents;
            if (avEvents != null)
            {
                avEvents.AfterDraw += AvEvents_AfterDraw;
            }
        }

        

        private void AvEvents_AfterDraw(IDisplay Display, esriViewDrawPhase phase)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AvEvents_AfterDraw(Display, phase)));
            }
            else
            {
                if (phase == esriViewDrawPhase.esriViewForeground)
                {
                    redrawCones();
                }
            }
        }

        private delegate void viewerChangeDelegate(ViewersChangeEventArgs args);

        private void API_OnViewerChangeEvent(ViewersChangeEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => API_OnViewerChangeEvent(args)));
            }
            else
            {
                SetVisibility(args.Viewers.Count > 0);
                var missingViewers = new List<string>();
                lock (ConePerViewerDict)
                {
                    foreach (var kvp in ConePerViewerDict)
                    {
                        if (!args.Viewers.Any(v => v == kvp.Key))
                        {
                            missingViewers.Add(kvp.Key);
                        }
                    }
                    foreach (var removeViewer in missingViewers) {
                        ConePerViewerDict.Remove(removeViewer);
                    }
                }
                // force a repaint
                var display = ArcMap.Document?.ActiveView?.ScreenDisplay;
                display.Invalidate(ArcMap.Document.ActiveView.Extent, true, (short)esriScreenCache.esriNoScreenCache);
            }
        }

        internal void SetVisibility(bool visible)
        {
            if (visible)
            {
                Visible = true;
            }
            else
            {
                var dockWindowManager = ArcMap.Application as ESRI.ArcGIS.Framework.IDockableWindowManager;
                ESRI.ArcGIS.esriSystem.UID windowId = new ESRI.ArcGIS.esriSystem.UIDClass { Value = "Cyclomedia_StreetSmartArcMap_DockableWindows_StreetSmartDockableWindow" };
                ESRI.ArcGIS.Framework.IDockableWindow window = dockWindowManager.GetDockableWindow(windowId);

                if (window.IsVisible())
                    window.Show(false);
            }
        }

        private void API_OnViewingConeChanged(ViewingConeChangeEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => API_OnViewingConeChanged(args)));
            }
            else {
                var cone = args.ViewingCone;
                var viewerId = args.ViewerId;
                lock (ConePerViewerDict)
                {
                    ConePerViewerDict[viewerId] = cone;
                }
                IEnvelope2 env = (IEnvelope2)new Envelope()
                {
                    XMin = double.MaxValue,
                    XMax = double.MinValue,
                    YMin = double.MaxValue,
                    YMax = double.MinValue
                };
                env.SpatialReference = ArcUtils.SpatialReference;
                lock (ConePerViewerDict)
                {
                    foreach (var kvp in ConePerViewerDict)
                    {
                        var coordinate = kvp.Value.Coordinate;
                        if (coordinate.X > env.XMax)
                        {
                            env.XMax = coordinate.X.Value;
                        }
                        if (coordinate.X < env.XMin)
                        {
                            env.XMin = coordinate.X.Value;
                        }
                        if (coordinate.Y > env.YMax)
                        {
                            env.YMax = coordinate.Y.Value;
                        }
                        if (coordinate.Y < env.YMin)
                        {
                            env.YMin = coordinate.Y.Value;
                        }
                    }
                }

                // TODO: check if the cones are within this extent. If not, it's too close to the edge and we need to rescale.
                //var extent = (IEnvelope2)ArcMap.Document.ActiveView.Extent;
                //extent.Expand(0.8, 0.8, true);
                
                

                env.Expand(1.1, 1.1, true);
                ArcMap.Document.ActiveView.Extent.Union(env);
                IActiveView activeView = ArcUtils.ActiveView;

                var display = ArcMap.Document?.ActiveView?.ScreenDisplay;
                display.Invalidate(ArcMap.Document.ActiveView.Extent, true, (short)esriScreenCache.esriNoScreenCache);
            }
        }

        private void redrawCones()
        {
            var config = Configuration.Configuration.Instance;
            int srs = int.Parse(config.ApiSRS.Substring(config.ApiSRS.IndexOf(":") + 1));
            // empty views
            var extension = StreetSmartExtension.GetExtension();
            if (extension.InsideScale())
            {
                var display = ArcMap.Document?.ActiveView?.ScreenDisplay;
                var displayTransformation = display.DisplayTransformation;
                display.StartDrawing(display.hDC, (short)esriScreenCache.esriNoScreenCache);

                lock (ConePerViewerDict)
                {
                    foreach (var kvp in ConePerViewerDict)
                    {
                        var cone = kvp.Value;

                        var mappoint = new Point()
                        {
                            X = cone.Coordinate.X.Value,
                            Y = cone.Coordinate.Y.Value,
                            Z = cone.Coordinate.Z.Value,
                            SpatialReference = new SpatialReferenceEnvironmentClass().CreateSpatialReference(srs)
                        };
                        // Project the API SRS to the current map SRS.
                        mappoint.Project(ArcUtils.SpatialReference);

                        var esriColor = Converter.ToRGBColor(cone.Color);
                        esriColor.Transparency = Alpha; // TODO: does this work?
                        var symbol = new SimpleFillSymbol()
                        {
                            Color = esriColor,
                            Style = esriSimpleFillStyle.esriSFSSolid
                        };

                        int screenX, screenY;
                        displayTransformation.FromMapPoint(mappoint, out screenX, out screenY);

                        double angleh = (cone.Orientation.HFov ?? 0.0) * Math.PI / 360;
                        double angle = (270 + (cone.Orientation.Yaw ?? 0.0)) % 360 * Math.PI / 180;
                        double angle1 = angle - angleh;
                        double angle2 = angle + angleh;
                        int size = (int)Size / 2;

                        var screenPoint1 = new WinPoint(screenX + (int)(size * Math.Cos(angle1)), screenY + (int)(size * Math.Sin(angle1)));
                        var screenPoint2 = new WinPoint(screenX + (int)(size * Math.Cos(angle2)), screenY + (int)(size * Math.Sin(angle2)));
                        var point1 = displayTransformation.ToMapPoint(screenPoint1.X, screenPoint1.Y);
                        var point2 = displayTransformation.ToMapPoint(screenPoint2.X, screenPoint2.Y);

                        var polygon = new Polygon();
                        polygon.AddPoint(mappoint);
                        polygon.AddPoint(point1);
                        polygon.AddPoint(point2);

                        display.SetSymbol((ISymbol)symbol);
                        display.DrawPolygon((IGeometry)polygon);
                    }
                }
                display.FinishDrawing();
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