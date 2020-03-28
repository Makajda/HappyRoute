// <copyright file="AlinerViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using HappyRoute4.Views;
using HappyRoute4.Vimos;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace HappyRoute4.ViewModels {
    public class AlinerViewModel : BindableBase {
        private AlinerView view;
        private MapVimo mapVimo;
        private List<Tuple<FrameworkElement, object>> routs;

        public AlinerViewModel(MapVimo mapVimo) {
            this.mapVimo = mapVimo;
            view = new AlinerView() { DataContext = this };
        }

        public List<Tuple<FrameworkElement, object>> Routs {
            get { return routs; }
            set { SetProperty(ref routs, value); }
        }

        public void Show(Station stationStart, Station stationFinish) {
            var r = new List<Tuple<FrameworkElement, object>>();
            var lines = stationStart.Segments.SelectMany(n => n.Lines).Intersect(stationFinish.Segments.SelectMany(n => n.Lines)).ToList();

            if (lines.Count == 0) {
                return;
            }

            var alinerLine = new AlinerLine(stationStart, stationFinish);
            foreach (var line in lines) {
                var max = mapVimo.Map.Stations.Count(n => n.Segments.Any(s => s.Lines.Contains(line)));
                var branches = alinerLine.GetLineBrunches(line, max);
                if (branches != null) {
                    foreach (var branch in branches) {
                        var alinerTabVimo = new AlinerTabViewModel(mapVimo, stationStart, stationFinish, branch);
                        r.Add(new Tuple<FrameworkElement, object>(alinerTabVimo.View, line == null ? "noname" : mapVimo.IsName2 ? line.Name2 : line.Name1));
                    }
                }
            }

            r.Sort(CompareAlinerTabVimoByStationCount);
            Routs = r;

            mapVimo.PropopupService.ShowPopup(view, new Point(stationFinish.Left, stationFinish.Top));
        }

        private static int CompareAlinerTabVimoByStationCount(Tuple<FrameworkElement, object> x, Tuple<FrameworkElement, object> y) {
            var xc = ((AlinerTabViewModel)x.Item1.DataContext).Stations.Count;
            var yc = ((AlinerTabViewModel)y.Item1.DataContext).Stations.Count;
            if (xc > yc) {
                return 1;
            }
            else {
                return xc == yc ? 0 : -1;
            }
        }

        private class AlinerLine {
            private Station stationStart;
            private Station stationFinish;
            private Line line;
            private int max;

            public AlinerLine(Station stationStart, Station stationFinish) {
                this.stationStart = stationStart;
                this.stationFinish = stationFinish;
            }

            public List<List<Station>> GetLineBrunches(Line line, int max) {
                this.line = line;
                this.max = max;
                var branches = GetNextBranches(stationStart, null, 0);
                return branches;
            }

            private List<List<Station>> GetNextBranches(Station station, Segment segmentStart, int cur) {
                if (cur > max) {
                    return null;
                }

                List<List<Station>> branches = null;

                var segments = station.Segments.Where(n => n != segmentStart && n.Lines.Contains(line));
                foreach (var segment in segments) {
                    var stationNext = segment.Station1 == station ? segment.Station2 : segment.Station1;
                    if (stationNext == stationFinish) {
                        var branch = new List<Station>();
                        branch.Add(stationNext);
                        if (branches == null) {
                            branches = new List<List<Station>>();
                        }

                        branches.Add(branch);
                    }
                    else {
                        var branchesNext = GetNextBranches(stationNext, segment, ++cur);
                        if (branchesNext != null) {
                            foreach (var branch in branchesNext) {
                                if (!branch.Contains(stationNext)) {
                                    branch.Add(stationNext);
                                    if (branches == null) {
                                        branches = new List<List<Station>>();
                                    }

                                    branches.Add(branch);
                                }
                            }
                        }
                    }
                }

                return branches;
            }
        }
    }
}
