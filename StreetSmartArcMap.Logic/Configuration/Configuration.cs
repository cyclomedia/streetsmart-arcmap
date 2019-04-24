using StreetSmartArcMap.Logic.Utilities;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using SystemIOFile = System.IO.File;

namespace StreetSmartArcMap.Logic.Configuration
{
    [XmlRoot("Configuration")]
    public class Configuration : INotifyPropertyChanged, IStreetSmartOptions
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Members

        private static readonly XmlSerializer XmlConfiguration;
        private static Configuration _configuration;

        public string AddressLocale { get; set; }
        public string AddressDatabase { get; set; }
        public string AddressDefaultQuery { get; set; }

        public string ApiSRS { get; set; }
        public string ApiUsername { get; set; }
        public string ApiPassword { get; set; }
        public string ApiKey { get; set; }

        public string BaseUrl { get; set; }
        public string RecordingsServiceUrl { get; set; }
        public string SpatialReferencesUrl { get; set; }
        public string SwfUrl { get; set; }

        public string DefaultRecordingSrs { get; set; }

        #endregion

        #region Constructors

        static Configuration()
        {
            XmlConfiguration = new XmlSerializer(typeof(Configuration));
        }

        #endregion

        #region Properties

        public static Configuration Instance
        {
            get
            {
                if (_configuration == null)
                {
                    Load();
                }

                return _configuration ?? (_configuration = Create());
            }
        }

        private static string FileName => Path.Combine(FileUtils.FileDir, "Configuration.xml");

        #endregion

        #region Functions

        public void Save()
        {
            OnPropertyChanged();
            FileStream streamFile = SystemIOFile.Open(FileName, FileMode.Create);
            XmlConfiguration.Serialize(streamFile, this);
            streamFile.Close();
        }

        private static void Load()
        {
            if (SystemIOFile.Exists(FileName))
            {
                var streamFile = new FileStream(FileName, FileMode.OpenOrCreate);
                _configuration = (Configuration)XmlConfiguration.Deserialize(streamFile);
                streamFile.Close();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static Configuration Create()
        {
            var result = new Configuration
            {
                AddressLocale = string.Empty,
                AddressDatabase = string.Empty,
                AddressDefaultQuery = string.Empty,

                ApiSRS = string.Empty,
                ApiUsername = string.Empty,
                ApiPassword = string.Empty,
                ApiKey = string.Empty,

                BaseUrl = "https://atlas.cyclomedia.com",
                RecordingsServiceUrl = "https://atlas.cyclomedia.com/recordings/wfs",
                SpatialReferencesUrl = "https://streetsmart.cyclomedia.com/api/v18.10/assets/srs/SpatialReference.xml", // TODO: is this the actual URL?
                SwfUrl = string.Empty,

                DefaultRecordingSrs = string.Empty
            };

            result.Save();
            return result;
        }

        #endregion
    }
}
