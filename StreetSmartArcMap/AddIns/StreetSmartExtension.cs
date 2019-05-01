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
using StreetSmartArcMap.Buttons;
using StreetSmartArcMap.Forms;
using StreetSmartArcMap.Layers;
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

        public bool IsEnabled => State == ExtensionState.Enabled;
        private Configuration.Configuration Config => Configuration.Configuration.Instance;

        public bool Enabled
        {
            get
            {
                return (State == ExtensionState.Enabled);
            }
        }

        #region event handlers

        protected override void OnStartup()
        {
            Configuration.Configuration.AgreementChanged += OnAgreementChanged;

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
            if (CycloMediaGroupLayer != null)
            {
                CycloMediaGroupLayer.RemoveLayer(name);
            }
        }

        public void RemoveLayers()
        {
            if (CycloMediaGroupLayer != null)
            {
                StreetSmartShowInCyclorama.RemoveFromMenu();
                //FrmCycloMediaOptions.CloseForm();
                //FrmMeasurement.Close();
                //FrmIdentify.Close();
                CycloMediaGroupLayer cycloLayer = CycloMediaGroupLayer;
                CycloMediaGroupLayer = null;
                cycloLayer.Dispose();
                //FrmGlobespotter.ShutDown(true);
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