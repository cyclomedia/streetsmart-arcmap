using StreetSmart.Common.Interfaces.API;
using System;
using System.Collections.Generic;

namespace StreetSmartArcMap
{
    public class ViewersChangeEventArgs: EventArgs
    {
        public IList<string> Viewers { get; set; }
    }
}