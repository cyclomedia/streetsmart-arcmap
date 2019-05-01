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

using ESRI.ArcGIS.esriSystem;
using StreetSmart.Common.Exceptions;
using StreetSmart.Common.Factories;
using StreetSmart.Common.Interfaces.API;
using StreetSmart.Common.Interfaces.Data;
using StreetSmart.Common.Interfaces.DomElement;
using StreetSmart.WinForms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreetSmartArcMap.Logic
{
    public class StreetSmartApiWrapper
    {
        #region private const

        public const string ApiKey = "O3Qd-D85a3YF6DkNmLEp-XU9OrQpGX8RG7IZi7UFKTAFO38ViDo9CD4xmbcdejcd";

        #endregion private const

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

        #endregion Singleton construction

        #region private properties

        private IPanoramaViewerOptions PanoramaOptions { get; set; }
        private IList<ViewerType> ViewerTypes { get; set; }
        private IStreetSmartAPI StreetSmartAPI { get; set; }
        private IStreetSmartOptions StreetSmartOptions { get; set; }
        private IOptions ApiOptions { get; set; }
        private bool RequestOpen { get; set; }
        private bool RequestOverlay { get; set; }
        private int RequestOverlayDistance { get; set; }
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
        public bool Initialised { get; private set; }

        #endregion public properties

        #region private functions

        /// <summary>
        /// Eventhandler is notified when the API is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StreetSmartAPI_APIReady(object sender, EventArgs e)
        {
            await Init();
        }

        /// <summary>
        /// Initialises the Street Smart API with configured settings
        /// </summary>
        /// <returns></returns>
        private async Task Init()
        {
            try
            {
                RestartStreetSmartAPI(StreetSmartOptions);

                // Open image
                ViewerTypes = new List<ViewerType> { ViewerType.Panorama };
                PanoramaOptions = PanoramaViewerOptionsFactory.Create(true, false, true, true, true, true);
                PanoramaOptions.MeasureTypeButtonToggle = false;
                Initialised = true;
                if (RequestOpen)
                {
                    RequestOpen = false;
                    await Open(RequestSRS, RequestQuery);
                }
                if (RequestOverlay)
                {
                    RequestOverlay = false;
                    StreetSmartAPI.SetOverlayDrawDistance(RequestOverlayDistance);
                }
            }
            catch (StreetSmartLoginFailedException)
            {
                MessageBox.Show("api laden >> login failed");
            }
        }

        #endregion private functions

        #region public functions

        public void RestartStreetSmartAPI(IStreetSmartOptions options)
        {
            try
            {
                //Destroy if existing
                if (Initialised)
                    StreetSmartAPI.Destroy(ApiOptions).Wait(5000);

                //Create new
                IAddressSettings addressSettings = AddressSettingsFactory.Create(StreetSmartOptions.AddressLocale, StreetSmartOptions.AddressDatabase);
                IDomElement element = DomElementFactory.Create();

                ApiOptions = OptionsFactory.Create(StreetSmartOptions.ApiUsername, StreetSmartOptions.ApiPassword, ApiKey, StreetSmartOptions.ApiSRS, addressSettings, element);

                StreetSmartAPI.Init(ApiOptions);
            }
            catch (Exception ex)
            {
                //
            }
        }

        public void InitApi(IStreetSmartOptions options, IStreetSmartAPI api = null)
        {
            if (api == null)
                StreetSmartAPI = StreetSmartAPIFactory.Create();
            else
                StreetSmartAPI = api;

            StreetSmartOptions = options;
            StreetSmartAPI.APIReady += StreetSmartAPI_APIReady;
        }

        public void SetOverlayDrawDistance(int distance, esriUnits mapUnits)
        {
            switch (mapUnits)
            {
                case esriUnits.esriFeet:
                    distance = (int)Math.Round(distance * 3.280839895, 0);
                    break;

                default: break;
            }

            if (!Initialised)
            { // wait until initialised
                RequestOverlay = true;
                RequestOverlayDistance = distance;
            }
            else
            {
                StreetSmartAPI.SetOverlayDrawDistance(distance);
            }
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
                if (!Initialised)
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