﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESRI.ArcGIS.Geometry;
//using StreetSmartArcMap.Layers;
//using StreetSmartArcMap.Client;
//using StreetSmartArcMap.DockableWindows;
//using StreetSmartArcMap.Utilities;

//namespace StreetSmartArcMap.Objects
//{
//    class Measurement : SortedDictionary<int, MeasurementPoint>, IDisposable
//    {
//        #region members

//        // =========================================================================
//        // Members
//        // =========================================================================
//        private static readonly Dictionary<int, Measurement> Measurements;
//        private static Measurement _open;

//        private readonly StreetSmartDockableWindow _StreetSmartDockableWindow;
//        private readonly TypeOfLayer _typeOfLayer;
//        private readonly int _entityId;

//        private static readonly LogClient LogClient;

//        #endregion

//        #region properties

//        // =========================================================================
//        // Properties
//        // =========================================================================
//        public static Measurement Sketch { get; private set; }
//        public bool DrawPoint { get; private set; }

//        public bool IsPointMeasurement
//        {
//            get { return (_typeOfLayer == TypeOfLayer.Point); }
//        }

//        public bool IsSketch
//        {
//            get { return (Sketch == this); }
//        }

//        public bool IsOpen
//        {
//            get { return (_open == this); }
//        }

//        #endregion

//        #region constructor

//        // =========================================================================
//        // Constructor
//        // =========================================================================
//        static Measurement()
//        {
//            Measurements = new Dictionary<int, Measurement>();
//            LogClient = new LogClient(typeof(Measurement));
//            _open = null;
//            Sketch = null;
//        }

//        private Measurement(int entityId, string entityType, StreetSmartDockableWindow StreetSmartDockableWindow, bool drawPoint)
//        {
//            _entityId = entityId;
//            _StreetSmartDockableWindow = StreetSmartDockableWindow;
//            DrawPoint = drawPoint;

//            switch (entityType)
//            {
//                case "pointMeasurement":
//                    _typeOfLayer = TypeOfLayer.Point;
//                    break;
//                case "surfaceMeasurement":
//                    _typeOfLayer = TypeOfLayer.Polygon;
//                    break;
//                case "lineMeasurement":
//                    _typeOfLayer = TypeOfLayer.Line;
//                    break;
//                default:
//                    _typeOfLayer = TypeOfLayer.None;
//                    break;
//            }
//        }

//        #endregion

//        #region functions (public)

//        // =========================================================================
//        // Functions (Public)
//        // =========================================================================
//        public void Dispose()
//        {
//            foreach (var element in this)
//            {
//                MeasurementPoint measurementPoint = element.Value;
//                measurementPoint.Dispose();
//            }

//            _open = IsOpen ? null : _open;
//            Sketch = IsSketch ? null : Sketch;

//            if (Measurements.ContainsKey(_entityId))
//            {
//                Measurements.Remove(_entityId);
//            }
//        }

//        public bool IsTypeOfLayer(TypeOfLayer typeOfLayer)
//        {
//            return (_typeOfLayer == typeOfLayer);
//        }

//        public MeasurementPoint GetPointByNr(int nr)
//        {
//            return Values.ElementAt(nr);
//        }

//        public void Close()
//        {
//            _open = null;
//            DrawPoint = true;
//        }

//        public void Open()
//        {
//            _open = this;
//        }

//        public void SetSketch()
//        {
//            Sketch = this;
//        }

//        public void AddPoint(int pointId)
//        {
//            Add(pointId, new MeasurementPoint(pointId, (Count + 1), this));
//        }

//        public void OpenPoint()
//        {
//            if (Values.Count == 1)
//            {
//                MeasurementPoint measurementPoint = GetPointByNr(0);
//                measurementPoint.OpenPoint();
//            }
//        }

//        public void OpenPoint(int pointId)
//        {
//            if (_StreetSmartDockableWindow != null)
//            {
//                _StreetSmartDockableWindow.OpenMeasurementPoint(_entityId, pointId);
//            }
//        }

