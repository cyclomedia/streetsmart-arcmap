using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetSmartArcMap
{
    public class MeasuremenChangeEventArgs : EventArgs
    {
        public IGeometry Geometry { get; set; }
    }
}
