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
using ESRI.ArcGIS.Geometry;
using StreetSmartArcMap.AddIns;
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

        private const byte Alpha = 192;

        #endregion Constants

        #region Private members

        /// <summary>
        /// A dictionary of the cone belonging to a certain viewer (ID)
        /// </summary>
        private Dictionary<string, ViewingCone> ConePerViewerDict = new Dictionary<string, ViewingCone>();

        /// <summary>
        /// the configuration object
        /// </summary>
        private Configuration.Configuration Config => Configuration.Configuration.Instance;

        /// <summary>
        /// The StreetSmartAPI Instance
        /// </summary>
        private StreetSmartApiWrapper API => StreetSmartApiWrapper.Instance;

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook { get; set; }

        #endregion Private members

        #region Constructor

        public StreetSmartDockableWindow(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
            this.VisibleChanged += StreetSmartDockableWindow_VisibleChanged;

            API.InitApi(Config);
            API.OnViewerChangeEvent += API_OnViewerChangeEvent;
            API.OnViewingConeChanged += API_OnViewingConeChanged;
            this.Controls.Add(API.StreetSmartGUI);
            IDocumentEvents_Event docEvents = (IDocumentEvents_Event)ArcMap.Document;
            docEvents.MapsChanged += DocEvents_MapsChanged;
        }

        private void StreetSmartDockableWindow_VisibleChanged(object sender, EventArgs e)
        {
            SetVisibility();
        }

        #endregion Constructor

        #region Event handlers

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
                    RedrawCones();
                }
            }
        }

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
                    foreach (var removeViewer in missingViewers)
                    {
                        ConePerViewerDict.Remove(removeViewer);
                    }
                }
                var avEvents = ArcUtils.ActiveViewEvents;
                if (avEvents != null)
                {
                    avEvents.AfterDraw -= AvEvents_AfterDraw;
                    avEvents.AfterDraw += AvEvents_AfterDraw;
                }
            }
        }

        private IEnvelope2 CalculateConesEnvelope()
        {
            double xmin = double.MaxValue;
            double xmax = double.MinValue;
            double ymin = double.MaxValue;
            double ymax = double.MinValue;

            lock (ConePerViewerDict)
            {
                foreach (var kvp in ConePerViewerDict)
                {
                    var coordinate = kvp.Value.Coordinate;

                    if (coordinate.X > xmax)
                        xmax = coordinate.X.Value;
                    if (coordinate.X < xmin)
                        xmin = coordinate.X.Value;
                    if (coordinate.Y > ymax)
                        ymax = coordinate.Y.Value;
                    if (coordinate.Y < ymin)
                        ymin = coordinate.Y.Value;
                }
            }

            IEnvelope2 envelope = (IEnvelope2)new Envelope()
            {
                XMin = xmin,
                XMax = xmax,
                YMin = ymin,
                YMax = ymax,
            };

            envelope.SpatialReference = ArcUtils.SpatialReference;
            envelope.Expand(1.1, 1.1, true);

            if (ArcMap.Document.ActivatedView.Extent.XMin < xmin)
                xmin = ArcMap.Document.ActivatedView.Extent.XMin;
            if (ArcMap.Document.ActivatedView.Extent.XMax > xmax)
                xmax = ArcMap.Document.ActivatedView.Extent.XMax;
            if (ArcMap.Document.ActivatedView.Extent.YMin < ymin)
                ymin = ArcMap.Document.ActivatedView.Extent.YMin;
            if (ArcMap.Document.ActivatedView.Extent.YMax > ymax)
                ymax = ArcMap.Document.ActivatedView.Extent.YMax;

            return envelope;
        }

        internal void SetMapExtentToCones(bool redraw = true)
        {
            var view = ArcMap.Document.ActiveView;

            if (view != null)
                view.Extent = CalculateConesEnvelope();

            if (redraw)
                RedrawMapExtent();
        }

        private void RedrawMapExtent()
        {
            ArcMap.Document?.ActiveView?.ScreenDisplay?.Invalidate(ArcMap.Document.ActiveView.Extent, true, (short)esriScreenCache.esriNoScreenCache);
        }

        private void API_OnViewingConeChanged(ViewingConeChangeEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => API_OnViewingConeChanged(args)));
            }
            else
            {
                var cone = args.ViewingCone;
                var viewerId = args.ViewerId;
                lock (ConePerViewerDict)
                {
                    ConePerViewerDict[viewerId] = cone;
                }

                RedrawMapExtent();
            }
        }

        private void DocEvents_MapsChanged()
        {
            API.SetOverlayDrawDistance(Config.OverlayDrawDistanceInMeters, ArcMap.Document.FocusMap.DistanceUnits);
        }

        #endregion Event handlers

        #region Functions (Private)

        internal void SetVisibility()
        {
            SetVisibility(ConePerViewerDict != null && ConePerViewerDict.Count > 0);

            RedrawCones();
        }

        private void SetVisibility(bool visible)
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

        private void RedrawCones()
        {
            var config = Configuration.Configuration.Instance;
            int srs = int.Parse(config.ApiSRS.Substring(config.ApiSRS.IndexOf(":") + 1));
            // empty views
            var extension = StreetSmartExtension.GetExtension();
            if (extension.InsideScale())
            {
                var display = ArcMap.Document?.ActiveView?.ScreenDisplay;
                display.Filter = new TransparencyDisplayFilterClass { Transparency = Alpha };
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

                        var symbol = new SimpleFillSymbol()
                        {
                            Color = Converter.ToRGBColor(cone.Color),
                            Style = esriSimpleFillStyle.esriSFSSolid
                        };

                        int screenX, screenY;
                        displayTransformation.FromMapPoint(mappoint, out screenX, out screenY);

                        double angleh = (cone.Orientation.HFov ?? 0.0) * Math.PI / 360;
                        double angle = (270 + (cone.Orientation.Yaw ?? 0.0)) % 360 * Math.PI / 180;
                        double angle1 = angle - angleh;
                        double angle2 = angle + angleh;
                        int size = 48; // was: const (value 96) / 2

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

        #endregion Functions (Private)

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