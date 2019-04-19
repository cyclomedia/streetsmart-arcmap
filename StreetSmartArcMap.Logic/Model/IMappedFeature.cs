using System.Collections.Generic;
using System.Xml.Linq;
using StreetSmartArcMap.Logic.Model.Shape;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace StreetSmartArcMap.Logic.Model
{
    /// <summary>
    /// This is the interface for feature information
    /// </summary>
    public interface IMappedFeature
    {
        #region properties

        // =========================================================================
        // Properties
        // =========================================================================
        Dictionary<string, esriFieldType> Fields { get; }
        string ObjectId { get; }
        XName Name { get; }
        string ShapeFieldName { get; }
        esriGeometryType EsriGeometryType { get; }
        IShape Shape { get; }

        #endregion

        #region functions

        // =========================================================================
        // Functions
        // =========================================================================
        object FieldToItem(string name);
        void UpdateItem(string name, object item);
        void Update(XElement mappedFeatureElement);

        #endregion
    }
}
