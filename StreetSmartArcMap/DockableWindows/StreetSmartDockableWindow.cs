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
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Layers;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Objects;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private const int MaxTimeUpdate = 100;

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

        public bool _toUpdateCones { get; private set; }
        public System.Threading.Timer _updateTimer { get; private set; }

        private string _measurementName { get; set; }

        #endregion Private members

        #region Constructor

        public StreetSmartDockableWindow(object hook)
        {
            InitializeComponent();

            this.Hook = hook;
            this.VisibleChanged += StreetSmartDockableWindow_VisibleChanged;

            API.InitApi(Config).Wait();
            API.OnViewerChangeEvent += API_OnViewerChangeEvent;
            API.OnViewingConeChanged += API_OnViewingConeChanged;
            API.OnVectorLayerChanged += API_OnVectorLayerChanged;

            VectorLayer.StartMeasurementEvent += VectorLayer_StartMeasurementEvent;

            this.Controls.Add(API.StreetSmartGUI);
            IDocumentEvents_Event docEvents = (IDocumentEvents_Event)ArcMap.Document;
            docEvents.MapsChanged += DocEvents_MapsChanged;
        }

        private void StreetSmartDockableWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible) // switching from visible to hidden!
                RemoveConesFromScreen();
        }

        #endregion Constructor

        #region Event handlers



        private void API_OnViewerChangeEvent(ViewersChangeEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => API_OnViewerChangeEvent(args)));
            }
            else
            {

                // force a repaint.
                var activeView = ArcUtils.ActiveView;
                activeView.ScreenDisplay?.Invalidate(activeView.Extent, true, (short)esriScreenCache.esriNoScreenCache);

                var closedViewers = new List<string>();
                lock (ConePerViewerDict)
                {
                    foreach (var kvp in ConePerViewerDict)
                    {
                        if (!args.Viewers.Any(v => v == kvp.Key))
                        {
                            closedViewers.Add(kvp.Key);
                        }
                    }
                    foreach (var removeViewer in closedViewers)
                    {
                        ConePerViewerDict.Remove(removeViewer);
                    }
                }

                SetVisibility(args.Viewers.Count > 0);
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
                    if (ConePerViewerDict.ContainsKey(viewerId))
                    {
                        var _current = ConePerViewerDict[viewerId];
                        if (!_current.Equals(cone))
                        {
                            _current.Dispose();
                            ConePerViewerDict[viewerId] = cone;
                            cone.Redraw();
                        }
                    }
                    else
                    {
                        ConePerViewerDict[viewerId] = cone;
                        cone.Redraw();
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

        private void API_OnVectorLayerChanged(VectorLayerChangeEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => API_OnVectorLayerChanged(args)));
            }
            else
            {
                args.Layer.IsVisibleInStreetSmart = true;
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
                    foreach (var kvp in ConePerViewerDict)
                    {
                        var cone = kvp.Value;
                        cone.AfterDraw(Display, phase);
                    }
                }
            }
        }

        private void DocEvents_MapsChanged()
        {
            API.SetOverlayDrawDistance(Config.OverlayDrawDistanceInMeters, ArcMap.Document.FocusMap.DistanceUnits);
        }

        #endregion Event handlers

        #region Functions (Private)

        private void RemoveConesFromScreen()
        {
            foreach (var kvp in ConePerViewerDict)
            {
                kvp.Value.Dispose();
            }
            ConePerViewerDict = new Dictionary<string, ViewingCone>();
        }

        private void SetVisibility(bool visible)
        {

            var dockWindowManager = ArcMap.Application as ESRI.ArcGIS.Framework.IDockableWindowManager;
            ESRI.ArcGIS.esriSystem.UID windowId = new ESRI.ArcGIS.esriSystem.UIDClass { Value = "Cyclomedia_StreetSmartArcMap_DockableWindows_StreetSmartDockableWindow" };
            ESRI.ArcGIS.Framework.IDockableWindow window = dockWindowManager.GetDockableWindow(windowId);

            if (visible)
            {

                if (!window.IsVisible())
                    window.Show(true);
            }
            else
            {
                if (window.IsVisible())
                    window.Show(false);
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

        //private void OnStartMeasurement(IGeometry geometry)
        //{
        //    if (Config.MeasurePermissions)
        //    {
        //        Measurement measurement = Measurement.Sketch;
        //        StartMeasurement(geometry, measurement, true);
        //    }
        //}

        private void VectorLayer_StartMeasurementEvent(IGeometry geometry)
        {
            if (Config.MeasurePermissions)
            {
                Measurement measurement = Measurement.Sketch;
                StartMeasurement(geometry, measurement, true);
            }
        }

        private async Task StartMeasurement(IGeometry geometry, Measurement measurement, bool sketch)
        {
            if (Config.MeasurePermissions && API != null && measurement != null)
            {
                var typeOfLayer = Measurement.GetTypeOfLayer(geometry);

                if (typeOfLayer != TypeOfLayer.None)
                {
                    if (measurement.IsTypeOfLayer(typeOfLayer))
                    {
                        measurement.OpenMeasurement();
                    }
                    else
                    {
                        measurement.RemoveMeasurement();

                        _measurementName = "my measurement";

                        Measurement.CloseOpenMeasurement();

                        int entityId = await API.CreateMeasurement(typeOfLayer);

                        measurement = Measurement.Get(entityId);
                        measurement?.Open();

                        if (sketch)
                            measurement?.SetSketch();
                    }
                }
            }
        }

        public void EnableMeasurementSeries(int entityId)
        {
            throw new NotImplementedException();
        }

        public void DisableMeasurementSeries()
        {
            throw new NotImplementedException();
        }



        public void CreateMeasurementPoint(int entityId, IPoint gsPoint)
        {
            throw new NotImplementedException();
        }

        public void OpenMeasurementPoint(int entityId, int pointId)
        {
            throw new NotImplementedException();
        }

        public void CloseMeasurementPoint(int entityId, int pointId)
        {
            throw new NotImplementedException();
        }

        public void RemoveMeasurementPoint(int entityId, int pointId)
        {
            throw new NotImplementedException();
        }



        public int GetMeasurementPointIndex(int entityId, int pointId)
        {
            throw new NotImplementedException();
        }
    }
}