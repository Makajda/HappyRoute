// <copyright file="LinePropAddStationsViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using HappyRoute4.Vimos;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace HappyRoute4.ViewModels.Prop {
    public class LinePropAddStationsViewModel : BindableBase {
        private double x1 = 0.1d;
        private double y1 = 0.5d;
        private double x2 = 0.9d;
        private double y2 = 0.5d;
        private bool isEllipse;
        private Line line;
        private double angleText = 0d;
        private string stations;

        public LinePropAddStationsViewModel(MapVimo mapVimo) {
            MapVimo = mapVimo;
            AddCommand = new DelegateCommand(Add);
            ClearCommand = new DelegateCommand(Clear);
        }

        public MapVimo MapVimo { get; private set; }
        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }

        public double X1 {
            get { return x1; }
            set { SetProperty(ref x1, value); }
        }

        public double Y1 {
            get { return y1; }
            set { SetProperty(ref y1, value); }
        }

        public double X2 {
            get { return x2; }
            set { SetProperty(ref x2, value); }
        }

        public double Y2 {
            get { return y2; }
            set { SetProperty(ref y2, value); }
        }

        public bool IsEllipse {
            get { return isEllipse; }
            set { SetProperty(ref isEllipse, value); }
        }

        public Line Line {
            get { return line; }
            set { SetProperty(ref line, value); }
        }

        public double AngleText {
            get { return angleText; }
            set { SetProperty(ref angleText, value); }
        }

        public string Stations {
            get { return stations; }
            set { SetProperty(ref stations, value); }
        }

        private void Clear() {
            Stations = null;
        }

        private void Add() {
            if (string.IsNullOrWhiteSpace(Stations)) {
                return;
            }

            var names = Stations.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            double x, y, dx_da, dy_a, max = 0;

            if (IsEllipse) {
                max = Math.Max(Math.Abs(X2 - X1) * MapVimo.MapView.Host.ActualWidth, Math.Abs(Y2 - Y1) * MapVimo.MapView.Host.ActualHeight) / 2d;
                x = (X1 + ((X2 - X1) / 2d)) * MapVimo.MapView.Host.ActualWidth;
                y = (Y1 + ((Y2 - Y1) / 2d)) * MapVimo.MapView.Host.ActualHeight;
                dx_da = Math.PI * 2d / names.Length;
                dy_a = Math.Acos((X1 - X2) / Math.Sqrt(Math.Pow(X2 - X1, 2) + Math.Pow(Y2 - Y1, 2)));
                if (Y2 > Y1) {
                    dy_a = (Math.PI * 2d) - dy_a;
                }

                dy_a += dx_da / 2d;
            }
            else {
                x = X1 * MapVimo.MapView.Host.ActualWidth;
                y = Y1 * MapVimo.MapView.Host.ActualHeight;
                dx_da = (X2 - X1) * MapVimo.MapView.Host.ActualWidth / (names.Length - 1);
                dy_a = (Y2 - Y1) * MapVimo.MapView.Host.ActualHeight / (names.Length - 1);
            }

            Station prevStation = null;

            foreach (var s in names) {
                var a = s.Split('/');
                var name1 = a.Length > 0 ? a[0].Trim() : null;
                var name2 = a.Length > 1 ? a[1].Trim() : null;

                var station = new Station() { Name1 = name1, Name2 = name2, Angle = AngleText };
                MapVimo.Map.Stations.Add(station);
                if (IsEllipse) {
                    station.Left = (max * Math.Cos(dy_a)) + x;
                    station.Top = (max * Math.Sin(dy_a)) + y;
                    dy_a += dx_da;
                }
                else {
                    station.Left = x;
                    station.Top = y;
                    x += dx_da;
                    y += dy_a;
                }

                if (prevStation != null) {
                    var segment = new Segment() { Station1 = prevStation, Station2 = station };
                    segment.Lines.Add(Line);
                    segment.Vimo = new SegmentVimo(segment, MapVimo);

                    station.Segments.Add(segment);
                    prevStation.Segments.Add(segment);
                    MapVimo.Map.Segments.Add(segment);
                    prevStation.Vimo.ChangeShape();
                }

                station.Vimo = new StationVimo(station, MapVimo);
                prevStation = station;
            }

            Stations = null;
            MapVimo.MapView.RecalcHostSize();
        }
    }
}