//        public void ClosePoint(int pointId)
//        {
//            if (_StreetSmartDockableWindow != null)
//            {
//                _StreetSmartDockableWindow.CloseMeasurementPoint(_entityId, pointId);
//            }
//        }

//        public MeasurementPoint GetPoint(IPoint point)
//        {
//            return Values.Aggregate<MeasurementPoint, MeasurementPoint>
//              (null, (current, value) => value.IsSame(point) ? value : current);
//        }

//        public MeasurementPoint GetPoint(IPoint point, bool includeZ)
//        {
//            return Values.Aggregate<MeasurementPoint, MeasurementPoint>
//              (null, (current, value) => value.IsSame(point, includeZ) ? value : current);
//        }

//        //public void UpdatePoint(int pointId, StreetSmart.Common.Interfaces.Data. PointMeasurementData measurementData, int index)
//        //{
//        //    if (!ContainsKey(pointId))
//        //    {
//        //        AddPoint(pointId);
//        //    }

//        //    if (ContainsKey(pointId))
//        //    {
//        //        MeasurementPoint measurementPoint = this[pointId];
//        //        measurementPoint.UpdatePoint(measurementData, index);
//        //    }
//        //}

//        //public void CloseMeasurement()
//        //{
//        //    if ((_StreetSmartDockableWindow != null) && IsOpen)
//        //    {
//        //        _open = null;
//        //        _StreetSmartDockableWindow.CloseMeasurement(_entityId);
//        //    }
//        //}

//        //public void DisableMeasurementSeries()
//        //{
//        //    if (_StreetSmartDockableWindow != null)
//        //    {
//        //        _StreetSmartDockableWindow.DisableMeasurementSeries();
//        //    }
//        //}

//        //public void EnableMeasurementSeries()
//        //{
//        //    if (_StreetSmartDockableWindow != null)
//        //    {
//        //        _StreetSmartDockableWindow.EnableMeasurementSeries(_entityId);
//        //    }
//        //}

//        //public void OpenMeasurement()
//        //{
//        //    if (_StreetSmartDockableWindow != null)
//        //    {
//        //        if (!IsOpen)
//        //        {
//        //            if (_open != null)
//        //            {
//        //                _open.CloseMeasurement();
//        //            }

//        //            _open = this;
//        //            _StreetSmartDockableWindow.OpenMeasurement(_entityId);
//        //        }

//        //        if (IsPointMeasurement)
//        //        {
//        //            _StreetSmartDockableWindow.SetFocusEntity(_entityId);
//        //            _StreetSmartDockableWindow.AddMeasurementPoint(_entityId);
//        //        }
//        //    }
//        //}

//        //public void RemoveMeasurement()
//        //{
//        //    if (_StreetSmartDockableWindow != null)
//        //    {
//        //        if (IsOpen)
//        //        {
//        //            CloseMeasurement();
//        //        }

//        //        _StreetSmartDockableWindow.RemoveMeasurement(_entityId);
//        //        Dispose();
//        //    }
//        //}

//        public void RemovePoint(int pointId)
//        {
//            MeasurementPoint measurementPoint = this[pointId];
//            measurementPoint.Dispose();
//            Remove(pointId);

//            for (int i = 0; i < Count; i++)
//            {
//                MeasurementPoint msPoint = GetPointByNr(i);
//                msPoint.SetIntId(i + 1);
//            }
//        }

//        public bool CheckSelectedVertex()
//        {
//            bool opened = false;

//            foreach (var value in Values)
//            {
//                bool selected = value.CheckSelected();

//                if (selected)
//                {
//                    if (!opened)
//                    {
//                        value.OpenPoint();
//                        opened = true;
//                    }
//                }
//                else
//                {
//                    value.ClosePoint();
//                }
//            }

//            return (Values.Count != 0);
//        }

//        public IPointCollection4 ToPointCollection(IGeometry geometry, out int nrPoints)
//        {
//            IPointCollection4 result = null;
//            nrPoints = 0;

