using ESRI.ArcGIS.ADF.Connection.Local;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using StreetSmart.Common.Interfaces.Data;
using StreetSmartArcMap.AddIns;
using StreetSmartArcMap.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using WinPoint = System.Drawing.Point;

namespace StreetSmartArcMap.Objects
{
    public class ViewingCone : IEquatable<ViewingCone>, IDisposable
    {
        #region Constants

        private const byte Alpha = 192;
        private const int MaxTimeUpdate = 100;
        private const int coneSize = 48; // was: const (value 96) / 2
        private bool disposing = false;

        #endregion Constants

        public string ViewerId { get; set; }
        public string ImageId { get; set; }
        public ICoordinate Coordinate { get; set; }
        public IOrientation Orientation { get; set; }
        public Color Color { get; set; }
        private bool _toUpdate { get; set; }
        private System.Threading.Timer _updateTimer { get; set; }

        public bool Equals(ViewingCone other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.ViewerId == other.ViewerId && this.ImageId == other.ImageId && this.Coordinate.X == other.Coordinate.X && this.Coordinate.Y == other.Coordinate.Y)
            {
                double epsilon = 1.0;
                // treat a small orientation change as equal
                if (Math.Abs(Orientation.HFov.Value - other.Orientation.HFov.Value) < epsilon && Math.Abs(Orientation.Yaw.Value - other.Orientation.Yaw.Value) < epsilon)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            disposing = true;
            Redraw();
        }

        public void Redraw()
        {
            StreetSmartExtension extension = StreetSmartExtension.GetExtension();
            if (extension.InsideScale())
            {
                if (_updateTimer == null)
                {
                    StartRedraw();
                }
                else
                {
                    _toUpdate = true;
                }
            }
        }

        public void AfterDraw(IDisplay Display, esriViewDrawPhase phase)
        {
            if (phase == esriViewDrawPhase.esriViewForeground)
            {
                var config = Configuration.Configuration.Instance;
                int srs = int.Parse(config.ApiSRS.Substring(config.ApiSRS.IndexOf(":") + 1));
                // empty views
                var extension = StreetSmartExtension.GetExtension();
                if (extension.InsideScale())
                {
                    var displayTransformation = Display.DisplayTransformation;
                    Display.Filter = new TransparencyDisplayFilterClass { Transparency = Alpha };
                    Display.StartDrawing(Display.hDC, (short)esriScreenCache.esriNoScreenCache);

                    if (!disposing)
                    {
                        ESRI.ArcGIS.Geometry.Point mappoint;
                        lock (Coordinate)
                        {
                            mappoint = new ESRI.ArcGIS.Geometry.Point()
                            {
                                X = Coordinate.X.Value,
                                Y = Coordinate.Y.Value,
                                Z = Coordinate.Z.Value,
                                SpatialReference = new SpatialReferenceEnvironmentClass().CreateSpatialReference(srs)
                            };
                        }
                        // Project the API SRS to the current map SRS.
                        mappoint.Project(ArcUtils.SpatialReference);

                        var symbol = new SimpleFillSymbol()
                        {
                            Color = Converter.ToRGBColor(Color),
                            Style = esriSimpleFillStyle.esriSFSSolid,
                            Outline = new SimpleLineSymbol()
                            {
                                Style = esriSimpleLineStyle.esriSLSNull
                            }
                        };

                        int screenX, screenY;
                        displayTransformation.FromMapPoint(mappoint, out screenX, out screenY);

                        double angleh = (Orientation.HFov ?? 0.0) * Math.PI / 360;
                        double angle = (270 + (Orientation.Yaw ?? 0.0)) % 360 * Math.PI / 180;
                        double angle1 = angle - angleh;
                        double angle2 = angle + angleh;

                        var screenPoint1 = new WinPoint(screenX + (int)(coneSize * Math.Cos(angle1)), screenY + (int)(coneSize * Math.Sin(angle1)));
                        var screenPoint2 = new WinPoint(screenX + (int)(coneSize * Math.Cos(angle2)), screenY + (int)(coneSize * Math.Sin(angle2)));
                        var point1 = displayTransformation.ToMapPoint(screenPoint1.X, screenPoint1.Y);
                        var point2 = displayTransformation.ToMapPoint(screenPoint2.X, screenPoint2.Y);

                        var polygon = new Polygon();
                        polygon.AddPoint(mappoint);
                        polygon.AddPoint(point1);
                        polygon.AddPoint(point2);

                        Display.SetSymbol((ISymbol)symbol);
                        Display.DrawPolygon((IGeometry)polygon);
                    }

                    Display.FinishDrawing();
                }
            }
        }

