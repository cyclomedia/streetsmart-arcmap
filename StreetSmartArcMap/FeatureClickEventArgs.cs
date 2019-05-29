using StreetSmart.Common.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetSmartArcMap
{
    public class FeatureClickEventArgs: EventArgs
    {
        public string ViewerId;
        public IFeatureInfo FeatureInfo;
    }
}
