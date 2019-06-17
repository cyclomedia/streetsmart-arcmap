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

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using StreetSmartArcMap.Buttons;
using StreetSmartArcMap.Forms;
using StreetSmartArcMap.Layers;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Utilities;
using System;
using System.Diagnostics;

namespace StreetSmartArcMap.AddIns
{
    public delegate void OpenDocumentDelegate();

    /// <summary>
    /// The extension of the cyclomedia layers
    /// </summary>
    public class StreetSmartExtension : Extension
    {
        public static event OpenDocumentDelegate OpenDocumentEvent;

        private static StreetSmartExtension _extension;
        public CycloMediaGroupLayer CycloMediaGroupLayer { get; private set; }
        public bool CommunicatingWithStreetSmart { get; set; }
        public bool IsEnabled => State == ExtensionState.Enabled;
        private Configuration.Configuration Config => Configuration.Configuration.Instance;

        public bool Enabled
        {
            get
            {
                return (State == ExtensionState.Enabled);
            }
        }

        public bool AddToMenu => false;

        #region event handlers

        protected override void OnStartup()
        {
            Configuration.Configuration.AgreementChanged += OnAgreementChanged;
            Configuration.Configuration.Instance.SetCulture();

            _extension = this;

            base.OnStartup();
        }

        protected override void OnShutdown()
        {
            try
            {
                _extension = null;

                if (IsEnabled)
                    Uninitialize();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "StreetSmartExtension.OnShutdown");
            }

            Configuration.Configuration.AgreementChanged -= OnAgreementChanged;

            base.OnShutdown();
        }

        protected override bool OnSetState(ExtensionState state)
        {
            try
            {
                //Allow user to accept agreement.
                if (state == ExtensionState.Enabled && !Config.Agreement)
                {
                    Config.Agreement = AgreementForm.ShowAgreement();
                    Config.Save();
                }

                //Only enable when user has accepted agreement.
                if (state == ExtensionState.Enabled && Config.Agreement)
                    State = ExtensionState.Enabled;
                else
                    State = ExtensionState.Disabled;

                if (IsEnabled)
                {
                    var docEvents = ArcUtils.MxDocumentEvents;

                    if (docEvents != null)
                    {
                        docEvents.OpenDocument += OpenDocument;
                        docEvents.CloseDocument += CloseDocument;
                        
                    }
                }
                else
                {
                    Uninitialize();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "StreetSmartExtension.OnSetState");
            }

            return base.OnSetState(State);
        }

        protected override ExtensionState OnGetState()
        {
            return State;
        }

        #endregion event handlers

        #region functions

        internal void ShowStreetSmart()
        {
            if (Config.Agreement)
            {
                var dockWindowManager = ArcMap.Application as IDockableWindowManager;
                UID windowId = new UIDClass { Value = "Cyclomedia_StreetSmartArcMap_DockableWindows_StreetSmartDockableWindow" };
                IDockableWindow window = dockWindowManager.GetDockableWindow(windowId);

                if (!window.IsVisible())
                    window.Show(true);

                StreetSmartApiWrapper.Instance.SetOverlayDrawDistance(Config.OverlayDrawDistanceInMeters, ArcMap.Document.FocusMap.DistanceUnits);
            }
        }

        internal bool InsideScale()
        {
            return (CycloMediaGroupLayer != null) && CycloMediaGroupLayer.InsideScale;
        }

        private void OnAgreementChanged(object sender, bool value)
        {
            OnSetState(value ? ExtensionState.Enabled : ExtensionState.Disabled);
        }

        public void Uninitialize()
        {
            var docEvents = ArcUtils.MxDocumentEvents;

            if (docEvents != null)
            {
                docEvents.OpenDocument -= OpenDocument;
                docEvents.CloseDocument -= CloseDocument;
            }
        }

        private void CloseCycloMediaLayer(bool closeDocument)
        {
            if (CycloMediaGroupLayer != null)
            {
                if ((!ContainsCycloMediaLayer()) || closeDocument)
                {
                    RemoveLayers();
                }
            }

            if (closeDocument)
            {
                var arcEvents = ArcUtils.ActiveViewEvents;

                if (arcEvents != null)
                {
                    arcEvents.ItemDeleted -= ItemDeleted;
                    arcEvents.AfterDraw -= Afterdraw;
                    //arcEvents.SelectionChanged -= SelectionChanged;

                }

                CycloMediaLayer.LayerRemoveEvent -= OnLayerRemoved;
                StreetSmartRecordingsLayer.RemoveFromMenu();
            }
        }

