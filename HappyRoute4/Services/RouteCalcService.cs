// <copyright file="RouteCalcService.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Statline = System.Tuple<HappyRoute4.Models.Station, HappyRoute4.Models.Line>;

namespace HappyRoute4.Services {
    public class RouteCalcService {
        private List<Statline> statlines;
        private List<Tuple<int, double>>[] matrix; // int-index in statlines, double-weight
        private Stack<Statline> lastPath;
        private bool isPrepared;
        private Map map;

        public RouteCalcService() {
        }

        public void SetMap(Map map) {
            this.map = map;
            Reset();
        }

        public void Reset() {
            isPrepared = false;
        }

        public bool Recalc(Station selectedStation1, Station selectedStation2) {
            foreach (var segment in map.Segments) {
                segment.Vimo.Reset(true);
            }

            foreach (var station in map.Stations.Where(n => n != selectedStation1 && n != selectedStation2)) {
                station.Vimo.Reset(true);
            }

            if (!isPrepared) {
                Prepare();
            }

            var linesStart = GetLines(selectedStation1);
            var linesFinish = GetLines(selectedStation2);

            var rs = new List<Tuple<double, Stack<Statline>>>();
            foreach (var lineStart in linesStart) {
                foreach (var lineFinish in linesFinish) {
                    var tuple = CalcOne(selectedStation1, lineStart, selectedStation2, lineFinish);
                    if (tuple != null) {
                        rs.Add(tuple);
                    }
                }
            }

            bool isReadyShowRoute;
            var q = rs.FirstOrDefault(n => n.Item1 == rs.Min(m => m.Item1));
            if (q == null) {
                isReadyShowRoute = false;
                selectedStation2.Vimo.IsDimed = false;
            }
            else {
                lastPath = q.Item2;
                isReadyShowRoute = true;
                DrawPath();
            }

            return isReadyShowRoute;
        }

        public IEnumerable<RouteHop> GetPath() {
            if (lastPath == null) {
                return null;
            }

            var routePath = new List<RouteHop>();

            var statlineStart = lastPath.Peek();
            var runTotal = 0d;
            routePath.Add(new RouteHop(0d, 0d, statlineStart.Item2, statlineStart.Item1));
            var prev = statlineStart;
            var line = statlineStart.Item2;
            foreach (var next in lastPath.Skip(1)) {
                var weight = matrix[statlines.IndexOf(prev)].First(n => n.Item1 == statlines.IndexOf(next)).Item2;
                runTotal += weight;
                routePath.Add(new RouteHop(weight, runTotal, next.Item2, next.Item1));
                prev = next;
            }

            return routePath;
        }

        private void Prepare() {
            // Map.Stations -> List<Statline>
            statlines = new List<Statline>();
            foreach (var station in map.Stations) {
                var statline = station.Segments.SelectMany(n => n.Lines).Distinct().Select(l => new Statline(station, l));
                statlines.AddRange(statline);
            }

            // List<Statline> -> matrix of List<Tuple<int, double>>
            var count = statlines.Count;
            matrix = new List<Tuple<int, double>>[count]; // int-index in statlines, double-weight

            var transferWeight = map.Param.TransferWeight; // TransferWeight - obsolete +MapVimo.TransferWeight;

            for (int i = 0; i < count; i++) {
                var statline = statlines[i];
                matrix[i] = new List<Tuple<int, double>>();

                foreach (var segment in statline.Item1.Segments.Where(n => n.Lines.Contains(statline.Item2))) {
                    var station2 = segment.Station1 == statline.Item1 ? segment.Station2 : segment.Station1;
                    var statline2 = statlines.FirstOrDefault(n => n.Item1 == station2 && n.Item2 == statline.Item2);
                    var i2 = statlines.IndexOf(statline2);

                    var segmentWeight = segment.Weight;
                    if (segmentWeight < 0d) {
                        segmentWeight = 0d;
                    }

                    matrix[i].Add(new Tuple<int, double>(i2, segmentWeight));
                }

                var stationTransferWeight = statline.Item1.TransferWeight + transferWeight;
                if (stationTransferWeight < 0d) {
                    stationTransferWeight = 0d;
                }

                foreach (var tuple in statlines.Where(n => n.Item1 == statline.Item1).Where(n => n != statline)) {
                    var index = statlines.IndexOf(tuple);
                    matrix[i].Add(new Tuple<int, double>(index, stationTransferWeight));
                }
            }

            isPrepared = true;
        }

