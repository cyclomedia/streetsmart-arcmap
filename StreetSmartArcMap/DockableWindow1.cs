﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

using StreetSmart.Common.Factories;
using StreetSmart.Common.Exceptions;
using StreetSmart.Common.Interfaces.Data;
using StreetSmart.Common.Interfaces.DomElement;
using StreetSmart.Common.Interfaces.API;
using StreetSmart.Common.Interfaces.Events;
using StreetSmart.Common.Interfaces.GeoJson;

namespace TestArcMapAddin
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWindow1 : UserControl
    {
        public IStreetSmartAPI StreetSmartAPI { get; private set; }

        public DockableWindow1(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
            StreetSmartAPI = StreetSmartAPIFactory.Create();
            this.Controls.Add(StreetSmartAPI.GUI);

            StreetSmartAPI.APIReady += ApiReady;
        }

        private async void ApiReady(object sender, EventArgs args)
        {
            await InitApi();
        }

        private async Task InitApi()
        {
            string epsgCode = "EPSG:28992";
            IAddressSettings addressSettings = AddressSettingsFactory.Create("nl", "CMDatabase");
            IDomElement element = DomElementFactory.Create();
            var _options = OptionsFactory.Create("gbo", "Gg200786001", "testdfdsfj", epsgCode, addressSettings, element);

            try
            {
                await StreetSmartAPI.Init(_options);

                // Open image
                IList<ViewerType> viewerTypes = new List<ViewerType> { ViewerType.Panorama };
                IPanoramaViewerOptions panoramaOptions = PanoramaViewerOptionsFactory.Create(true, false, true, true, true, true);
                panoramaOptions.MeasureTypeButtonToggle = false;
                IViewerOptions viewerOptions = ViewerOptionsFactory.Create(viewerTypes, epsgCode, panoramaOptions);
                try
                {
                    IList<IViewer> viewers = await StreetSmartAPI.Open("Lange Haven 145, Schiedam", viewerOptions);
                }
                catch (StreetSmartImageNotFoundException)
                {
                    MessageBox.Show("image openen >> kapot");
                }
            }
            catch (StreetSmartLoginFailedException)
            {
                MessageBox.Show("api laden >> kapot");
            }
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindow1 m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new DockableWindow1(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }
    }
}
