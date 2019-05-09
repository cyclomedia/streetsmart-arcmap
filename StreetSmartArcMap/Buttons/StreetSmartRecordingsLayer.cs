using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Layers;
using System.Diagnostics;
using System.Windows.Forms;
using StreetSmartArcMap.Utilities;

namespace StreetSmartArcMap.Buttons
{
    public class StreetSmartRecordingsLayer : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        /// <summary>
        /// The name of the menu and the command item of this button
        /// </summary>
        private const string LayerName = "Recent Recordings";
        private const string MenuItem = "esriArcMapUI.MxAddDataMenu";
        private const string CommandItem = "CycloMedia_StreetSmartArcMap_StreetSmartRecordingsLayer";

        private readonly LogClient _logClient;


        public StreetSmartRecordingsLayer()
        {
            _logClient = new LogClient(typeof(StreetSmartRecordingsLayer));
            Checked = false;
            StreetSmartExtension extension = StreetSmartExtension.GetExtension();
            CycloMediaGroupLayer groupLayer = extension.CycloMediaGroupLayer;

            if (groupLayer != null)
            {
                IList<CycloMediaLayer> layers = groupLayer.Layers;

                foreach (var layer in layers)
                {
                    if (layer.IsRemoved)
                    {
                        CycloMediaLayerRemoved(layer);
                    }
                    else
                    {
                        CycloMediaLayerAdded(layer);
                    }
                }
            }

            CycloMediaLayer.LayerAddedEvent += CycloMediaLayerAdded;
            CycloMediaLayer.LayerRemoveEvent += CycloMediaLayerRemoved;
        }

        #region other event handlers

        private void CycloMediaLayerAdded(CycloMediaLayer layer)
        {
            try
            {
                if (layer != null)
                {
                    Checked = (layer.Name == LayerName) || Checked;
                }
            }
            catch (Exception ex)
            {
                _logClient.Error("StreetSmartRecordingsLayer.CycloMediaLayerAdded", ex.Message, ex);
                Trace.WriteLine(ex.Message, "StreetSmartRecordingsLayer.CycloMediaLayerAdded");
            }
        }

        private void CycloMediaLayerRemoved(CycloMediaLayer layer)
        {
            try
            {
                if (layer != null)
                {
                    Checked = (layer.Name != LayerName) && Checked;
                }
            }
            catch (Exception ex)
            {
                _logClient.Error("StreetSmartRecordingsLayer.CycloMediaLayerRemoved", ex.Message, ex);
                Trace.WriteLine(ex.Message, "StreetSmartRecordingsLayer.CycloMediaRemoved");
            }
        }

        #endregion

        #region add or remove button from the menu

        public static void AddToMenu()
        {
            ArcUtils.AddCommandItem(MenuItem, CommandItem, 0);
        }

        public static void RemoveFromMenu()
        {
            ArcUtils.RemoveCommandItem(MenuItem, CommandItem);
        }

        #endregion

        protected override void OnClick()
        {
            try
            {
                OnUpdate();
                StreetSmartExtension extension = StreetSmartExtension.GetExtension();

                if (Checked)
                {
                    extension.RemoveLayer(LayerName);
                }
                else
                {
                    extension.AddLayers(LayerName);
                }
            }
            catch (Exception ex)
            {
                _logClient.Error("GsRecentDataLayer.OnClick", ex.Message, ex);

                MessageBox.Show(ex.Message, Properties.Resources.ErrorIntegration);
            }
        }

        protected override void OnUpdate()
        {
        }
    }
}
