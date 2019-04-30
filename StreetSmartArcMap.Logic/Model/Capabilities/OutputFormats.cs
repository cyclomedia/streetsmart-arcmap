using System.Xml.Linq;

namespace StreetSmartArcMap.Logic.Model.Capabilities
{
    public class OutputFormats
    {
        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public string Format { get; set; }

        #endregion

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        /// <summary>
        /// Default empty constructor
        /// </summary>
        public OutputFormats()
        {
            // empty
        }

        /// <summary>
        /// Constructor with xml parsing
        /// </summary>
        /// <param name="element">xml</param>
        public OutputFormats(XElement element)
        {
            Update(element);
        }

        #endregion

        #region functions

        // =========================================================================
        // Functions
        // =========================================================================
        /// <summary>
        /// xml parsing
        /// </summary>
        /// <param name="element">xml</param>
        public void Update(XElement element)
        {
            XElement outputformatElement = element.Element(Namespaces.WfsNs + "OutputFormats");

            if (outputformatElement != null)
            {
                XElement formatElement = outputformatElement.Element(Namespaces.WfsNs + "Format");

                if (formatElement != null)
                {
                    Format = formatElement.Value;
                }
            }
        }

        #endregion
    }
}