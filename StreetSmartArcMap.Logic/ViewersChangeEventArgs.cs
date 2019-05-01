using System;

namespace StreetSmartArcMap.Logic
{
    public class ViewersChangeEventArgs: EventArgs
    {
        public int NumberOfViewers { get; set; }
    }
}