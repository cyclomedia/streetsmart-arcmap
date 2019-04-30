using System.Collections.Generic;
using System.Xml.Linq;

namespace StreetSmartArcMap.Logic.Model.Capabilities
{
    public class FeatureType
    {
        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public Name Name { get; set; }

        public string Title { get; set; }
        public string DefaultSrs { get; set; }

        public List<string> OtherSrs { get; set; }
        public OutputFormats OutputFormats { get; set; }
        public BBoundingBox BBoundingBox { get; set; }

        public static XName TypeName
        {
            get { return (Namespaces.WfsNs + "FeatureType"); }
        }

        #endregion properties

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        /// <summary>
        /// Default empty constructor
        /// </summary>
        public FeatureType()
        {
            // empty
        }

        /// <summary>
        /// Constructor with xml parsing
        /// </summary>
        /// <param name="mappedFeatureElement">xml</param>
        public FeatureType(XElement mappedFeatureElement)
        {
            Update(mappedFeatureElement);
        }

        #endregion constructor

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
            if (element != null)
            {
                Name = new Name(element);
                XElement titleElement = element.Element(Namespaces.WfsNs + "Title");
                XElement defaultSrsElement = element.Element(Namespaces.WfsNs + "DefaultSRS");
                var otherSrsElements = element.Elements(Namespaces.WfsNs + "OtherSRS");
                OutputFormats = new OutputFormats(element);
                BBoundingBox = new BBoundingBox(element);

                Title = (titleElement == null) ? null : titleElement.Value.Trim();
                DefaultSrs = (defaultSrsElement == null) ? null : defaultSrsElement.Value.Trim();
                OtherSrs = new List<string>();

                foreach (var otherSrsElement in otherSrsElements)
                {
                    if (otherSrsElement != null)
                    {
                        OtherSrs.Add(otherSrsElement.Value.Trim());
                    }
                }
            }
        }

        #endregion functions
    }
}