        private void Redraw(object state)
        {
            try
            {
                IActiveView activeView = ArcUtils.ActiveView;
                if (activeView != null)
                {
                    var display = activeView.ScreenDisplay;
                    display.Invalidate(activeView.Extent, true, (short)esriScreenCache.esriNoScreenCache);

                    //// We should calculate an envelope around this cone to invalidate only that extent, instead of everything on screen.

                    //var display = ArcUtils.ActiveView.ScreenDisplay;
                    //var displayTransformation = display.DisplayTransformation;
                    ////double size = displayTransformation.FromPoints(coneSize);
                    //var config = Configuration.Configuration.Instance;
                    //int srs = int.Parse(config.ApiSRS.Substring(config.ApiSRS.IndexOf(":") + 1));
                    //var apiSrs = new SpatialReferenceEnvironmentClass().CreateSpatialReference(srs);
                    //double x, y;
                    //ESRI.ArcGIS.Geometry.Point mappoint;
                    //lock (Coordinate)
                    //{
                    //    mappoint = new ESRI.ArcGIS.Geometry.Point()
                    //    {
                    //        X = Coordinate.X.Value,
                    //        Y = Coordinate.Y.Value,
                    //        Z = Coordinate.Z.Value,
                    //        SpatialReference = apiSrs
                    //    };
                    //}
                    //// Project the API SRS to the current map SRS.
                    ////mappoint.Project(ArcUtils.SpatialReference);
                    ////var test = mappoint.SpatialReference?.FactoryCode;

                    //int screenX, screenY;
                    //displayTransformation.FromMapPoint(mappoint, out screenX, out screenY);
                    //var size = (int)coneSize / 2;
                    //var screenBL = new WinPoint(screenX - coneSize, screenY - coneSize);
                    //var screenTR = new WinPoint(screenX + coneSize, screenY + coneSize);

                    //var bottomLeft = displayTransformation.ToMapPoint(screenBL.X, screenBL.Y);
                    //var topRight = displayTransformation.ToMapPoint(screenTR.X, screenTR.Y);

                    ////double xmin = mappoint.X - size;
                    ////double xmax = mappoint.X + size;
                    ////double ymin = mappoint.Y - size;
                    ////double ymax = mappoint.Y + size;

                    //IEnvelope envelope = new EnvelopeClass() { XMin = bottomLeft.X, XMax = topRight.X, YMin = bottomLeft.Y, YMax = topRight.Y, SpatialReference = apiSrs};
                    //activeView.ScreenDisplay?.Invalidate(envelope, true, (short)esriScreenCache.esriNoScreenCache);
                    

                    if (_toUpdate)
                    {
                        StartRedraw();
                    }
                    else
                    {
                        _updateTimer = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "ViewingCone.Redraw");
            }
        }

        private void StartRedraw()
        {
            var redrawEvent = new AutoResetEvent(true);
            var redrawTimerCallBack = new TimerCallback(Redraw);
            _updateTimer = new System.Threading.Timer(redrawTimerCallBack, redrawEvent, MaxTimeUpdate, -1);
            _toUpdate = false;
        }
    }
}