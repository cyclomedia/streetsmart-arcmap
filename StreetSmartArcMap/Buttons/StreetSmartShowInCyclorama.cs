/*
 * Integration in ArcMap for StreetSmart
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

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Layers;
using StreetSmartArcMap.Utilities;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace StreetSmartArcMap.Buttons
{
    public class StreetSmartShowInCyclorama : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private const string MenuItem = "esriArcMapUI.FeatureLayerContextMenu";
        private const string CommandItem = "CycloMedia_StreetSmartArcMap_StreetSmartShowInCyclorama";

        #region members

        private CycloMediaLayer _cycloMediaLayer;
        private VectorLayer _vectorLayer;

        private Configuration.Configuration Config => Configuration.Configuration.Instance;
        #endregion members

        public StreetSmartShowInCyclorama()
        {
            Config.SetCulture();

            Caption = Properties.Resources.StreetSmartShowInCycloramaButtonCaption;
            Tooltip = Properties.Resources.StreetSmartShowInCycloramaButtonTip;
        }

        protected override void OnClick()
        {
            try
            {
                Checked = !Checked;

                if (_cycloMediaLayer != null)
                {
                    _cycloMediaLayer.IsVisibleInStreetSmart = Checked;
                }

                if (_vectorLayer != null)
                {
                    _vectorLayer.IsVisibleInStreetSmart = Checked;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "StreetSmartShowInCyclorama.OnClick");
            }
        }

        protected override void OnUpdate()
        {
            try
            {
                StreetSmartExtension extension = StreetSmartExtension.GetExtension();
                IApplication application = ArcMap.Application;
                Enabled = ((application != null) && (extension?.Enabled ?? false));

                if (application != null)
                {
                    var document = application.Document as IMxDocument;

                    if (document != null)
                    {
                        var tocDisplayView = document.CurrentContentsView as TOCDisplayView;

                        if (tocDisplayView != null)
                        {
                            var selectedItem = tocDisplayView.SelectedItem as ILayer;

                            if (selectedItem != null)
                            {
                                _vectorLayer = VectorLayer.GetLayer(selectedItem);
                                CycloMediaGroupLayer cycloMediaGroupLayer = extension?.CycloMediaGroupLayer;
                                _cycloMediaLayer = (cycloMediaGroupLayer == null) ? null : cycloMediaGroupLayer.GetLayer(selectedItem);

                                if (_cycloMediaLayer != null)
                                {
                                    Checked = _cycloMediaLayer.IsVisibleInStreetSmart;
                                    Enabled = _cycloMediaLayer.IsVisible;
                                }

                                if (_vectorLayer != null)
                                {
                                    Checked = _vectorLayer.IsVisibleInStreetSmart;
                                    Enabled = _vectorLayer.IsVisible;
                                }
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "StreetSmartShowInCyclorama.OnUpdate");
            }
            Caption = Properties.Resources.StreetSmartShowInCycloramaButtonCaption;
            Tooltip = Properties.Resources.StreetSmartShowInCycloramaButtonTip;
        }

        #region add or remove button from the menu

        public static void AddToMenu()
        {
            ArcUtils.AddCommandItem(MenuItem, CommandItem, 0);
        }

        public static void RemoveFromMenu()
        {
            ArcUtils.RemoveCommandItem(MenuItem, CommandItem);
        }

        #endregion add or remove button from the menu
    }
}