        private IEnumerable<Line> GetLines(Station selectedStation) {
            if (selectedStation.Vimo.LinePath == null) {
                return selectedStation.Segments.SelectMany(n => n.Lines).Distinct();
            }
            else {
                var lines = new List<Line>();
                lines.Add(selectedStation.Vimo.LinePath);
                return lines;
            }
        }

        private Tuple<double, Stack<Statline>> CalcOne(Station stationStart, Line lineStart, Station stationFinish, Line lineFinish) {
            var statlineStart = statlines.FirstOrDefault(n => n.Item1 == stationStart && n.Item2 == lineStart);
            var statlineFinish = statlines.FirstOrDefault(n => n.Item1 == stationFinish && n.Item2 == lineFinish);

            var indexStart = statlines.IndexOf(statlineStart);
            var indexFinish = statlines.IndexOf(statlineFinish);

            if (indexStart < 0 || indexFinish < 0) {
                return null;
            }

            var count = matrix.Length;

            // Dijkstra
            var d = new double[count];
            for (int i = 0; i < count; i++) {
                d[i] = double.PositiveInfinity;
            }

            d[indexStart] = 0d;
            var p = new int[count];
            var used = new bool[count];

            for (int i = 0; i < count; i++) {
                int v = -1;
                for (int j = 0; j < count; j++) {
                    if (!used[j] && (v == -1 || d[j] < d[v])) {
                        v = j;
                    }
                }

                if (double.IsPositiveInfinity(d[v])) {
                    break;
                }

                used[v] = true;

                for (int j = 0; j < matrix[v].Count; j++) {
                    var tuple = matrix[v][j];
                    var to = tuple.Item1;
                    var weight = tuple.Item2;
                    if (d[v] + weight < d[to]) {
                        d[to] = d[v] + weight;
                        p[to] = v;
                    }
                }
            }

            // show
            if (used[indexFinish]) {
                var path = new Stack<Statline>();
                for (int v = indexFinish; v != indexStart; v = p[v]) {
                    path.Push(statlines[v]);
                }

                path.Push(statlineStart);
                return new Tuple<double, Stack<Statline>>(d[indexFinish], path);
            }

            return null;
        }

        private void DrawPath() {
            var statlineStart = lastPath.Peek();
            var statlineFinish = lastPath.LastOrDefault();

            statlineStart.Item1.Vimo.LinePath = statlineStart.Item2;
            statlineFinish.Item1.Vimo.LinePath = statlineFinish.Item2;

            var prev = statlineStart;
            var line = statlineStart.Item2;
            foreach (var next in lastPath.Skip(1)) {
                next.Item1.Vimo.IsDimed = false;
                if (next.Item2 == line) {
                    if (next.Item1.Vimo.LinePath == null) {
                        next.Item1.Vimo.LinePath = line;
                    }
                }
                else {
                    line = next.Item2;
                    if (next.Item1 != statlineStart.Item1 && next.Item1 != statlineFinish.Item1) {
                        next.Item1.Vimo.LinePath = null;
                    }
                }

                var segment = prev.Item1.Segments.Where(n => (n.Station1 == prev.Item1 && n.Station2 == next.Item1)
                    || (n.Station2 == prev.Item1 && n.Station1 == next.Item1)).FirstOrDefault();
                if (segment != null) {
                    segment.Vimo.IsDimed = false;
                    segment.Vimo.LinePath = prev.Item2;
                }

                prev = next;
            }
        }
    }
}