        public void AddLayers()
        {
            AddLayers(null);
        }

        public void AddLayers(string name)
        {
            if (Enabled)
            {
                if (CycloMediaGroupLayer == null)
                {
                    if (AddToMenu)
                        StreetSmartShowInCyclorama.AddToMenu();

                    StreetSmartConfigurationForm.CheckOpenCredentials();
                    CycloMediaGroupLayer = new CycloMediaGroupLayer();
                }

                if (!string.IsNullOrEmpty(name))
                {
                    CycloMediaGroupLayer.AddLayer(name);
                }
            }
        }

        public void RemoveLayer(string name)
        {
            CycloMediaGroupLayer?.RemoveLayer(name);

            CycloMediaGroupLayer?.Dispose();
            CycloMediaGroupLayer = null;
        }

        public void RemoveLayers()
        {
            if (CycloMediaGroupLayer != null)
            {
                if (AddToMenu)
                    StreetSmartShowInCyclorama.RemoveFromMenu();

                CycloMediaGroupLayer?.Dispose();
                CycloMediaGroupLayer = null;
            }
        }

        private bool ContainsCycloMediaLayer()
        {
            // ReSharper disable UseIndexedProperty
            bool result = false;
            IMap map = ArcUtils.Map;
            var layers = map.get_Layers();
            ILayer layer;

            while ((layer = layers.Next()) != null)
            {
                result = ((CycloMediaGroupLayer == null)
                            ? (layer.Name == "CycloMedia")
                            : CycloMediaGroupLayer.IsKnownName(layer.Name)) || result;
            }

            // ReSharper restore UseIndexedProperty
            return result;
        }

        #endregion functions

        #region other event handlers

        private void OpenDocument()
        {
            try
            {
                var arcEvents = ArcUtils.ActiveViewEvents;

                if (arcEvents != null)
                {
                    arcEvents.ItemDeleted += ItemDeleted;
                    arcEvents.AfterDraw += Afterdraw;
                    //arcEvents.SelectionChanged += SelectionChanged;
                }

                OpenDocumentEvent?.Invoke();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "StreetSmartExtension.OpenDocument");
            }

            if (ContainsCycloMediaLayer())
            {
                AddLayers();
            }
            StreetSmartRecordingsLayer.AddToMenu();
        }

        //private async void SelectionChanged()
        //{
        //    var map = ArcUtils.Map;
        //    var setup = (IEnumFeatureSetup)map.FeatureSelection;
        //    setup.AllFields = true;
        //    var selection = (IEnumFeature)map.FeatureSelection;
        //    IFeature feature = selection.Next();

        //    if (feature == null) // no selection
        //    {
        //        await StreetSmartApiWrapper.Instance.DeselectAll();
        //    }

        //    while (feature != null)
        //    {
        //        await StreetSmartApiWrapper.Instance.Select(feature);
        //        feature = selection.Next();
        //    }

        //    ArcUtils.ActiveView?.ScreenDisplay?.Invalidate(ArcUtils.ActiveView.Extent, true, (short)esriScreenCache.esriNoScreenCache);
        //}

        private void CloseDocument()
        {
            try
            {
                CloseCycloMediaLayer(true);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "StreetSmartExtension.CloseDocument");
            }
        }

        private void ItemDeleted(object item)
        {
            try
            {
                CloseCycloMediaLayer(false);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "StreetSmartExtension.ItemDeleted");
            }
        }

        private void Afterdraw(IDisplay display, esriViewDrawPhase drawPhase)
        {
            if (drawPhase == esriViewDrawPhase.esriViewForeground)
            {
                //
            }
        }

        private void OnLayerRemoved(CycloMediaLayer cycloMediaLayer)
        {
            if (CycloMediaGroupLayer != null)
            {
                if (!CycloMediaGroupLayer.ContainsLayers)
                {
                    RemoveLayers();
                }
            }
        }


        #endregion other event handlers

        internal static StreetSmartExtension GetExtension()
        {
            if (_extension == null)
            {
                try
                {
                    UID extId = new UIDClass { Value = ThisAddIn.IDs.StreetSmartExtension };

                    _extension = ArcMap.Application.FindExtensionByCLSID(extId) as StreetSmartExtension;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message, "StreetSmartExtension.GetExtension");
                }
            }

            return _extension;
        }
    }
}