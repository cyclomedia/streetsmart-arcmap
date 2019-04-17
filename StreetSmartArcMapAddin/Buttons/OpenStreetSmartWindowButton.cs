using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;

namespace StreetSmartArcMapAddin.Buttons
{
    public class OpenStreetSmartWindowButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public OpenStreetSmartWindowButton()
        {
            //
        }

        protected override void OnClick()
        {
            var application = ArcMap.Application;
            var dockWindowManager = application as IDockableWindowManager;
            UID windowId = new UIDClass { Value = "Cyclomedia_StreetSmartArcMapAddin_DockableWindows_StreetSmartDockableWindow" };
            var window = dockWindowManager.GetDockableWindow(windowId);
            window.Show(true);
        }

        protected override void OnUpdate()
        {
        }
    }
}
