// <copyright file="StationVimo.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace HappyRoute4.Vimos {
    public class StationVimo {
        private StationView stationView;
        private Station stationNew;
        private Line linePath;
        private bool isDimed;
        private bool isVisible = true;

        public StationVimo(Station station, MapVimo mapVimo) {
            Station = station;
            MapVimo = mapVimo;
            stationView = new StationView(this);

            Resize();
            ChangeShape();
        }

        public Station Station { get; private set; }
        public MapVimo MapVimo { get; private set; }

        public bool IsStationTransit {
            get {
                switch (Station.Segments.Count) {
                    case 1:
                        return Station.Segments[0].Lines.Count == 1;
                    case 2:
                        if (Station.Segments[0].Lines.Count == 1 && Station.Segments[1].Lines.Count == 1) {
                            return Station.Segments[0].Lines[0] == Station.Segments[1].Lines[0];
                        }
                        else {
                            return false;
                        }

                    default:
                        return false;
                }
            }
        }

        public bool IsDimed {
            get {
                return isDimed;
            }
            set {
                if (isDimed != value) {
                    isDimed = value;
                    stationView.SetBrushes(LinePath);
                }
            }
        }

        public bool IsVisible {
            get {
                return isVisible;
            }
            set {
                if (isVisible != value) {
                    isVisible = value;
                    stationView.SetVisible();
                }
            }
        }

        public Line LinePath {
            get {
                return linePath;
            }
            set {
                if (linePath != value) {
                    linePath = value;
                    stationView.SetBrushes(LinePath);
                }
            }
        }

        public void Relocation() {
            stationView.Relocation();
        }

        public void Resize() {
            stationView.Resize(1d);
        }

        public void ChangeShape() {
            stationView.ChangeShape();
            stationView.SetBrushes(LinePath);
        }

        public void Remove() {
            MapVimo.Map.Stations.Remove(Station);
            stationView.Remove();
        }

        public Rect GetBord() {
            return stationView.GetBord();
        }

        public void SetIsName2() {
            stationView.RelocationText();
        }

        public void SetOver(bool isOver) {
            stationView.SetOver(isOver);
        }

        public void Reset(bool isDimed) {
            linePath = null;
            this.isDimed = isDimed;
            stationView.SetBrushes(linePath);
        }

        public void Select() {
            stationView.Select();
        }

        public void OnSelected(DependencyObject target) {
            MapVimo.SearchStationService.SearchText = null;
            if (MapVimo.IsEdit) {
                if (MapVimo.EditMode == EditModes.Align) {
                    if (IsDimed == true) {
                        MapVimo.MapRoute.UnselectLines();
                    }

                    MapVimo.MapRoute.SelectedStations.Change(Station, target);
                }
            }
            else {
                MapVimo.MapRoute.SelectedStations.Change(Station, target);
                if (IsDimed == true && !MapVimo.MapRoute.SelectedStations.IsTwoSelected) {
                    MapVimo.MapRoute.UnselectLines();
                }

                foreach (var station in MapVimo.Map.Stations) {
                    station.Vimo.IsVisible = true;
                }

                foreach (var segment in MapVimo.Map.Segments) {
                    segment.Vimo.IsVisible = true;
                }
            }
        }

        public bool OnPressed() {
            return MapVimo.IsEdit && (MapVimo.EditMode == EditModes.Move || MapVimo.EditMode == EditModes.Create);
        }

        public void OnReleased(Point position) {
            if (!MapVimo.IsEdit) {
                return;
            }

            Station stationMoved;
            if (MapVimo.EditMode == EditModes.Move) {
                stationMoved = Station;
            }
            else
                if (MapVimo.EditMode == EditModes.Create) {
                if (stationNew == null) {
                    return;
                }

                stationMoved = stationNew;
            }
            else {
                return;
            }

            var stationTarget = GetStationUnder(stationMoved);
            if (stationTarget != null) {
                foreach (var segment in stationMoved.Segments) {
                    if (segment.Station1 == stationMoved) {
                        segment.Station1 = stationTarget;
                    }
                    else {
                        segment.Station2 = stationTarget;
                    }

                    stationTarget.Segments.Add(segment);

                    // RemoveSegmentTwin-begin
                    var segmentsTwin = stationTarget.Segments.Where(n => n != segment
                        && ((n.Station1 == segment.Station1 && n.Station2 == segment.Station2)
                        || (n.Station1 == segment.Station2 && n.Station2 == segment.Station1))).ToList();

                    foreach (var segmentEq in segmentsTwin) {
                        foreach (var line in segmentEq.Lines) {
                            if (!segment.Lines.Contains(line)) {
                                segment.Lines.Add(line);
                            }
                        }

                        segmentEq.Station1.Segments.Remove(segmentEq);
                        segmentEq.Station2.Segments.Remove(segmentEq);
                        segmentEq.Vimo.Remove();
                        MapVimo.Map.Segments.Remove(segmentEq);
                    } // RemoveSegmentTwin-end

                    segment.Vimo.Relocation();
                    segment.Vimo.ChangeShape();
                }

                stationMoved.Vimo.Remove();

                if (stationTarget.Name2 != stationMoved.Name2) {
                    stationTarget.Name2 += stationMoved.Name2;
                }

                if (stationTarget.Name1 != stationMoved.Name1) {
                    stationTarget.Name1 += stationMoved.Name1;
                }

                stationTarget.Vimo.ChangeShape();
            }
            else {
                if (MapVimo.EditMode == EditModes.Create && stationMoved.Segments.Count == 1) {
                    var segmentNew = stationMoved.Segments[0];
                    var stationSource = segmentNew.Station1 == stationMoved ? segmentNew.Station2 : segmentNew.Station1;
                    if (stationSource.Segments.Count == 2) {
                        var segmentSource = stationSource.Segments[0] == segmentNew ? stationSource.Segments[1] : stationSource.Segments[0];
                        segmentNew.Lines.Clear();
                        foreach (var line in segmentSource.Lines) {
                            segmentNew.Lines.Add(line);
                        }

                        segmentNew.Vimo.Relocation();
                        segmentNew.Vimo.ChangeShape();
                        segmentNew.Station1.Vimo.ChangeShape();
                        segmentNew.Station2.Vimo.ChangeShape();
                    }
                    else {
                        MapVimo.MapProp.ShowSegmentProp(segmentNew, position);
                    }
                }
            }

            MapVimo.MapView.RecalcHostSize();
        }

        public void MoveStation(Point delta) {
            if (MapVimo.IsEdit) {
                if (MapVimo.EditMode == EditModes.Move) {
                    MoveStation(delta, Station);
                }
                else {
                    if (MapVimo.EditMode == EditModes.Create) {
                        if (stationNew != null) {
                            MoveStation(delta, stationNew);
                        }
                    }
                }
            }
        }

        public void CreateStation() {
            if (!MapVimo.IsEdit || MapVimo.EditMode != EditModes.Create) {
                return;
            }

            var segmentNew = new Segment();
            stationNew = new Station() {
                Angle = Station.Angle,
                FontSize = Station.FontSize,
                Left = Station.Left,
                Radius = Station.Radius,
                TextIndent = Station.TextIndent,
                Top = Station.Top
            };

            MapVimo.Map.Stations.Add(stationNew);
            MapVimo.Map.Segments.Add(segmentNew);
            Station.Segments.Add(segmentNew);
            stationNew.Segments.Add(segmentNew);
            segmentNew.Station1 = Station;
            segmentNew.Station2 = stationNew;
            segmentNew.Lines.Add(null);
            Station.Vimo.ChangeShape();
            segmentNew.Vimo = new SegmentVimo(segmentNew, MapVimo);
            stationNew.Vimo = new StationVimo(stationNew, MapVimo);
        }

        public void Prop() {
            MapVimo.MapProp.ShowStationProp(Station);
        }

        private void MoveStation(Point delta, Station station) {
            var zoomFactor = 1f;
            if (Math.Abs(MapVimo.MapView.ScrollViewer.ZoomFactor) > float.Epsilon) {
                zoomFactor = MapVimo.MapView.ScrollViewer.ZoomFactor;
            }

            station.Left += delta.X / zoomFactor;
            station.Top += delta.Y / zoomFactor;
            station.Vimo.Relocation();
            station.Vimo.SetOver(GetStationUnder(station) != null);
            foreach (var segment in station.Segments) {
                segment.Vimo.Relocation();
            }
        }

        private Station GetStationUnder(Station source) {
            var radius = MapVimo.Map.Param.Radius;
            var targets = MapVimo.Map.Stations.Where(s => s != source && s.Segments.All(g => g.Station1 != source && g.Station2 != source)
                && s.Left - Math.Max(s.Radius + radius, 0d) < source.Left && s.Left + Math.Max(s.Radius + radius, 0d) > source.Left
                && s.Top - Math.Max(s.Radius + radius, 0d) < source.Top && s.Top + Math.Max(s.Radius + radius, 0d) > source.Top);
            return targets.FirstOrDefault();
        }
    }
}
