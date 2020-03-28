// <copyright file="AlinerTabViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using HappyRoute4.Views;
using HappyRoute4.Vimos;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HappyRoute4.ViewModels {
    public class AlinerTabViewModel : BindableBase {
        private Station stationStart;
        private List<Station> stations;
        private double angleStations;
        private double angleNames;

        public AlinerTabViewModel(MapVimo mapVimo, Station stationStart, Station stationFinish, List<Station> branch) {
            MapVimo = mapVimo;
            this.stationStart = stationStart;
            Stations = branch;

            NamesCommand = new DelegateCommand(AlineNames);
            ThickCommand = new DelegateCommand(() => AlineStations(-1d));
            SetCommand = new DelegateCommand(() => AlineStations(0d));
            ThinCommand = new DelegateCommand(() => AlineStations(1d));

            View = new AlinerTabView() { DataContext = this };

            // AngleNames
            var angle = Stations.Count > 0 ? Stations[0].Angle : 0d;
            if (Stations.All(n => Math.Abs(n.Angle - angle) < double.Epsilon)) {
                AngleNames = angle;
            }
            else {
                AngleNames = 0d;
            }

            // AngleStations
            var diagonal = Math.Sqrt(Math.Pow(stationStart.Left - stationFinish.Left, 2) + Math.Pow(stationStart.Top - stationFinish.Top, 2));
            if (Math.Abs(diagonal) < double.Epsilon) {
                return;
            }

            var sin = (stationFinish.Left - stationStart.Left) / diagonal;
            if (stationFinish.Top > stationStart.Top) {
                AngleStations = Math.Acos(sin) * 180 / Math.PI;
            }
            else {
                AngleStations = (Math.Asin(sin) * 180 / Math.PI) - 90d;
            }
        }

        public AlinerTabView View { get; private set; }
        public MapVimo MapVimo { get; private set; }

        public DelegateCommand NamesCommand { get; private set; }
        public DelegateCommand ThickCommand { get; private set; }
        public DelegateCommand SetCommand { get; private set; }
        public DelegateCommand ThinCommand { get; private set; }

        public List<Station> Stations {
            get { return stations; }
            set { SetProperty(ref stations, value); }
        }

        public double AngleStations {
            get { return angleStations; }
            set { SetProperty(ref angleStations, value); }
        }

        public double AngleNames {
            get { return angleNames; }
            set { SetProperty(ref angleNames, value); }
        }

        private void AlineNames() {
            foreach (var station in Stations) {
                station.Angle = AngleNames;
            }

            MapVimo.MapView.RecalcHostSize();
        }

        private void AlineStations(double delta) {
            if (Stations.Count == 0) {
                return;
            }

            var stationFinish = Stations[0];
            var diagonal = Math.Sqrt(Math.Pow(stationStart.Left - stationFinish.Left, 2) + Math.Pow(stationStart.Top - stationFinish.Top, 2)) + delta;
            var d = diagonal / Stations.Count;
            foreach (var station in Stations) {
                station.Left = stationStart.Left + (diagonal * Math.Cos(AngleStations * Math.PI / 180d));
                station.Top = stationStart.Top + (diagonal * Math.Sin(AngleStations * Math.PI / 180d));
                station.Vimo.Relocation();
                diagonal -= d;
            }

            foreach (var segment in stations.SelectMany(n => n.Segments).Distinct()) {
                segment.Vimo.Relocation();
            }

            MapVimo.MapView.RecalcHostSize();
        }
    }
}