//            if (geometry != null)
//            {
//                result = geometry as IPointCollection4;
//                TypeOfLayer typeOfLayer = GetTypeOfLayer(geometry);

//                if ((!geometry.IsEmpty) && (typeOfLayer == TypeOfLayer.Point) && (result == null) && IsPointMeasurement)
//                {
//                    var pointc = geometry as IPoint;
//                    result = new MultipointClass();
//                    result.AddPoint(pointc);
//                }

//                if (result != null)
//                {
//                    nrPoints = result.PointCount;

//                    if ((nrPoints >= 2) && (typeOfLayer == TypeOfLayer.Polygon))
//                    {
//                        IPoint point1 = result.Point[0];
//                        IPoint point2 = result.Point[nrPoints - 1];
//                        nrPoints = (point1.Compare(point2) == 0) ? (nrPoints - 1) : nrPoints;
//                    }
//                }
//            }

//            return result;
//        }

//        public int GetMeasurementPointIndex(int pointId)
//        {
//            int result = 0;

//            if (_StreetSmartDockableWindow != null)
//            {
//                result = _StreetSmartDockableWindow.GetMeasurementPointIndex(_entityId, pointId);
//            }

//            return result;
//        }

//        public void UpdateMeasurementPoints(IGeometry geometry)
//        {
//            LogClient.Info(string.Format("Update MeasurementPoints. EntityId: {0}", _entityId));

//            if ((geometry != null) && (_StreetSmartDockableWindow != null))
//            {
//                int nrPoints;
//                var ptColl = ToPointCollection(geometry, out nrPoints);

//                if (ptColl != null)
//                {
//                    int msPoints = Count;
//                    var toRemove = new Dictionary<MeasurementPoint, bool>();
//                    var toAdd = new List<IPoint>();

//                    for (int i = 0; i < msPoints; i++)
//                    {
//                        MeasurementPoint measurementPoint = GetPointByNr(i);

//                        if (!measurementPoint.NotCreated)
//                        {
//                            LogClient.Info(string.Format("To remove added: entityId: {0}", measurementPoint.PointId));
//                            toRemove.Add(measurementPoint, true);
//                        }
//                    }

//                    for (int j = 0; j < nrPoints; j++)
//                    {
//                        IPoint point = ptColl.Point[j];
//                        var measurementPoint = GetPoint(point);

//                        if (measurementPoint == null)
//                        {
//                            LogClient.Info(string.Format("To add added: number: {0}", j));
//                            toAdd.Add(point);
//                        }
//                        else
//                        {
//                            if (toRemove.ContainsKey(measurementPoint))
//                            {
//                                toRemove[measurementPoint] = false;
//                            }
//                        }
//                    }

//                    if (toRemove.Aggregate(false, (current, remove) => remove.Value || current) || (toAdd.Count >= 1))
//                    {
//                        if (!IsPointMeasurement)
//                        {
//                            _StreetSmartDockableWindow.DisableMeasurementSeries();
//                        }

//                        foreach (var elem in toRemove)
//                        {
//                            if (elem.Value)
//                            {
//                                MeasurementPoint msPoint = elem.Key;
//                                _StreetSmartDockableWindow.RemoveMeasurementPoint(_entityId, msPoint.PointId);
//                            }
//                        }

//                        foreach (var point in toAdd)
//                        {
//                            IPoint gsPoint = new Point
//                            {
//                                X = point.X,
//                                Y = point.Y,
//                                Z = point.Z,
//                                SpatialReference = ArcUtils.SpatialReference
//                            };

//                            Configuration.Configuration config = Configuration.Configuration.Instance;
//                            SpatialReference spatRel = config.SpatialReference;
//                            ISpatialReference spatialReference = (spatRel == null) ? ArcUtils.SpatialReference : spatRel.SpatialRef;
//                            gsPoint.Project(spatialReference);
//                            _StreetSmartDockableWindow.CreateMeasurementPoint(_entityId, gsPoint);
//                        }

