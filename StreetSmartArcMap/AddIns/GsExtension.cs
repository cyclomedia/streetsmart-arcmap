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
using System.Diagnostics;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Forms;
using StreetSmartArcMap.Utilities;
using StreetSmartArcMap.Logic.Configuration;

namespace StreetSmartArcMap.AddIns
{
    public delegate void OpenDocumentDelegate();

    /// <summary>
    /// The extension of the cyclomedia layers
    /// </summary>
    public class GsExtension : Extension
    {
        public static event OpenDocumentDelegate OpenDocumentEvent;

        private static GsExtension _extension;

        public bool IsEnabled => State == ExtensionState.Enabled;
        private Configuration Config => Configuration.Instance;

        #region event handlers

        protected override void OnStartup()
        {
            Configuration.AgreementChanged += OnAgreementChanged;

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
                Trace.WriteLine(ex.Message, "GsExtension.OnShutdown");
            }

            Configuration.AgreementChanged -= OnAgreementChanged;

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
                        docEvents.ActiveViewChanged += OnActiveViewChanged;
                    }

                }
                else
                {
                    Uninitialize();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "GsExtension.OnSetState");
            }

            return base.OnSetState(State);
        }

        protected override ExtensionState OnGetState()
        {
            return State;
        }

        #endregion

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

        #endregion

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
                Trace.WriteLine(ex.Message, "GsExtension.OpenDocument");
            }
        }

        private void CloseDocument()
        {
            try
            {
                //
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "GsExtension.CloseDocument");
            }
        }

        private void ItemDeleted(object item)
        {
            try
            {
                //
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "GsExtension.ItemDeleted");
            }
        }

        private void Afterdraw(IDisplay display, esriViewDrawPhase drawPhase)
        {
            if (drawPhase == esriViewDrawPhase.esriViewForeground)
            {
                //
            }
        }

        private void OnActiveViewChanged()
        {
            //
        }

        #endregion

        internal static GsExtension GetExtension()
        {
            if (_extension == null)
            {
                try
                {
                    UID extId = new UIDClass { Value = ThisAddIn.IDs.GsExtension };

                    _extension = ArcMap.Application.FindExtensionByCLSID(extId) as GsExtension;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message, "GsExtension.GetExtension");
                }
            }

            return _extension;
        }
    }
}
