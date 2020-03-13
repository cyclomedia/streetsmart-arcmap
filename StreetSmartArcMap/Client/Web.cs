/*
 * Integration in ArcMap for StreetSmart
 * Copyright (c) 2019 - 2020, CycloMedia, All rights reserved.
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

using ESRI.ArcGIS.Geometry;
using StreetSmartArcMap.Layers;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Logic.Model;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace StreetSmartArcMap.Client
{
    public class Web
    {
        #region constants

        // =========================================================================
        // Constants
        // =========================================================================
        private const string RecordingRequest = "{0}?service=WFS&version=1.1.0&request=GetFeature&srsname={1}&featureid={2}&TYPENAME=atlas:Recording";

        private const string WfsBboxRequest = "{0}?SERVICE=WFS&VERSION={1}&REQUEST=GetFeature&SRSNAME={2}&BBOX={3},{4},{5},{6},{7}&TYPENAME={8}";
        private const string CapabilityString = "{0}?REQUEST=GetCapabilities&VERSION={1}&SERVICE=WFS";
        private const string AuthorizationRequest = "{0}/configuration/configuration/API";

        private const int BufferImageLengthService = 2048;
        private const int XmlConfig = 0;
        private const int DownloadImageConfig = 1;
        private const int LeaseTimeOut = 5000;
        private const int DefaultConnectionLimit = 5;

        #endregion constants

        #region members

        // =========================================================================
        // Members
        // =========================================================================
        private readonly int[] _waitTimeInternalServerError = { 5000, 0 };

        private readonly int[] _timeOutService = { 3000, 1000 };
        private readonly int[] _retryTimeService = { 3, 1 };

        private static Web _web;

        private readonly CultureInfo _ci;
        private readonly Login _login;
        private readonly Configuration.Configuration _config;

        #endregion members

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================

        public Configuration.Configuration Config => Configuration.Configuration.Instance;

        public static Web Instance => _web ?? (_web = new Web());

        #endregion properties

        #region Constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        private Web()
        {
            _ci = CultureInfo.InvariantCulture;
            _login = Login.Instance;
            _config = Configuration.Configuration.Instance;
            //_apiKey = APIKey.Instance;
            ServicePointManager.DefaultConnectionLimit = DefaultConnectionLimit;
        }

        #endregion Constructor

        #region interface functions

        // =========================================================================
        // Interface functions
        // =========================================================================
        public List<XElement> GetByImageId(string imageId, CycloMediaLayer cycloMediaLayer)
        {
            string epsgCode = cycloMediaLayer.EpsgCode;
            epsgCode = SpatialReferences.Instance.ToKnownSrsName(epsgCode);
            string remoteLocation = string.Format(RecordingRequest, Config.RecordingsServiceUrlToUse, epsgCode, imageId);
            var xml = (string)GetRequest(remoteLocation, GetXmlCallback, XmlConfig);
            return ParseXml(xml, (Namespaces.GmlNs + "featureMembers"));
        }

        public List<XElement> GetByBbox(IEnvelope envelope, CycloMediaLayer cycloMediaLayer)
        {
            string epsgCode = cycloMediaLayer.EpsgCode;
            epsgCode = SpatialReferences.Instance.ToKnownSrsName(epsgCode);
            List<XElement> result;

            if (cycloMediaLayer is WfsLayer)
            {
                var wfsLayer = cycloMediaLayer as WfsLayer;
                string remoteLocation = string.Format(_ci, WfsBboxRequest, wfsLayer.Url, wfsLayer.Version, epsgCode,
                                                      envelope.XMin, envelope.YMin, envelope.XMax, envelope.YMax,
                                                      epsgCode, wfsLayer.TypeName);
                var xml = (string)GetRequest(remoteLocation, GetXmlCallback, XmlConfig);
                result = ParseXml(xml, (Namespaces.GmlNs + "featureMember"));
            }
            else
            {
                string postItem = string.Format(_ci, cycloMediaLayer.WfsRequest, epsgCode, envelope.XMin, envelope.YMin,
                                                envelope.XMax,
                                                envelope.YMax, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00-00:00"));
                var xml = (string)PostRequest(Config.RecordingsServiceUrlToUse, GetXmlCallback, postItem, XmlConfig);
                result = ParseXml(xml, (Namespaces.GmlNs + "featureMembers"));
            }

            return result;
        }

        public List<XElement> GetByBbox(IEnvelope envelope, string wfsRequest)
        {
            string epsgCode = ArcUtils.EpsgCode;
            string postItem = string.Format(_ci, wfsRequest, epsgCode, envelope.XMin, envelope.YMin, envelope.XMax,
                                            envelope.YMax, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00-00:00"));
            var xml = (string)PostRequest(Config.RecordingsServiceUrlToUse, GetXmlCallback, postItem, XmlConfig);
            return ParseXml(xml, (Namespaces.GmlNs + "featureMembers"));
        }

        public List<XElement> GetCapabilities(string wfsService, string version)
        {
            string remotelocation = string.Format(CapabilityString, wfsService, version);
            var xml = (string)GetRequest(remotelocation, GetXmlCallback, XmlConfig);
            return ParseXml(xml, (Namespaces.WfsNs + "FeatureTypeList"));
        }

        public Image DownloadImage(string url)
        {
            var imageStream = GetRequest(url, GetStreamCallback, DownloadImageConfig) as Stream;
            return (imageStream == null) ? null : Image.FromStream(imageStream);
        }

        public Stream DownloadSpatialReferences()
        {
            return GetRequest(Config.SpatialReferencesUrlToUse, GetStreamCallback, XmlConfig) as Stream;
        }

        public Stream DownloadGlobeSpotterConfiguration()
        {
            const string postItem = @"<Authorization />";
            string authorizationService = string.Format(AuthorizationRequest, Config.BaseUrlToUse);

            return PostRequest(authorizationService, GetStreamCallback, postItem, XmlConfig) as Stream;
        }

        #endregion interface functions

        #region parse XML

        // =========================================================================
        // Parse XML
        // =========================================================================
        private static List<XElement> ParseXml(string xml, XName xName)
        {
            var stringReader = new StringReader(xml);
            var xmlTextReader = new XmlTextReader(stringReader);
            XDocument xmlDoc = XDocument.Load(xmlTextReader);
            IEnumerable<XElement> elements = xmlDoc.Descendants(xName);
            return elements.ToList();
        }

        #endregion parse XML

        #region wfs request functions

        // =========================================================================
        // wfs request functions
        // =========================================================================
        private object GetRequest(string remoteLocation, AsyncCallback asyncCallback, int configId)
        {
            object result = null;
            bool download = false;
            int retry = 0;
            WebRequest request = OpenWebRequest(remoteLocation, WebRequestMethods.Http.Get, 0);
            var state = new State { Request = request };

            while ((download == false) && (retry < _retryTimeService[configId]))
            {
                try
                {
                    lock (this)
                    {
                        ManualResetEvent waitObject = state.OperationComplete;
                        request.BeginGetResponse(asyncCallback, state);

                        if (!waitObject.WaitOne(_timeOutService[configId]))
                        {
                            throw new Exception("Time out download item");
                        }

                        if (state.OperationException != null)
                        {
                            throw state.OperationException;
                        }

                        result = state.Result;
                        download = true;
                    }
                }
                catch (WebException ex)
                {
                    retry++;
                    var responce = ex.Response as HttpWebResponse;

                    if (responce != null)
                    {
                        if ((responce.StatusCode == HttpStatusCode.InternalServerError) && (retry < _retryTimeService[configId]))
                        {
                            Thread.Sleep(_waitTimeInternalServerError[configId]);
                        }
                    }

                    if (retry == _retryTimeService[configId])
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    retry++;

                    if (retry == _retryTimeService[configId])
                    {
                        throw;
                    }
                }
            }

            return result;
        }

        private object PostRequest(string remoteLocation, AsyncCallback asyncCallback, string postItem, int configId)
        {
            object result = null;
            bool download = false;
            int retry = 0;
            var bytes = (new UTF8Encoding()).GetBytes(postItem);
            WebRequest request = OpenWebRequest(remoteLocation, WebRequestMethods.Http.Post, bytes.Length);
            var state = new State { Request = request };

            lock (this)
            {
                using (Stream reqstream = request.GetRequestStream())
                {
                    reqstream.Write(bytes, 0, bytes.Length);
                }
            }

            while ((download == false) && (retry < _retryTimeService[configId]))
            {
                try
                {
                    lock (this)
                    {
                        ManualResetEvent waitObject = state.OperationComplete;
                        request.BeginGetResponse(asyncCallback, state);

                        if (!waitObject.WaitOne(_timeOutService[configId]))
                        {
                            throw new Exception("Time out download item.");
                        }

                        if (state.OperationException != null)
                        {
                            throw state.OperationException;
                        }

                        result = state.Result;
                        download = true;
                    }
                }
                catch (WebException ex)
                {
                    retry++;
                    var responce = ex.Response as HttpWebResponse;

                    if (responce != null)
                    {
                        Uri responseUri = responce.ResponseUri;

                        if (responseUri != null)
                        {
                            string absoluteUri = responseUri.AbsoluteUri;
                            // string absolutePath = responseUri.AbsolutePath;
                            // string host = absoluteUri.Remove(absoluteUri.IndexOf(absolutePath));

                            if (absoluteUri != remoteLocation)
                            {
                                remoteLocation = absoluteUri;
                                // Urls.BaseUrl = host;
                                request = OpenWebRequest(remoteLocation, WebRequestMethods.Http.Post, bytes.Length);
                                state = new State { Request = request };

                                lock (this)
                                {
                                    using (Stream reqstream = request.GetRequestStream())
                                    {
                                        reqstream.Write(bytes, 0, bytes.Length);
                                    }
                                }
                            }
                        }

                        if ((responce.StatusCode == HttpStatusCode.InternalServerError) && (retry < _retryTimeService[configId]))
                        {
                            Thread.Sleep(_waitTimeInternalServerError[configId]);
                        }
                    }

                    if (retry == _retryTimeService[configId])
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    retry++;

                    if (retry == _retryTimeService[configId])
                    {
                        throw;
                    }
                }
            }

            return result;
        }

        private WebRequest OpenWebRequest(string remoteLocation, string webRequest, int length)
        {
            try
            {
                IWebProxy proxy;

                if (Config.UseProxyServer)
                {
                    var webProxy = new WebProxy(Config.ProxyAddress, Config.ProxyPort)
                    {
                        BypassProxyOnLocal = Config.BypassProxyOnLocal,
                        UseDefaultCredentials = Config.ProxyUseDefaultCredentials
                    };

                    if (!Config.ProxyUseDefaultCredentials)
                    {
                        webProxy.Credentials = new NetworkCredential(Config.ProxyUsername, Config.ProxyPassword, Config.ProxyDomain);
                    }

                    proxy = webProxy;
                }
                else
                {
                    proxy = WebRequest.GetSystemWebProxy();
                }

                var request = (HttpWebRequest)WebRequest.Create(remoteLocation);
                request.Credentials = new NetworkCredential(_login.Username, _login.Password);
                request.Method = webRequest;
                request.ContentLength = length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.Proxy = proxy;
                request.PreAuthenticate = true;
                request.ContentType = "text/xml";
                request.Headers.Add("ApiKey", Configuration.Configuration.ApiKey);

                if (request.ServicePoint != null)
                {
                    request.ServicePoint.ConnectionLeaseTimeout = LeaseTimeOut;
                    request.ServicePoint.MaxIdleTime = LeaseTimeOut;
                }

                return request;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to open web request.", ex);
            }
        }

        #endregion wfs request functions

        #region call back functions

        // =========================================================================
        // Call back functions
        // =========================================================================
        private static void GetXmlCallback(IAsyncResult ar)
        {
            var state = (State)ar.AsyncState;

            try
            {
                var response = state.Request.EndGetResponse(ar);
                Stream responseStream = response.GetResponseStream();

                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream);
                    state.Result = reader.ReadToEnd();
                }

                response.Close();
                state.OperationComplete.Set();
            }
            catch (Exception e)
            {
                state.OperationException = e;
                state.OperationComplete.Set();
            }
        }

        private static void GetStreamCallback(IAsyncResult ar)
        {
            var state = (State)ar.AsyncState;

            try
            {
                var response = state.Request.EndGetResponse(ar);
                Stream responseStream = response.GetResponseStream();

                if (responseStream != null)
                {
                    var readFile = new BinaryReader(responseStream);
                    state.Result = new MemoryStream();
                    var writeFile = new BinaryWriter((Stream)state.Result);
                    var buffer = new byte[BufferImageLengthService];
                    int readBytes;

                    do
                    {
                        readBytes = readFile.Read(buffer, 0, BufferImageLengthService);
                        writeFile.Write(buffer, 0, readBytes);
                    } while (readBytes != 0);

                    writeFile.Flush();
                }

                response.Close();
                state.OperationComplete.Set();
            }
            catch (Exception e)
            {
                state.OperationException = e;
                state.OperationComplete.Set();
            }
        }

        #endregion call back functions
    }
}