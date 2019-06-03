using StreetSmartArcMap.Logic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StreetSmartArcMap.Client
{
    [XmlRoot("ConstantsViewer")]
    public class ConstantsViewer
    {
        #region Members

        private static readonly XmlSerializer XmlConstantsViewer;
        private static ConstantsViewer _constantsViewer;

        #endregion

        #region Constructors

        static ConstantsViewer()
        {
            XmlConstantsViewer = new XmlSerializer(typeof(ConstantsViewer));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Address language code
        /// </summary>
        public string AddressLanguageCode { get; set; }

        /// <summary>
        /// Address Database
        /// </summary>
        public string AddressDatabase { get; set; }

        /// <summary>
        /// Size of the Cross Check
        /// </summary>
        public double CrossCheckSize { get; set; }

        /// <summary>
        /// Size of the measurement point
        /// </summary>
        public double MeasurementPointSize { get; set; }

        /// <summary>
        /// Size of the measurement font
        /// </summary>
        public float MeasurementFontSize { get; set; }

        public static ConstantsViewer Instance
        {
            get
            {
                if (_constantsViewer == null)
                {
                    Load();
                }

                return _constantsViewer ?? (_constantsViewer = Create());
            }
        }

        private static string FileName => Path.Combine(FileUtils.FileDir, "ConstantsViewer.xml");

        #endregion

        #region Functions

        public void Save()
        {
            using (var output = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                XmlConstantsViewer.Serialize(output, this);
            }
        }

        private static void Load()
        {
            if (File.Exists(FileName))
            {
                using (var input = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    _constantsViewer = (ConstantsViewer)XmlConstantsViewer.Deserialize(input);
                }
            }
        }

        private static ConstantsViewer Create()
        {
            var result = new ConstantsViewer
            {
                AddressLanguageCode = "nl",
                AddressDatabase = "CMDatabase",
                CrossCheckSize = 10.0,
                MeasurementPointSize = 5.0,
                MeasurementFontSize = 8.0f,
            };

            result.Save();

            return result;
        }

        #endregion
    }
}
