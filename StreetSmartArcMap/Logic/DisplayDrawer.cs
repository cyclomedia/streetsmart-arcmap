using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using StreetSmart.Common.Interfaces.Data;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetSmartArcMap.Logic
{
    public class DisplayDrawer : IDisposable
    {
        public IScreenDisplay Display { get; private set; }

        public DisplayDrawer()
        {
            Display = ArcUtils.ActiveView?.ScreenDisplay;

            Display?.StartDrawing(Display.hDC, (short)esriScreenCache.esriNoScreenCache);
        }

        public void DrawPoint(ICoordinate coordinate)
        {
            var point = CreatePoint(coordinate);

            if (point != null)
            {
                //TODO: Measurement - Draw point to map
                //Display.SetSymbol();
                Display.DrawPoint(point);
            }
        }

        public IPoint CreatePoint(ICoordinate coordinate)
        {
            if (coordinate != null && coordinate.X.HasValue && coordinate.Y.HasValue)
                return Display.DisplayTransformation.ToMapPoint((int)coordinate.X.Value, (int)coordinate.Y.Value);
            else
                return null;

            //if (coordinate != null && coordinate.X.HasValue && coordinate.Y.HasValue)
            //    return new Point
            //    {
            //        X = coordinate.X ?? 0,
            //        Y = coordinate.Y ?? 0,
            //        Z = coordinate.Z ?? 0,
            //        SpatialReference = new SpatialReferenceEnvironmentClass().CreateSpatialReference(Configuration.Configuration.Instance.ApiSSRAsInt),
            //    };
            //else
            //    return null;
        }

        public void Dispose()
        {
            Display?.FinishDrawing();
        }
    }
}