//                        if (!IsPointMeasurement)
//                        {
//                            _StreetSmartDockableWindow.EnableMeasurementSeries(_entityId);
//                        }
//                    }
//                }
//            }
//        }

//        #endregion

//        #region functions (static)

//        // =========================================================================
//        // Functions (Static)
//        // =========================================================================
//        public static void CloseOpenMeasurement()
//        {
//            if (_open != null)
//            {
//                _open.CloseMeasurement();
//            }
//        }

//        public static void RemoveSketch()
//        {
//            if (Sketch != null)
//            {
//                Sketch.RemoveMeasurement();
//            }
//        }

//        public static Measurement Get(int entityId)
//        {
//            return Measurements.ContainsKey(entityId) ? Measurements[entityId] : null;
//        }

//        public static Measurement Get(IGeometry geometry)
//        {
//            return Get(geometry, true);
//        }

//        public static Measurement Get(IGeometry geometry, bool includeZ)
//        {
//            Measurement result = null;

//            if (geometry != null)
//            {
//                for (int i = 0; ((i < Measurements.Count) && (result == null)); i++)
//                {
//                    var element = Measurements.ElementAt(i);
//                    Measurement measurement = element.Value;
//                    int nrPoints;
//                    var ptColl = measurement.ToPointCollection(geometry, out nrPoints);

//                    if ((ptColl != null) && (measurement != null))
//                    {
//                        int msPoints = measurement.Count;

//                        if (nrPoints == msPoints)
//                        {
//                            bool found = true;

//                            for (int j = 0; ((j < nrPoints) && found); j++)
//                            {
//                                IPoint point = ptColl.Point[j];
//                                MeasurementPoint measurementPoint = measurement.GetPointByNr(j);

//                                if ((measurementPoint != null) && (point != null))
//                                {
//                                    found = measurementPoint.IsSame(point, includeZ);
//                                }
//                            }

//                            if (found)
//                            {
//                                result = measurement;
//                            }
//                        }
//                    }
//                }
//            }

//            return result;
//        }

//        public static void Update()
//        {
//            foreach (var measurement in Measurements)
//            {
//                foreach (var measurementPoint in measurement.Value)
//                {
//                    var point = measurementPoint.Value;
//                    point.Update();
//                }
//            }
//        }

//        public static void Add(int entityId, string entityType, StreetSmartDockableWindow StreetSmartDockableWindow, bool drawPoint)
//        {
//            Measurements.Add(entityId, new Measurement(entityId, entityType, StreetSmartDockableWindow, drawPoint));
//        }

//        public static void RemoveAll()
//        {
//            while (Measurements.Count >= 1)
//            {
//                var element = Measurements.ElementAt(0);
//                Measurement measurement = element.Value;
//                measurement.RemoveMeasurement();
//            }
//        }

//        public static void RemoveUnusedMeasurements(List<Measurement> usedMeasurements)
//        {
//            if (Sketch != null)
//            {
//                if (!usedMeasurements.Contains(Sketch))
//                {
//                    usedMeasurements.Add(Sketch);
//                }
//            }

//            int i = 0;

//            while (i < Measurements.Count)
//            {
//                var measurement = Measurements.ElementAt(i);
//                Measurement element = measurement.Value;

//                if (!usedMeasurements.Contains(element))
//                {
//                    element.RemoveMeasurement();
//                }
//                else
//                {
//                    i++;
//                }
//            }
//        }

//        public static TypeOfLayer GetTypeOfLayer(IGeometry geometry)
//        {
//            var result = TypeOfLayer.None;

//            if (geometry != null)
//            {
//                if (geometry is IPoint)
//                {
//                    result = TypeOfLayer.Point;
//                }

//                if (geometry is IPolyline)
//                {
//                    result = TypeOfLayer.Line;
//                }

//                if (geometry is IPolygon4)
//                {
//                    result = TypeOfLayer.Polygon;
//                }
//            }

//            return result;
//        }

//        #endregion
//    }
//}