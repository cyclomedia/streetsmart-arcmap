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
using StreetSmart.Common.Interfaces.GeoJson;
using StreetSmart.Common.Interfaces.SLD;
using StreetSmart.WinForms;
using StreetSmartArcMap.Layers;
using StreetSmartArcMap.Objects;
using StreetSmartArcMap.Utilities;
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
            _vectorLayers = new List<VectorLayer>();
            _vectorLayerInChange = new List<VectorLayer>();
        }

        #endregion Singleton construction

        #region private properties

        private Configuration.Configuration Config => Configuration.Configuration.Instance;
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
        private readonly IList<VectorLayer> _vectorLayers;
        private readonly IList<VectorLayer> _vectorLayerInChange;

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

                // these events still need to be implemented:
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

        private async Task RemoveVectorLayerAsync(VectorLayer vectorLayer)
        {
            IOverlay overlay = vectorLayer?.Overlay;

            if (overlay != null)
            {
                await StreetSmartAPI.RemoveOverlay(overlay.Id);
                vectorLayer.Overlay = null;
                if (_vectorLayers.Contains(vectorLayer))
                    _vectorLayers.Remove(vectorLayer);
            }
        }

        private async Task AddVectorLayerAsync(VectorLayer vectorLayer)
        {
            var viewers = await StreetSmartAPI.GetViewers();
            var recordings = new List<IRecording>();
            foreach (var v in viewers)
            {
                var rec = await ((IPanoramaViewer)v).GetRecording();
                recordings.Add(rec);
            }
            var color = vectorLayer.GetColor();

            var spatRel = Config.SpatialReference;
            string srsName = (spatRel == null) ? ArcUtils.EpsgCode : spatRel.SRSName;
            string layerName = vectorLayer.Name;

            IFeatureCollection geoJson = vectorLayer.GenerateJson(recordings);
            IStyledLayerDescriptor sld = vectorLayer.CreateSld(geoJson, color);

            var overlay = OverlayFactory.Create(geoJson, layerName, srsName, color);
            var o = await StreetSmartAPI.AddOverlay(overlay);
            vectorLayer.Overlay = o;
            _vectorLayers.Add(vectorLayer);
        }

        private async void VectorLayer_LayerAddEvent(VectorLayer layer)
        {
            if (layer != null && layer.IsVisibleInStreetSmart && StreetSmartAPI != null)
            {
                if (!_vectorLayerInChange.Contains(layer))
                {
                    _vectorLayerInChange.Add(layer);
                    await RemoveVectorLayerAsync(layer);
                    await AddVectorLayerAsync(layer);
                    _vectorLayerInChange.Remove(layer);
                }
            }
        }

        private async void VectorLayer_LayerRemoveEvent(VectorLayer layer)
        {
            if (layer != null && layer.IsVisibleInStreetSmart && StreetSmartAPI != null)
            {
                if (!_vectorLayerInChange.Contains(layer))
                {
                    _vectorLayerInChange.Add(layer);
                    await RemoveVectorLayerAsync(layer);
                    _vectorLayerInChange.Remove(layer);
                }
            }
        }

        private async void VectorLayer_LayerChangedEvent(VectorLayer layer)
        {
            if (StreetSmartAPI != null)
            {
                if (layer != null)
                {
                    if (!_vectorLayerInChange.Contains(layer))
                    {
                        _vectorLayerInChange.Add(layer);
                        if (layer.IsVisibleInStreetSmart)
                        {
                            await RemoveVectorLayerAsync(layer);
                            await AddVectorLayerAsync(layer);
                        }
                        else
                        {
                            await RemoveVectorLayerAsync(layer);
                        }
                        _vectorLayerInChange.Remove(layer);
                    }
                    else
                    {
                        IList<VectorLayer> vectorLayers = VectorLayer.Layers;
                        int i = 0;

                        while (i < _vectorLayers.Count)
                        {
                            var lay = _vectorLayers.ElementAt(i);

                            if (!vectorLayers.Contains(lay))
                            {
                                await StreetSmartAPI.RemoveOverlay(lay.Overlay.Id);
                                _vectorLayers.Remove(lay);
                            }
                            else
                            {
                                i++;
                            }
                        }

                        foreach (var vectorLayer in vectorLayers)
                        {
                            if (!_vectorLayers.Contains(vectorLayer))
                            {
                                VectorLayer_LayerAddEvent(vectorLayer);
                            }
                            else
                            {
                                if (vectorLayer.IsVisibleInStreetSmart)
                                {
                                    VectorLayer_LayerAddEvent(vectorLayer);
                                }
                                else
                                {
                                    if (_vectorLayers.Contains(vectorLayer))
                                    {
                                        await StreetSmartAPI.RemoveOverlay(vectorLayer.Overlay.Id);
                                        _vectorLayers.Remove(vectorLayer);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void VectorLayer_FeatureStartEditEvent(IList<ESRI.ArcGIS.Geometry.IGeometry> geometries)
        {
            //if (Config.MeasurePermissions && (geometries != null))
            //{
            //    var usedMeasurements = new List<Measurement>();

            //    foreach (ESRI.ArcGIS.Geometry.IGeometry geometry in geometries)
            //    {
            //        if (geometry != null)
            //        {
            //            Measurement measurement = Measurement.Get(geometry);
            //            _drawPoint = false;
            //            measurement = StartMeasurement(geometry, measurement, false);
            //            _drawPoint = true;

            //            if (measurement != null)
            //            {
            //                measurement.UpdateMeasurementPoints(geometry);
            //                measurement.CloseMeasurement();
            //                usedMeasurements.Add(measurement);
            //            }
            //        }
            //    }

            //    Measurement.RemoveUnusedMeasurements(usedMeasurements);
            //}
        }

        private void VectorLayer_FeatureUpdateEditEvent(ESRI.ArcGIS.Geodatabase.IFeature feature)
        {
            //if (Config.MeasurePermissions && (feature != null))
            //{
            //    ESRI.ArcGIS.Geometry.IGeometry geometry = feature.Shape;

            //    if (geometry != null)
            //    {
            //        Measurement measurement = Measurement.Get(geometry);

            //        if (measurement != null)
            //        {
            //            measurement.UpdateMeasurementPoints(geometry);
            //        }
            //    }
            //}
        }

        private void VectorLayer_FeatureDeleteEvent(ESRI.ArcGIS.Geodatabase.IFeature feature)
        {
            //if (Config.MeasurePermissions)
            //{
            //     ESRI.ArcGIS.Geometry.IGeometry geometry = feature.Shape;

            //    if (geometry != null)
            //    {
            //        Measurement measurement = Measurement.Get(geometry);

            //        if (measurement != null)
            //        {
            //            measurement.RemoveMeasurement();
            //        }
            //    }
            //}
        }

        private void VectorLayer_StopEditEvent()
        {
            //if (Config.MeasurePermissions)
            //{
            //    _drawingSketch = false;
            //    Measurement.RemoveAll();
            //    FrmMeasurement.Close();
            //}
        }

        private void VectorLayer_StartMeasurementEvent(ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            //if (Config.MeasurePermissions)
            //{
            //    Measurement measurement = Measurement.Sketch;
            //    StartMeasurement(geometry, measurement, true);
            //}
        }

        private void VectorLayer_SketchCreateEvent(ESRI.ArcGIS.Editor.IEditSketch3 sketch)
        {
            //if (Config.MeasurePermissions && (!_sketchModified) && (!_screenPointAdded) && (_layer != null))
            //{
            //    _sketchModified = true;
            //    _layer.AddZToSketch(sketch);
            //    _sketchModified = false;
            //}
        }

        private void VectorLayer_SketchModifiedEvent(ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            //if (Config.MeasurePermissions)
            //{
            //    _mapPointAdded = !_screenPointAdded;
            //    Measurement measurement = Measurement.Sketch;

            //    if (geometry != null)
            //    {
            //        if (((!_drawingSketch) && (!geometry.IsEmpty)) || (measurement == null))
            //        {
            //            _drawingSketch = true;
            //            measurement = StartMeasurement(geometry, measurement, true);
            //        }

            //        if (measurement != null)
            //        {
            //            measurement.UpdateMeasurementPoints(geometry);
            //        }
            //    }

            //    _mapPointAdded = false;
            //}
        }

        private void VectorLayer_SketchFinishedEvent()
        {
            //if (Config.MeasurePermissions)
            //{
            //    _screenPointAdded = false;
            //    _mapPointAdded = false;
            //    _drawingSketch = false;
            //    Measurement.RemoveSketch();
            //}
        }

        private async Task DeinitApi()
        {
            VectorLayer.LayerAddEvent -= VectorLayer_LayerAddEvent;
            VectorLayer.LayerRemoveEvent -= VectorLayer_LayerRemoveEvent;
            VectorLayer.LayerChangedEvent -= VectorLayer_LayerChangedEvent;

            VectorLayer.FeatureStartEditEvent -= VectorLayer_FeatureStartEditEvent;
            VectorLayer.FeatureUpdateEditEvent -= VectorLayer_FeatureUpdateEditEvent;
            VectorLayer.FeatureDeleteEvent -= VectorLayer_FeatureDeleteEvent;

            VectorLayer.StopEditEvent -= VectorLayer_StopEditEvent;
            VectorLayer.StartMeasurementEvent -= VectorLayer_StartMeasurementEvent;

            VectorLayer.SketchCreateEvent -= VectorLayer_SketchCreateEvent;
            VectorLayer.SketchModifiedEvent -= VectorLayer_SketchModifiedEvent;
            VectorLayer.SketchFinishedEvent -= VectorLayer_SketchFinishedEvent;

            await StreetSmartAPI.Destroy(ApiOptions);
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
                    await DeinitApi();
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