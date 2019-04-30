using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using StreetSmartArcMap.Layers;
using System.Diagnostics;
using StreetSmartArcMap.AddIns;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Utilities;

namespace StreetSmartArcMap.Buttons
{
    public class StreetSmartShowInCyclorama : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private const string MenuItem = "esriArcMapUI.FeatureLayerContextMenu";
        private const string CommandItem = "CycloMedia_StreetSmartArcMap_StreetSmartShowInCyclorama";

        #region members
        private CycloMediaLayer _cycloMediaLayer;
        private VectorLayer _vectorLayer;

        private readonly LogClient _logClient;

        #endregion

        public StreetSmartShowInCyclorama()
        {
        }

        protected override void OnClick()
        {
            try
            {
                Checked = !Checked;
                _logClient.Info(string.Format("Show cyclorama: {0}", Checked));

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
                _logClient.Error("StreetSmartShowInCyclorama.OnClick", ex.Message, ex);
                Trace.WriteLine(ex.Message, "StreetSmartShowInCyclorama.OnClick");
            }
        }

        protected override void OnUpdate()
        {
            try
            {
                StreetSmartExtension extension = StreetSmartExtension.GetExtension();
                IApplication application = ArcMap.Application;
                Enabled = ((application != null) && extension.Enabled);

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
                                CycloMediaGroupLayer cycloMediaGroupLayer = extension.CycloMediaGroupLayer;
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
                _logClient.Error("StreetSmartShowInCyclorama.OnUpdate", ex.Message, ex);
                Trace.WriteLine(ex.Message, "StreetSmartShowInCyclorama.OnUpdate");
            }
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

        #endregion
    }
}
