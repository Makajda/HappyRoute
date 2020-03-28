// <copyright file="SegmentVimo.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using Windows.Foundation;

namespace HappyRoute4.Vimos {
    public class SegmentVimo {
        private SegmentView segmentView;
        private Line linePath;
        private bool isDimed;
        private bool isVisible = true;

        public SegmentVimo(Segment segment, MapVimo mapVimo) {
            Segment = segment;
            MapVimo = mapVimo;
            segmentView = new SegmentView(this);

            Relocation();
            ChangeShape();
        }

        public Segment Segment { get; private set; }
        public MapVimo MapVimo { get; private set; }

        public bool IsDimed {
            get {
                return isDimed;
            }
            set {
                if (isDimed != value) {
                    isDimed = value;
                    segmentView.SetBrushes(LinePath);
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
                    segmentView.SetVisible();
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
                    ChangeShape();
                }
            }
        }

        public void Relocation() {
            segmentView.Relocation(false);
        }

        public void ChangeShape() {
            segmentView.SetBrushes(LinePath);
        }

        public void ChangeLine(int iLine, Line newLine) {
            if (newLine != null && iLine >= 0 && iLine < Segment.Lines.Count) {
                if (Segment.Lines[iLine] != newLine) {
                    Segment.Lines[iLine] = newLine;
                    Segment.Vimo.ChangeShape();
                    Segment.Station1.Vimo.ChangeShape();
                    Segment.Station2.Vimo.ChangeShape();
                }
            }
        }

        public void Remove() {
            Segment.Station1.Segments.Remove(Segment);
            Segment.Station2.Segments.Remove(Segment);
            MapVimo.Map.Segments.Remove(Segment);
            Segment.Station1.Vimo.ChangeShape();
            Segment.Station2.Vimo.ChangeShape();
            segmentView.Remove();
        }

        public void Remove(int iLine) {
            if (iLine >= 0 && iLine < Segment.Lines.Count) {
                Segment.Lines.RemoveAt(iLine);
                segmentView.Remove(iLine);
                Segment.Vimo.ChangeShape();
                Segment.Station1.Vimo.ChangeShape();
                Segment.Station2.Vimo.ChangeShape();
            }
        }

        public void Reset(bool isDimed) {
            linePath = null;
            this.isDimed = isDimed;
            segmentView.SetBrushes(linePath);
        }

        public void AddStation(Point position) {
            if (MapVimo.IsEdit && MapVimo.EditMode == EditModes.Create) {
                var stationNew = new Station() { Left = position.X, Top = position.Y };
                var segmentNew = new Segment();
                MapVimo.Map.Stations.Add(stationNew);
                MapVimo.Map.Segments.Add(segmentNew);
                Segment.Station1.Segments.Remove(Segment);
                Segment.Station1.Segments.Add(segmentNew);
                stationNew.Segments.Add(Segment);
                stationNew.Segments.Add(segmentNew);
                segmentNew.Station1 = Segment.Station1;
                segmentNew.Station2 = stationNew;
                Segment.Station1 = stationNew;
                foreach (var line in Segment.Lines) {
                    segmentNew.Lines.Add(line);
                }

                segmentNew.Vimo = new SegmentVimo(segmentNew, MapVimo);
                stationNew.Vimo = new StationVimo(stationNew, MapVimo);
            }
        }

        public void Prop(Point position) {
            MapVimo.MapProp.ShowSegmentProp(Segment, position);
        }
    }
}
