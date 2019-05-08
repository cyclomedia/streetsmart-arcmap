using StreetSmart.Common.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetSmartArcMap.Logic.Model
{
    public class ViewingCone
    {
        public string ViewerId { get; set; }
        public string ImageId { get; set; }
        public ICoordinate Coordinate { get; set; }
        public IOrientation Orientation { get; set; }
        public Color Color { get; set; }
    }
}
