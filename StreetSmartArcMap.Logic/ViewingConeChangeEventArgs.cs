using StreetSmartArcMap.Logic.Model;
using System;

namespace StreetSmartArcMap.Logic
{
    public class ViewingConeChangeEventArgs : EventArgs
    {
        public string ViewerId;
        public ViewingCone ViewingCone;
    }
}