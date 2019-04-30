using System.Globalization;
using System.Xml.Linq;

namespace StreetSmartArcMap.Logic.Model.Capabilities
{
    public class Point
    {
        #region members

        // =========================================================================
        // Members
        // =========================================================================
        private readonly CultureInfo _ci;

        #endregion

        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public double X { get; set; }
        public double Y { get; set; }

        #endregion

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        /// <summary>
        /// Default empty constructor
        /// </summary>
        public Point()
        {
            _ci = CultureInfo.InvariantCulture;
            X = 0.0;
            Y = 0.0;
        }

        /// <summary>
        /// Constructor with xml parsing
        /// </summary>
        /// <param name="element">xml</param>
        public Point(XElement element)
        {
            _ci = CultureInfo.InvariantCulture;
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
            if (element != null)
            {
                string position = element.Value.Trim();
                string[] values = position.Split(' ');
                X = (values.Length >= 1) ? double.Parse(values[0], _ci) : 0.0;
                Y = (values.Length >= 2) ? double.Parse(values[1], _ci) : 0.0;
            }
        }

        #endregion
    }
}