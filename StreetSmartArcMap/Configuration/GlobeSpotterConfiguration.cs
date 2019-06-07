using StreetSmartArcMap.Client;
using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace StreetSmartArcMap.Configuration
{
    [XmlType(AnonymousType = true, Namespace = "https://www.globespotter.com/gsc")]
    [XmlRoot(Namespace = "https://www.globespotter.com/gsc", IsNullable = false)]
    public class GlobeSpotterConfiguration
    {
        #region Members

        private static readonly XmlSerializer XmlstreetSmartconfiguration;
        private static readonly Web Web;

        private static GlobeSpotterConfiguration _GlobeSpotterConfiguration;

        #endregion

        #region Constructors

        static GlobeSpotterConfiguration()
        {
            XmlstreetSmartconfiguration = new XmlSerializer(typeof(GlobeSpotterConfiguration));
            Web = Web.Instance;
        }

        #endregion

        #region Properties

        /// <summary>
        /// ApplicationConfiguration
        /// </summary>
        public ApplicationConfiguration ApplicationConfiguration { get; set; }

        [XmlIgnore]
        public bool LoginFailed { get; private set; }

        [XmlIgnore]
        public bool LoadException { get; private set; }

        [XmlIgnore]
        public Exception Exception { get; private set; }

        [XmlIgnore]
        public bool Credentials => ApplicationConfiguration != null && ApplicationConfiguration.Functionalities.Length >= 1;

        public static bool MeasureSmartClick => Instance.CheckFunctionality("MeasureSmartClick");

        public static bool MeasurePoint => Instance.CheckFunctionality("MeasurePoint");

        public static bool MeasureLine => Instance.CheckFunctionality("MeasureLine");

        public static bool MeasurePolygon => Instance.CheckFunctionality("MeasurePolygon");

        public static bool AddLayerWfs => Instance.CheckFunctionality("AddLayerWFS");

        public static bool MeasurePermissions => MeasurePoint || MeasureLine || MeasurePolygon || MeasureSmartClick;

        public static GlobeSpotterConfiguration Instance
        {
            get
            {
                bool loadException = false;
                bool loginFailed = false;
                Exception exception = null;

                if (_GlobeSpotterConfiguration == null)
                {
                    try
                    {
                        Load();
                    }
                    catch (WebException ex)
                    {
                        exception = ex;

                        if (ex.Response is HttpWebResponse)
                        {
                            HttpWebResponse response = ex.Response as HttpWebResponse;

                            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.NotFound)
                                loginFailed = true;
                            else
                                loadException = true;
                        }
                        else
                        {
                            loadException = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        loadException = true;
                    }
                }

                return _GlobeSpotterConfiguration ?? (_GlobeSpotterConfiguration = new GlobeSpotterConfiguration
                {
                    LoadException = loadException,
                    LoginFailed = loginFailed,
                    Exception = exception
                });
            }
        }

        #endregion

        #region Functions

        private bool CheckFunctionality(string name)
        {
            return ApplicationConfiguration?.GetFunctionality(name) != null;
        }

        public static GlobeSpotterConfiguration Load()
        {
            try
            {
                using (Stream input = Web.DownloadGlobeSpotterConfiguration())
                {
                    if (input != null)
                    {
                        input.Position = 0;
                        _GlobeSpotterConfiguration = (GlobeSpotterConfiguration)XmlstreetSmartconfiguration.Deserialize(input);
                    }
                }
            }
            catch
            {
                // ignored
            }

            return _GlobeSpotterConfiguration;
        }

        public static void Delete()
        {
            _GlobeSpotterConfiguration = null;
        }

        public static bool CheckCredentials()
        {
            Delete();
            return Instance.Credentials;
        }

        #endregion
    }
}
