using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetSmartArcMap.Logic
{
    public class StreetSmartOptions : IStreetSmartOptions
    {
        public string EpsgCode { get; set; }
        public string Locale { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
    }
}
