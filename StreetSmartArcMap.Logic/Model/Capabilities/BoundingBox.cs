using System.Xml.Linq;

namespace StreetSmartArcMap.Logic.Model.Capabilities
{
    public class BBoundingBox
    {
        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public Point LowerCorner { get; set; }

        public Point UpperCorner { get; set; }

        #endregion properties

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        /// <summary>
        /// Default empty constructor
        /// </summary>
        public BBoundingBox()
        {
            // empty
        }

        /// <summary>
        /// Constructor with xml parsing
        /// </summary>
        /// <param name="element">xml</param>
        public BBoundingBox(XElement element)
        {
            Update(element);
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
            XElement wgs84BoundingBoxElement = element.Element(Namespaces.OwsNs + "WGS84BoundingBox");

            if (wgs84BoundingBoxElement != null)
            {
                XElement lowerCornerElement = wgs84BoundingBoxElement.Element(Namespaces.OwsNs + "LowerCorner");
                XElement upperCornerElement = wgs84BoundingBoxElement.Element(Namespaces.OwsNs + "UpperCorner");
                LowerCorner = new Point(lowerCornerElement);
                UpperCorner = new Point(upperCornerElement);
            }
        }

        #endregion functions
    }
}