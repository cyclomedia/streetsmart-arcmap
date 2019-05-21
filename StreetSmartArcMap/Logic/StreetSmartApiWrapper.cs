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
using StreetSmart.Common.Interfaces.Events;
using StreetSmart.WinForms;
using StreetSmartArcMap.Layers;
using StreetSmartArcMap.Logic.Model;
using StreetSmartArcMap.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreetSmartArcMap.Logic
{
    public delegate void ViewersChangeEventDelegate(ViewersChangeEventArgs args);
    public delegate void ViewingConeChangeEventDelegate(ViewingConeChangeEventArgs args);

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

        private StreetSmartApiWrapper()
        {
            Initialised = false;
        }

        #endregion Singleton construction

        #region private properties
        private bool Loading = false;
        private IPanoramaViewerOptions DefaultPanoramaOptions { get; set; }
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

        #region Events

        public event ViewersChangeEventDelegate OnViewerChangeEvent;

        public ViewingConeChangeEventDelegate OnViewingConeChanged;

        #endregion Events

        #region private functions

        private IPanoramaViewerOptions GetPanoramaOptions(bool addExtraViewer)
        {
            if (DefaultPanoramaOptions == null)
            {
                DefaultPanoramaOptions = PanoramaViewerOptionsFactory.Create(true, false, true, true, true, true);
                DefaultPanoramaOptions.MeasureTypeButtonToggle = false;
            }
            if (addExtraViewer)
            {
                var panoramaOptions = PanoramaViewerOptionsFactory.Create(true, false, true, true, false, true);
                panoramaOptions.MeasureTypeButtonToggle = false;
                return panoramaOptions;
            }
            else
            {
                return DefaultPanoramaOptions;
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

        /// <summary>
        /// Initialises the Street Smart API with configured settings
        /// </summary>
        /// <returns></returns>
        private async Task Init()
        {
            try
            {
                //await RestartStreetSmartAPI(StreetSmartOptions);


                if (StreetSmartOptions != null)
                {
                    IAddressSettings addressSettings = AddressSettingsFactory.Create(StreetSmartOptions.AddressLocale, StreetSmartOptions.AddressDatabase);
                    IDomElement element = DomElementFactory.Create();
                    ApiOptions = StreetSmartOptions.UseDefaultBaseUrl ?
                        OptionsFactory.Create(StreetSmartOptions.ApiUsername, StreetSmartOptions.ApiPassword, ApiKey, StreetSmartOptions.ApiSRS, StreetSmartOptions.LocaleToUse, addressSettings, element) : 
                        OptionsFactory.Create(StreetSmartOptions.ApiUsername, StreetSmartOptions.ApiPassword, ApiKey, StreetSmartOptions.ApiSRS, StreetSmartOptions.LocaleToUse, StreetSmartOptions.ConfigurationUrlToUse, addressSettings, element);

                    await StreetSmartAPI.Init(ApiOptions);
                }

                VectorLayer.LayerAddEvent += VectorLayer_LayerAddEvent;
                VectorLayer.LayerRemoveEvent += VectorLayer_LayerRemoveEvent;
                VectorLayer.LayerChangedEvent += VectorLayer_LayerChangedEvent;

                VectorLayer.FeatureStartEditEvent += VectorLayer_FeatureStartEditEvent;
                VectorLayer.FeatureUpdateEditEvent += VectorLayer_FeatureUpdateEditEvent;
                VectorLayer.FeatureDeleteEvent += VectorLayer_FeatureDeleteEvent;

                VectorLayer.StopEditEvent += VectorLayer_StopEditEvent;
                VectorLayer.StartMeasurementEvent += VectorLayer_StartMeasurementEvent;

                VectorLayer.SketchCreateEvent += VectorLayer_SketchCreateEvent;
                VectorLayer.SketchModifiedEvent += VectorLayer_SketchModifiedEvent;
                VectorLayer.SketchFinishedEvent += VectorLayer_SketchFinishedEvent;
                    
                //TODO: remove VectorLayer events

                // Open image
                ViewerTypes = new List<ViewerType> { ViewerType.Panorama };
                Initialised = true;

                if (RequestOverlay)
                {
                    RequestOverlay = false;
                    StreetSmartAPI.SetOverlayDrawDistance(RequestOverlayDistance);
                }

                if (RequestOpen)
                    await Open(RequestSRS, RequestQuery);

                OnViewerChangeEvent?.Invoke(new ViewersChangeEventArgs() { Viewers = new List<string>() });
            }
            catch (StreetSmartLoginFailedException)
            {
                MessageBox.Show("api laden >> login failed");
            }
        }

        private void VectorLayer_LayerAddEvent(VectorLayer layer)
        {
            //
        }

        private void VectorLayer_LayerRemoveEvent(VectorLayer layer)
        {
            //
        }

        private void VectorLayer_LayerChangedEvent(VectorLayer layer)
        {
            //
        }

        private void VectorLayer_FeatureStartEditEvent(IList<ESRI.ArcGIS.Geometry.IGeometry> geometries)
        {
            //
        }

        private void VectorLayer_FeatureUpdateEditEvent(ESRI.ArcGIS.Geodatabase.IFeature feature)
        {
            //
        }

        private void VectorLayer_FeatureDeleteEvent(ESRI.ArcGIS.Geodatabase.IFeature feature)
        {
            //
        }

        private void VectorLayer_StopEditEvent()
        {
            //
        }

        private void VectorLayer_StartMeasurementEvent(ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            //
        }

        private void VectorLayer_SketchCreateEvent(ESRI.ArcGIS.Editor.IEditSketch3 sketch)
        {
            //
        }

        private void VectorLayer_SketchModifiedEvent(ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            //
        }

        private void VectorLayer_SketchFinishedEvent()
        {
           //
        }

        #endregion private functions

        #region public functions

        public async Task RestartStreetSmartAPI(IStreetSmartOptions options)
        {
            try
            {
                //Destroy if existing
                if (Initialised)
                {
                    Initialised = false;
                    await StreetSmartAPI.Destroy(ApiOptions);
                    await InitApi(options);
                }

                ////Open request
                //if (Initialised && RequestQuery != null)
                //    await Open(RequestSRS, RequestQuery);
            }
            catch
            {
                //
            }
        }

        public async Task InitApi(IStreetSmartOptions options, IStreetSmartAPI api = null)
        {
            StreetSmartOptions = options;
            if (api == null)
            {
                if (StreetSmartAPI == null)
                {
                    StreetSmartAPI = StreetSmartAPIFactory.Create();
                    StreetSmartAPI.APIReady += StreetSmartAPI_APIReady;
                    StreetSmartAPI.ViewerRemoved += StreetSmartAPI_ViewerRemoved;
                    StreetSmartAPI.ViewerAdded += StreetSmartAPI_ViewerAdded;
                }
                else
                {
                    // already ready, resume init
                    await Init();
                }
            }
            else
            {
                StreetSmartAPI = api;
                await Init();
            }

        }

        private async void NotifyViewerChange()
        {
            var viewers = await StreetSmartAPI.GetViewers();
            var viewerIds = new List<string>();
            foreach (var viewer in viewers)
            {
                var id = await viewer.GetId();
                viewerIds.Add(id);
            }
            //viewers.
            // TODO: remove viewing cone of the removed viewers!
            OnViewerChangeEvent?.Invoke(new ViewersChangeEventArgs() { Viewers = viewerIds });
        }

        private async void StreetSmartAPI_ViewerAdded(object sender, IEventArgs<IViewer> e)
        {
            NotifyViewerChange();

            if (e.Value != null && e.Value is IPanoramaViewer)
            {
                var viewer = e.Value as IPanoramaViewer;

                viewer.ImageChange += Viewer_ImageChange;
                viewer.ViewChange += Viewer_ViewChange;

                var cone = await CreateCone(viewer);
                var args = new ViewingConeChangeEventArgs()
                {
                    ViewerId = await viewer.GetId(),
                    ViewingCone = cone
                };
                OnViewingConeChanged?.Invoke(args);
            }
        }

        private async void Viewer_ImageChange(object sender, EventArgs e)
        {
            if (sender != null && sender is IPanoramaViewer && StreetSmartAPI != null)
            {
                var viewer = (sender as IPanoramaViewer);
                var cone = await CreateCone(viewer);
                var args = new ViewingConeChangeEventArgs()
                {
                    ViewerId = await viewer.GetId(),
                    ViewingCone = cone
                };
                OnViewingConeChanged?.Invoke(args);
            }
        }

        private async void Viewer_ViewChange(object sender, IEventArgs<IOrientation> e)
        {
            if (sender != null && sender is IPanoramaViewer && StreetSmartAPI != null)
            {
                var viewer = (sender as IPanoramaViewer);
                var cone = await CreateCone(viewer);
                var args = new ViewingConeChangeEventArgs()
                {
                    ViewerId = await viewer.GetId(),
                    ViewingCone = cone
                };
                OnViewingConeChanged?.Invoke(args);
            }
        }

        private async Task<ViewingCone> CreateCone(IPanoramaViewer viewer)
        {
            if (viewer == null)
                throw new ApplicationException();

            while (Loading)
            {
                await Task.Delay(100);
            }
            var recording = await viewer.GetRecording();

            return new ViewingCone
            {
                ImageId = recording.Id,
                Coordinate = recording.XYZ,
                Orientation = await viewer.GetOrientation(),
                Color = await viewer.GetViewerColor(),
                ViewerId = await viewer.GetId()
            };
        }

        private void StreetSmartAPI_ViewerRemoved(object sender, IEventArgs<IViewer> e)
        {
            // TODO: remove this viewer from the viewercones
            NotifyViewerChange();
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
        public async Task Open(string srsCode, string query, bool addExtraViewer = false)
        {
            try
            {
                RequestQuery = query;

                if (!Initialised)
                {
                    RequestOpen = true;
                    RequestSRS = srsCode;
                }
                else
                {
                    RequestOpen = false;
                    Loading = true;
                    IViewerOptions options = ViewerOptionsFactory.Create(ViewerTypes, srsCode, GetPanoramaOptions(addExtraViewer));
                    IList<IViewer> viewers = await StreetSmartAPI.Open(query, options);
                    Loading = false;
                    NotifyViewerChange();
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