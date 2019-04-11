using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;

namespace StreetSmartArcMapAddin
{
    public class ArcGISAddin1 : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ArcGISAddin1()
        {
        }

        protected override void OnClick()
        {
            var application = ArcMap.Application;
            var dockWindowManager = application as IDockableWindowManager;
            UID windowId = new UIDClass { Value = "StreetSmartArcMapAddin_DockableWindow1" };
            var window = dockWindowManager.GetDockableWindow(windowId);
            window.Show(true);
        }

        protected override void OnUpdate()
        {
        }
    }
}
