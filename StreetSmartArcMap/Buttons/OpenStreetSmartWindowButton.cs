using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using StreetSmartArcMap.Logic;

namespace StreetSmartArcMap.Buttons
{
    public class OpenStreetSmartWindowButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private IDockableWindow window { get; set; }
        public OpenStreetSmartWindowButton()
        {
            //
            var application = ArcMap.Application;
            var dockWindowManager = application as IDockableWindowManager;
            UID windowId = new UIDClass { Value = "Cyclomedia_StreetSmartArcMap_DockableWindows_StreetSmartDockableWindow" };
            window = dockWindowManager.GetDockableWindow(windowId);
        }

        protected override async void OnClick()
        {
            //var config = StreetSmartArcMap.Logic.Configuration.Configuration.Instance;

            if (!window.IsVisible())
                window.Show(true);

            await StreetSmartApiWrapper.Instance.Open("EPSG:28992", "Lange Haven 145, Schiedam");

        }

        protected override void OnUpdate()
        {
            //
        }

        
    }
}
