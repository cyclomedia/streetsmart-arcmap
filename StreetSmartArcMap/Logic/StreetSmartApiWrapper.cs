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

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using StreetSmart.Common.Exceptions;
using StreetSmart.Common.Factories;
using StreetSmart.Common.Interfaces.API;
using StreetSmart.Common.Interfaces.Data;
using StreetSmart.Common.Interfaces.DomElement;
using StreetSmart.Common.Interfaces.Events;
using StreetSmart.Common.Interfaces.GeoJson;
using StreetSmart.Common.Interfaces.SLD;
using StreetSmart.WinForms;
using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Layers;
using StreetSmartArcMap.Objects;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreetSmartArcMap.Logic
{
    public delegate void ViewersChangeEventDelegate(ViewersChangeEventArgs args);
    public delegate void ViewingConeChangeEventDelegate(ViewingConeChangeEventArgs args);
    public delegate void VectorLayerChangeEventDelegate(VectorLayerChangeEventArgs args);
    public delegate void SelectedFeatureChangeEventDelegate(SelectedFeatureChangedEventArgs args);
    public delegate void FeatureClickEventDelegate(FeatureClickEventArgs args);
    public delegate void MeasuremenChangeEventDelegate(MeasuremenChangeEventArgs args);

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
        private readonly LogClient _logClient;
        private bool _screenPointAdded;
        private bool _mapPointAdded;
        private IFeatureCollection ActiveMeasurement { get; set; }

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

        /// <summary>
        /// Check if the API is currently receiving an initialisation so we won't get stuck in an infinite loop.
        /// </summary>
        public bool BusyForMeasurement { get; set; }

        #endregion public properties

        #region Events

        public event ViewersChangeEventDelegate OnViewerChangeEvent;

        public ViewingConeChangeEventDelegate OnViewingConeChanged;
        public VectorLayerChangeEventDelegate OnVectorLayerChanged;
        public MeasuremenChangeEventDelegate OnMeasuremenChanged;
        public SelectedFeatureChangeEventDelegate OnSelectedFeatureChanged;
        public FeatureClickEventDelegate OnFeatureClicked;
        
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
                    if (Config.DevTools)
                        StreetSmartAPI.ShowDevTools();

                    IAddressSettings addressSettings = AddressSettingsFactory.Create(StreetSmartOptions.AddressLocale, StreetSmartOptions.AddressDatabase);
                    IDomElement element = DomElementFactory.Create();
                    ApiOptions = StreetSmartOptions.UseDefaultBaseUrl ?
                        OptionsFactory.Create(StreetSmartOptions.ApiUsername, StreetSmartOptions.ApiPassword, ApiKey, StreetSmartOptions.ApiSRS, StreetSmartOptions.LocaleToUse, addressSettings, element) :
                        OptionsFactory.Create(StreetSmartOptions.ApiUsername, StreetSmartOptions.ApiPassword, ApiKey, StreetSmartOptions.ApiSRS, StreetSmartOptions.LocaleToUse, StreetSmartOptions.ConfigurationUrlToUse, addressSettings, element);

                    await StreetSmartAPI.Init(ApiOptions);


                }

                VectorLayer.DetectVectorLayers(true);

                VectorLayer.LayerAddEvent += VectorLayer_LayerAddEvent;
                VectorLayer.LayerRemoveEvent += VectorLayer_LayerRemoveEvent;
                VectorLayer.LayerChangedEvent += VectorLayer_LayerChangedEvent;

                VectorLayer.FeatureStartEditEvent += VectorLayer_FeatureStartEditEvent;
                // these events still need to be implemented:
                VectorLayer.FeatureUpdateEditEvent += VectorLayer_FeatureUpdateEditEvent;
                VectorLayer.FeatureDeleteEvent += VectorLayer_FeatureDeleteEvent;

                VectorLayer.StopEditEvent += VectorLayer_StopEditEvent;
                VectorLayer.StartMeasurementEvent += VectorLayer_StartMeasurementEvent;

                VectorLayer.SketchCreateEvent += VectorLayer_SketchCreateEvent;
                VectorLayer.SketchModifiedEvent += VectorLayer_SketchModifiedEvent;
                VectorLayer.SketchFinishedEvent += VectorLayer_SketchFinishedEvent;

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

        private void StreetSmartAPI_MeasurementChanged(object sender, IEventArgs<IFeatureCollection> e)
        {
            ActiveMeasurement = e.Value;

            var args = new MeasuremenChangeEventArgs
            {
                Features = e.Value
            };

            OnMeasuremenChanged?.Invoke(args);
        }

        private async Task<List<IRecording>> GetRecordings()
        {
            var recordings = new List<IRecording>();
            var viewers = await StreetSmartAPI.GetViewers();

            foreach (var v in viewers)
            {
                var recording = await ((IPanoramaViewer)v).GetRecording();

                recordings.Add(recording);
            }

            return recordings;
        }

        private async Task<bool> TryAddVectorLayerAsync(VectorLayer layer)
        {
            var recordings = await GetRecordings();

            if (recordings.Count == 0)
                return false;

            Color outline;
            Color color = layer.GetColor(out outline);

            var spatRel = Config.SpatialReference;

            string srsName = (spatRel == null) ? ArcUtils.EpsgCode : spatRel.SRSName;
            string layerName = layer.Name;

            var geoJson = layer.GenerateJson(recordings);
            var sld = layer.CreateSld(geoJson, color, outline);

            IGeoJsonOverlay overlay;
            if (sld != null)
                overlay = OverlayFactory.Create(geoJson, layerName, srsName, sld);
            else
                overlay = OverlayFactory.Create(geoJson, layerName, srsName, color);

            //Set overlay
            layer.Overlay = await StreetSmartAPI.AddOverlay(overlay);

            //Set visibility
            IList<IViewer> viewers = await StreetSmartAPI.GetViewers();
            foreach (var viewer in viewers)
            {
                overlay.Visible = !StoredLayers.Instance.Get(overlay.Name);

                viewer.ToggleOverlay(overlay);
            }

            return true;
        }

        private async Task RemoveVectorLayerAsync(VectorLayer layer)
        {
            IOverlay overlay = layer?.Overlay;

            if (overlay != null)
            {
                await StreetSmartAPI.RemoveOverlay(overlay.Id);

                layer.Overlay = null;

                if (_vectorLayers.Contains(layer))
                    _vectorLayers.Remove(layer);
            }
        }

        private bool AddVectorInChange(VectorLayer layer)
        {
            if (_vectorLayerInChange.Any(v => v.Name == layer.Name))
            {
                return false;
            }
            else
            {
                _vectorLayerInChange.Add(layer);
                return true;
            }
        }

        private void RemoveVectorInchange(VectorLayer layer)
        {
            var query = _vectorLayerInChange.Where(v => v.Name == layer.Name).ToList();

            query.ForEach(l => _vectorLayerInChange.Remove(l));
        }

        private async void VectorLayer_LayerAddEvent(VectorLayer layer)
        {
            if (layer != null && layer.IsVisibleInStreetSmart && StreetSmartAPI != null)
            {
                if (AddVectorInChange(layer))
                {
                    try
                    {
                        if (layer.ContentsChanged)
                        {
                            await RemoveVectorLayerAsync(layer);

                            if (layer.IsVisibleInStreetSmart && !AddVectorInChange(layer) && await TryAddVectorLayerAsync(layer))
                                _vectorLayers.Add(layer);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        RemoveVectorInchange(layer);
                    }
                }
            }
        }

        private async void VectorLayer_LayerRemoveEvent(VectorLayer layer)
        {
            if (layer != null && layer.IsVisibleInStreetSmart && StreetSmartAPI != null)
            {
                if (AddVectorInChange(layer))
                {
                    try
                    {
                        await RemoveVectorLayerAsync(layer);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        RemoveVectorInchange(layer);
                    }
                }
            }
        }

        private async void VectorLayer_LayerChangedEvent(VectorLayer layer)
        {
            if (StreetSmartAPI != null && layer != null)
            {
                if (AddVectorInChange(layer))
                {
                    try
                    {
                        var firstLayer = _vectorLayers.Where(l => l.Name == layer.Name).FirstOrDefault();

                        if (layer.Overlay == null && firstLayer != null)
                            layer = firstLayer;

                        await RemoveVectorLayerAsync(layer);

                        if (layer.IsVisibleInStreetSmart && await TryAddVectorLayerAsync(layer))
                            _vectorLayers.Add(layer);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        RemoveVectorInchange(layer);
                    }
                }
            }
        }

        private void VectorLayer_FeatureStartEditEvent(IList<ESRI.ArcGIS.Geometry.IGeometry> geometries)
        {
            UpdateActiveMeasurement(geometries);
        }

        private void UpdateActiveMeasurement(ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            var geometries = new List<ESRI.ArcGIS.Geometry.IGeometry> { geometry };

            UpdateActiveMeasurement(geometries);
        }

        private void UpdateActiveMeasurement(IList<ESRI.ArcGIS.Geometry.IGeometry> geometries)
        {
            if (GlobeSpotterConfiguration.MeasurePermissions && geometries != null && ActiveMeasurement != null)
            {

                var collection = GeoJsonFactory.CloneFeatureCollection(ActiveMeasurement);

                for (int i = 0; i < collection.Features.Count; i++)
                {
                    var feature = collection.Features[i];
                    int count = 0;

                    if (i < geometries.Count)
                        feature.Geometry = ConvertGeometry(geometries[i], ref count);
                    else
                        feature.Geometry = GeoJsonFactory.CreatePointGeometry(null);

                    var measurementProperties = feature.Properties as IMeasurementProperties;
                    if (measurementProperties != null)
                    {
                        for (int c = measurementProperties.MeasureDetails.Count; c < count; c++)
                        {
                            measurementProperties.MeasureDetails.Add(GeoJsonFactory.CreateMeasureDetails());
                        }
                    }
                }
                try
                {
                    BusyForMeasurement = true;
                    StreetSmartAPI.SetActiveMeasurement(collection);
                }
                finally
                {
                    BusyForMeasurement = false;
                }
            }
        }

        private void VectorLayer_FeatureUpdateEditEvent(ESRI.ArcGIS.Geodatabase.IFeature feature)
        {
            //if (GlobeSpotterConfiguration.MeasurePermissions && feature != null && ActiveMeasurement != null)
            //{
            //    var collection = GeoJsonFactory.CreateFeatureCollection(ArcUtils.SpatialReference.FactoryCode);

            //    var item = ConvertFeature(feature);

            //    if (item != null)
            //        collection.Features.Add(item);

            //    StreetSmartAPI.SetActiveMeasurement(collection);
            //}
        }

        private void VectorLayer_FeatureDeleteEvent(ESRI.ArcGIS.Geodatabase.IFeature feature)
        {
            //if (GlobeSpotterConfiguration.MeasurePermissions && ActiveMeasurement != null)
            //{
            //    var collection = GeoJsonFactory.CreateFeatureCollection(ArcUtils.SpatialReference.FactoryCode);

            //    StreetSmartAPI.SetActiveMeasurement(collection);
            //}
        }

        private async void VectorLayer_StopEditEvent()
        {
            if (GlobeSpotterConfiguration.MeasurePermissions && ActiveMeasurement != null)
            {
                await StopMeasurement();
            }
        }

        private void VectorLayer_SketchCreateEvent(ESRI.ArcGIS.Editor.IEditSketch3 sketch)
        {

            if (GlobeSpotterConfiguration.MeasurePermissions && (!_screenPointAdded))
            {
                var geometry = sketch.Geometry;
                //VectorLayer_FeatureStartEditEvent(new List<ESRI.ArcGIS.Geometry.IGeometry>() { geometry });
            }
        }

        private void VectorLayer_SketchModifiedEvent(ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            if (GlobeSpotterConfiguration.MeasurePermissions && geometry != null && !geometry.IsEmpty)
            {
                UpdateActiveMeasurement(geometry);
            }
        }

        private async void VectorLayer_SketchFinishedEvent()
        {
            if (GlobeSpotterConfiguration.MeasurePermissions)
            {
                await StopMeasurement();
            }
        }

        public async Task CreateMeasurement(TypeOfLayer typeOfLayer)
        {
            var viewers = await StreetSmartAPI.GetViewers();
            var panoramaViewer = viewers?.ToList().Where(v => v is IPanoramaViewer).Select(v => v as IPanoramaViewer).LastOrDefault();

            if (panoramaViewer != null)
            {
                switch (typeOfLayer)
                {
                    case TypeOfLayer.Point:
                        CreatePointMeasurement(panoramaViewer);
                        break;
                    case TypeOfLayer.Line:
                        CreateLineMeasurement(panoramaViewer);
                        break;
                    case TypeOfLayer.Polygon:
                        CreatePolygonMeasurement(panoramaViewer);
                        break;
                    default:
                        break;
                }
            }
        }

        public void CreatePointMeasurement(IPanoramaViewer viewer)
        {
            if (GlobeSpotterConfiguration.MeasurePoint)
            {
                var options = MeasurementOptionsFactory.Create(MeasurementGeometryType.Point);

                StreetSmartAPI.StartMeasurementMode(viewer, options);
            }
        }

        public void CreateLineMeasurement(IPanoramaViewer viewer)
        {
            if (GlobeSpotterConfiguration.MeasureLine)
            {
                var options = MeasurementOptionsFactory.Create(MeasurementGeometryType.LineString);
                StreetSmartAPI.StartMeasurementMode(viewer, options);
            }
        }

        public void CreatePolygonMeasurement(IPanoramaViewer viewer)
        {
            if (GlobeSpotterConfiguration.MeasurePolygon)
            {
                var options = MeasurementOptionsFactory.Create(MeasurementGeometryType.Polygon);

                StreetSmartAPI.StartMeasurementMode(viewer, options);
            }
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
                    if (Config.UseDefaultStreetSmartLocation)
                        StreetSmartAPI = StreetSmartAPIFactory.Create();
                    else
                        StreetSmartAPI = StreetSmartAPIFactory.Create(Config.StreetSmartLocationToUse);

                    StreetSmartAPI.APIReady += StreetSmartAPI_APIReady;
                    StreetSmartAPI.ViewerRemoved += StreetSmartAPI_ViewerRemoved;
                    StreetSmartAPI.ViewerAdded += StreetSmartAPI_ViewerAdded;
                    StreetSmartAPI.MeasurementChanged += StreetSmartAPI_MeasurementChanged;
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

        public async Task StopMeasurement()
        {
            ActiveMeasurement = null;

            if (await StreetSmartAPI.GetApiReadyState())
            {
                StreetSmartAPI.StopMeasurementMode();
            }

            VectorLayer.FinishMeasurement();
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

                viewer.ToggleButtonEnabled(PanoramaViewerButtons.ZoomIn, false);
                viewer.ToggleButtonEnabled(PanoramaViewerButtons.ZoomOut, false);
                viewer.ToggleButtonEnabled(PanoramaViewerButtons.Measure, false);

                viewer.ImageChange += Viewer_ImageChange;
                viewer.ViewChange += Viewer_ViewChange;
                viewer.FeatureSelectionChange += Viewer_FeatureSelectionChange;
                viewer.LayerVisibilityChange += Viewer_LayerVisibilityChange;
                viewer.FeatureClick += Viewer_FeatureClick;

                await InvokeOnViewingConeChanged(viewer);
                InvokeOnVectorLayerChanged();
            }
        }

        private async void Viewer_FeatureClick(object sender, IEventArgs<IFeatureInfo> e)
        {
            if (sender != null && sender is IPanoramaViewer && StreetSmartAPI != null)
            {
                var viewer = sender as IPanoramaViewer;
                IFeatureInfo featureInfo = e.Value;
                await InvokeOnFeatureClicked(viewer, featureInfo);
            }
        }

        private async void Viewer_FeatureSelectionChange(object sender, IEventArgs<IFeatureInfo> e)
        {
            if (sender != null && sender is IPanoramaViewer && StreetSmartAPI != null)
            {
                var viewer = sender as IPanoramaViewer;
                IFeatureInfo featureInfo = e.Value;
                await InvokeOnSelectedFeatureChanged(viewer, featureInfo);
            }
        }

        private void Viewer_LayerVisibilityChange(object sender, IEventArgs<StreetSmart.Common.Interfaces.Data.ILayerInfo> e)
        {
            if (sender != null && sender is IPanoramaViewer && StreetSmartAPI != null)
            {
                var viewer = sender as IPanoramaViewer;
                var layerInfo = e.Value;

                var vectorLayer = _vectorLayers.FirstOrDefault(v => v.Overlay?.Id == layerInfo.LayerId);
                if (vectorLayer != null)
                    StoredLayers.Instance.Update(vectorLayer.Name, layerInfo.Visible);
            }
        }

        private async Task InvokeOnFeatureClicked(IPanoramaViewer viewer, IFeatureInfo featureInfo)
        {
            var args = new FeatureClickEventArgs()
            {
                ViewerId = await viewer.GetId(),
                FeatureInfo = featureInfo
            };

            OnFeatureClicked?.Invoke(args);
        }

        private async Task InvokeOnSelectedFeatureChanged(IPanoramaViewer viewer, IFeatureInfo featureInfo)
        {
            var args = new SelectedFeatureChangedEventArgs()
            {
                ViewerId = await viewer.GetId(),
                FeatureInfo = featureInfo
            };

            OnSelectedFeatureChanged?.Invoke(args);
        }

        private async Task InvokeOnViewingConeChanged(IPanoramaViewer viewer)
        {
            var cone = await CreateCone(viewer);
            var args = new ViewingConeChangeEventArgs()
            {
                ViewerId = await viewer.GetId(),
                ViewingCone = cone
            };

            OnViewingConeChanged?.Invoke(args);
        }

        private void InvokeOnVectorLayerChanged()
        {
            foreach (var layer in VectorLayer.Layers)
            {
                var args = new VectorLayerChangeEventArgs { Layer = layer };

                OnVectorLayerChanged?.Invoke(args);
            }
        }

        private async void Viewer_ImageChange(object sender, EventArgs e)
        {
            if (sender != null && sender is IPanoramaViewer && StreetSmartAPI != null)
            {
                var viewer = sender as IPanoramaViewer;

                await InvokeOnViewingConeChanged(viewer);
                InvokeOnVectorLayerChanged();
            }
        }

        private async void Viewer_ViewChange(object sender, IEventArgs<IOrientation> e)
        {
            if (sender != null && sender is IPanoramaViewer && StreetSmartAPI != null)
            {
                var viewer = sender as IPanoramaViewer;
                var cone = await CreateCone(viewer);

                await InvokeOnViewingConeChanged(viewer);
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

            OnSelectedFeatureChanged?.Invoke(null);

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

        public async Task DeselectAll()
        {
            if (StreetSmartAPI != null)
            {
                var viewers = await StreetSmartAPI.GetViewers();
                foreach (var viewer in viewers)
                {
                    var panoramaViewer = (IPanoramaViewer)viewer;
                    var json = JsonFactory.Create(new Dictionary<string, string>());
                    foreach (var vectorLayer in _vectorLayers)
                    {
                        if (vectorLayer.Overlay != null)
                            panoramaViewer.SetSelectedFeatureByProperties(json, vectorLayer.Overlay.Id);
                    }

                }
            }
        }

        public async Task Select(ESRI.ArcGIS.Geodatabase.IFeature feature)
        {
            var extension = StreetSmartExtension.GetExtension();
            if (!extension.CommunicatingWithStreetSmart && StreetSmartAPI != null && feature != null && feature.HasOID)
            {
                extension.CommunicatingWithStreetSmart = true;
                try
                {
                    VectorLayer layer = VectorLayer.GetLayer(feature);
                    if (layer != null)
                    {
                        VectorLayer vectorLayer = _vectorLayers.FirstOrDefault(v => v.Name == layer.Name);
                        if (vectorLayer != null && AddVectorInChange(vectorLayer))
                        {
                            var props = new Dictionary<string, string>();
                            var fields = feature.Fields;
                            for (int i = 0; i < fields.FieldCount; i++)
                            {
                                var f = fields.Field[i];
                                if (!f.Name.StartsWith("SHAPE", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    props.Add(f.Name, feature.Value[i]?.ToString());
                                }
                            }
                            var json = JsonFactory.Create(props);
                            var viewers = await StreetSmartAPI.GetViewers();
                            foreach (var viewer in viewers)
                            {
                                var panoramaViewer = (IPanoramaViewer)viewer;
                                panoramaViewer.SetSelectedFeatureByProperties(json, vectorLayer.Overlay.Id);
                            }
                            RemoveVectorInchange(vectorLayer);
                        }
                    }
                }
                finally
                {
                    extension.CommunicatingWithStreetSmart = false;
                }
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

        private async void VectorLayer_StartMeasurementEvent(TypeOfLayer typeOfLayer)
        {
            if (GlobeSpotterConfiguration.MeasurePermissions)
            {
                await CreateMeasurement(typeOfLayer);
            }
        }

        public static IFeature ConvertFeature(ESRI.ArcGIS.Geodatabase.IFeature feature)
        {
            int count = 0;

            if (feature != null && feature.Shape != null)
            {
                return ConvertFeature(feature.Shape, ref count);
            }

            return null;
        }

        public static IFeature ConvertFeature(ESRI.ArcGIS.Geometry.IGeometry geometry, ref int count)
        {
            if (geometry != null)
            {
                var type = VectorLayer.GetTypeOfLayer(geometry.GeometryType);

                switch (type)
                {
                    case TypeOfLayer.Point:
                        return GeoJsonFactory.CreatePointFeature(ToCoordinate(geometry as ESRI.ArcGIS.Geometry.IPoint, ref count));
                    case TypeOfLayer.Line:
                        return GeoJsonFactory.CreateLineFeature(ToCoordinate(geometry as ESRI.ArcGIS.Geometry.Polyline, ref count));
                    case TypeOfLayer.Polygon:
                        return GeoJsonFactory.CreatePolygonFeature(ToCoordinate(geometry as ESRI.ArcGIS.Geometry.Polygon, ref count));
                    default:
                        throw new NotImplementedException();
                }
            }

            return null;
        }

        public static StreetSmart.Common.Interfaces.GeoJson.IGeometry ConvertGeometry(ESRI.ArcGIS.Geometry.IGeometry geometry, ref int count)
        {
            if (geometry != null)
            {
                var type = VectorLayer.GetTypeOfLayer(geometry.GeometryType);

                switch (type)
                {
                    case TypeOfLayer.Point:
                        return GeoJsonFactory.CreatePointGeometry(ToCoordinate(geometry as ESRI.ArcGIS.Geometry.IPoint, ref count));
                    case TypeOfLayer.Line:
                        return GeoJsonFactory.CreateLineGeometry(ToCoordinate(geometry as ESRI.ArcGIS.Geometry.Polyline, ref count));
                    case TypeOfLayer.Polygon:
                        return GeoJsonFactory.CreatePolygonGeometry(ToCoordinate(geometry as ESRI.ArcGIS.Geometry.Polygon, ref count));
                    default:
                        return null;
                }
            }

            return null;
        }

        public static ICoordinate ToCoordinate(ESRI.ArcGIS.Geometry.IPoint point, ref int count)
        {
            if (point != null)
            {
                count++;

                return CoordinateFactory.Create(point.X, point.Y, point.Z);
            }

            return null;
        }

        public static List<ICoordinate> ToCoordinate(Polyline polyline, ref int count)
        {
            List<ICoordinate> coordinates = new List<ICoordinate>();

            for (int i = 0; i < polyline.PointCount; i++)
            {
                var coordinate = ToCoordinate(polyline.Point[i], ref count);

                if (coordinate != null)
                    coordinates.Add(coordinate);
            }

            return coordinates;
        }

        public static IList<IList<ICoordinate>> ToCoordinate(Polygon polygon, ref int count)
        {
            List<ICoordinate> coordinates = new List<ICoordinate>();

            for (int i = 0; i < polygon.PointCount; i++)
            {
                var coordinate = ToCoordinate(polygon.Point[i], ref count);

                if (coordinate != null)
                    coordinates.Add(coordinate);
            }

            return new List<IList<ICoordinate>> { coordinates };
        }

        //public static ESRI.ArcGIS.Geometry.IPoint ToPoint(ICoordinate coordinate)
        //{
        //    if (coordinate != null && coordinate.X.HasValue && coordinate.Y.HasValue)
        //        return new ESRI.ArcGIS.Geometry.Point
        //        {
        //            X = coordinate.X ?? 0,
        //            Y = coordinate.Y ?? 0,
        //            Z = coordinate.Z ?? 0,
        //            SpatialReference = new SpatialReferenceEnvironmentClass().CreateSpatialReference(Configuration.Configuration.Instance.ApiSSRAsInt),
        //        };
        //    else
        //        return null;
        //}

        //public static Polyline ToPolyline(List<ICoordinate> coordinates)
        //{
        //    if (coordinates != null)
        //    {
        //        var result = new PolylineClass
        //        {
        //            SpatialReference = new SpatialReferenceEnvironmentClass().CreateSpatialReference(Configuration.Configuration.Instance.ApiSSRAsInt)
        //        };

        //        foreach (var coordinate in coordinates)
        //        {
        //            var point = ToPoint(coordinate);

        //            result.AddPoint(point);
        //        }

        //        return result;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public static Polygon ToPolygon(IList<IList<ICoordinate>> coordinates)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion public functions
    }
}