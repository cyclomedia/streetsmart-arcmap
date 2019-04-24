using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreetSmart.Common.Factories;
using StreetSmart.Common.Exceptions;
using StreetSmart.Common.Interfaces.Data;
using StreetSmart.Common.Interfaces.DomElement;
using StreetSmart.Common.Interfaces.API;
using StreetSmart.Common.Interfaces.Events;
using StreetSmart.Common.Interfaces.GeoJson;
using System.Threading;
using StreetSmart.WinForms;
using System.Windows.Forms;

namespace StreetSmartArcMap.Logic
{
    public class StreetSmartApiWrapper
    {
        #region Singleton construction
        private static StreetSmartApiWrapper _instance;
        public static StreetSmartApiWrapper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StreetSmartApiWrapper();

                return _instance;
            }
        }
        #endregion Singleton contruction

        #region private properties
        private IPanoramaViewerOptions PanoramaOptions { get; set; }
        private IList<ViewerType> ViewerTypes { get; set; }
        private IStreetSmartAPI StreetSmartAPI { get; set; }
        private IStreetSmartOptions StreetSmartOptions { get; set; }

        private bool RequestOpen { get; set; }
        private string RequestQuery { get; set; }
        private string RequestSRS { get; set; }
        #endregion private properties

        #region public properties
        /// <summary>
        /// A reference to the GUI object to dock
        /// </summary>
        public StreetSmartGUI StreetSmartGUI => StreetSmartAPI?.GUI;
        /// <summary>
        /// Has the wrapper / API been initialized yet?
        /// </summary>
        public bool Initialized => (PanoramaOptions != null && ViewerTypes != null);
        #endregion public properties

        #region private functions
        /// <summary>
        /// Initialises the Street Smart API with configured settings
        /// </summary>
        /// <returns></returns>
        private async Task Init()
        {
            IAddressSettings addressSettings = AddressSettingsFactory.Create(StreetSmartOptions.AddressLocale, StreetSmartOptions.AddressDatabase);
            IDomElement element = DomElementFactory.Create();
            var apiOptions = OptionsFactory.Create(StreetSmartOptions.ApiUsername, StreetSmartOptions.ApiPassword, StreetSmartOptions.ApiKey, StreetSmartOptions.ApiSRS, addressSettings, element);

            try
            {
                await StreetSmartAPI.Init(apiOptions);

                // Open image
                ViewerTypes = new List<ViewerType> { ViewerType.Panorama };
                PanoramaOptions = PanoramaViewerOptionsFactory.Create(true, false, true, true, true, true);
                PanoramaOptions.MeasureTypeButtonToggle = false;

                if (RequestOpen)
                {
                    RequestOpen = false;
                    await Open(RequestSRS, RequestQuery);
                }
            }
            catch (StreetSmartLoginFailedException)
            {
                MessageBox.Show("api laden >> login failed");
            }
        }

        /// <summary>
        /// Eventhandler is notified when the API is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StreetSmartAPI_APIReady(object sender, EventArgs e)
        {
            await Init();
        }
        #endregion private functions


        #region public functions
        public void InitApi(IStreetSmartOptions options, IStreetSmartAPI api = null)
        {
            if (api == null)
            {
                StreetSmartAPI = StreetSmartAPIFactory.Create();
            }
            else
            {
                StreetSmartAPI = api;
            }
            StreetSmartOptions = options;
            StreetSmartAPI.APIReady += StreetSmartAPI_APIReady;
        }
        
        /// <summary>
        /// Opens a StreetSmart view on this location/imageID
        /// </summary>
        /// <param name="srsCode"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task Open(string srsCode, string query)
        {
            try
            {
                if (!Initialized)
                {
                    RequestOpen = true;
                    RequestQuery = query;
                    RequestSRS = srsCode;
                }
                else
                {
                    RequestOpen = false;
                    IViewerOptions options = ViewerOptionsFactory.Create(ViewerTypes, srsCode, PanoramaOptions);
                    IList<IViewer> viewers = await StreetSmartAPI.Open(query, options);
                }
            }
            catch (StreetSmartImageNotFoundException)
            {
                //MessageBox.Show("image openen >> kapot");
            }
        }
        #endregion public functions

    }
}
