using ESRI.ArcGIS.Geometry;
using StreetSmart.Common.Interfaces.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetSmartArcMap
{
    public class MeasuremenChangeEventArgs : EventArgs
    {
        public IFeatureCollection Features { get; set; }
    }
}
