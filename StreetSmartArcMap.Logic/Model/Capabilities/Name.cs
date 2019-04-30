using System.Xml.Linq;

namespace StreetSmartArcMap.Logic.Model.Capabilities
{
    public class Name
    {
        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        public XNamespace Namespace { get; set; }

        public string Value { get; set; }
        public string Type { get; set; }

        public XName XName
        {
            get { return (Namespace + Type); }
        }

        #endregion properties

        #region constructor

        // =========================================================================
        // Constructor
        // =========================================================================
        /// <summary>
        /// Default empty constructor
        /// </summary>
        public Name()
        {
            // empty
        }

        /// <summary>
        /// Constructor with xml parsing
        /// </summary>
        /// <param name="element">xml</param>
        public Name(XElement element)
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
            XElement name = element.Element(Namespaces.WfsNs + "Name");

            if (name != null)
            {
                Value = name.Value;
                string[] valueSplit = Value.Split(':');

                if (valueSplit.Length >= 2)
                {
                    string namespaceName = valueSplit[0].Trim();
                    Namespace = name.GetNamespaceOfPrefix(namespaceName);
                    Type = valueSplit[1].Trim();
                }
            }
        }

        #endregion functions
    }
}