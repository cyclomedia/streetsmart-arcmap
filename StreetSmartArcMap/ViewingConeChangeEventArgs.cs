using StreetSmartArcMap.Logic.Model;
using StreetSmartArcMap.Objects;
using System;

namespace StreetSmartArcMap
{
    public class ViewingConeChangeEventArgs : EventArgs
    {
        public string ViewerId;
        public ViewingCone ViewingCone;
    }
}