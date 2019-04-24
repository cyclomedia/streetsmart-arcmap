using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using StreetSmart.Common.Factories;
using StreetSmart.Common.Exceptions;
using StreetSmart.Common.Interfaces.Data;
using StreetSmart.Common.Interfaces.DomElement;
using StreetSmart.Common.Interfaces.API;
using StreetSmart.Common.Interfaces.Events;
using StreetSmart.Common.Interfaces.GeoJson;
using System.Threading.Tasks;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Logic.Configuration;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.esriSystem;

namespace StreetSmartArcMap.DockableWindows
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class StreetSmartDockableWindow : UserControl
    {

        public StreetSmartDockableWindow(object hook)
        {
            InitializeComponent();
            this.Hook = hook;

            StreetSmartApiWrapper.Instance.InitApi(Configuration.Instance);
            this.Controls.Add(StreetSmartApiWrapper.Instance.StreetSmartGUI);


            IDocumentEvents_Event docEvents = (IDocumentEvents_Event)ArcMap.Document;
            docEvents.MapsChanged += DocEvents_MapsChanged;
        }

        private void DocEvents_MapsChanged()
        {
            StreetSmartApiWrapper.Instance.SetOverlayDrawDistance(Configuration.Instance.OverlayDrawDistanceInMeters, ArcMap.Document.FocusMap.DistanceUnits); 
        }


        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook { get; set; }

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
    }
}
