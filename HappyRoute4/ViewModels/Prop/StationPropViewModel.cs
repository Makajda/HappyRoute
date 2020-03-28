// <copyright file="StationPropViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using HappyRoute4.Views.Prop;
using HappyRoute4.Vimos;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace HappyRoute4.ViewModels.Prop {
    public class StationPropViewModel : BindableBase {
        private TabView view;
        private StationPropNameTabView viewName;
        private StationPropParamTabView viewParam;
        private bool isOnChanged = false;
        private Station station;
        private double radius;
        private double fontSize;
        private double textIndent;
        private double transferWeight;

        public StationPropViewModel(MapVimo mapVimo) {
            this.MapVimo = mapVimo;
            RemoveCommand = new DelegateCommand(Remove);

            viewName = new StationPropNameTabView() { DataContext = this };
            viewParam = new StationPropParamTabView() { DataContext = this };

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var res1 = loader.GetString("StationPropNames");
            var res2 = loader.GetString("StationPropParam");

            var r = new List<Tuple<FrameworkElement, object>>();
            r.Add(new Tuple<FrameworkElement, object>(viewName, res1));
            r.Add(new Tuple<FrameworkElement, object>(viewParam, res2));
            view = new TabView() { ItemsSource = r };
        }

        public MapVimo MapVimo { get; private set; }
        public DelegateCommand RemoveCommand { get; private set; }

        public Station Station {
            get { return station; }
            set { SetProperty(ref station, value); }
        }

        public double Radius {
            get { return radius; }
            set { SetProperty(ref radius, value, OnRadiusChanged); }
        }

        public double FontSize {
            get { return fontSize; }
            set { SetProperty(ref fontSize, value, OnParamChanged); }
        }

        public double TextIndent {
            get { return textIndent; }
            set { SetProperty(ref textIndent, value, OnParamChanged); }
        }

        public double TransferWeight {
            get { return transferWeight; }
            set { SetProperty(ref transferWeight, value, OnTransferWeightChanged); }
        }

        public void Show(Station station) {
            isOnChanged = false;
            Station = station;
            Radius = station.Radius;
            FontSize = station.FontSize;
            TextIndent = station.TextIndent;

            MapVimo.PropopupService.ShowPopup(view, new Point(station.Left, station.Top));
            isOnChanged = true;
        }

        private void Remove() {
            if (Station == null) {
                return;
            }

            while (Station.Segments.Count > 0) {
                var segment = Station.Segments[0];
                segment.Vimo.Remove();
                var stationPrev = segment.Station1 == Station ? segment.Station2 : segment.Station1;

                while (segment.Lines.Count > 0) {
                    var line = segment.Lines[0];
                    segment.Lines.Remove(line);

                    var segmentNext = Station.Segments.FirstOrDefault(n => n.Lines.Contains(line));
                    if (segmentNext == null) {
                        stationPrev.Vimo.ChangeShape();
                    }
                    else {
                        if (segmentNext.Lines.Count > 1) {
                            segmentNext.Lines.Remove(line);
                        }
                        else {
                            segmentNext.Vimo.Remove();
                        }

                        var stationNext = segmentNext.Station1 == Station ? segmentNext.Station2 : segmentNext.Station1;
                        var segmentExist = stationPrev.Segments.FirstOrDefault(n => n.Station1 == stationNext || n.Station2 == stationNext);
                        if (segmentExist != null) {
                            if (!segmentExist.Lines.Contains(line)) {
                                segmentExist.Lines.Add(line);
                                segmentExist.Vimo.ChangeShape();
                                stationPrev.Vimo.ChangeShape();
                                stationNext.Vimo.ChangeShape();
                            }
                        }
                        else {
                            var segmentNew = new Segment() { Station1 = stationPrev, Station2 = stationNext };
                            segmentNew.Lines.Add(line);
                            segmentNew.Vimo = new SegmentVimo(segmentNew, MapVimo);
                            stationPrev.Segments.Add(segmentNew);
                            stationNext.Segments.Add(segmentNew);
                            MapVimo.Map.Segments.Add(segmentNew);
                            stationPrev.Vimo.ChangeShape();
                            stationNext.Vimo.ChangeShape();
                        }
                    }
                }
            }

            Station.Vimo.Remove();

            MapVimo.PropopupService.HidePopup();
            MapVimo.MapView.RecalcHostSize();
        }

        private void OnParamChanged() {
            if (isOnChanged) {
                Station.FontSize = FontSize;
                Station.TextIndent = TextIndent;
                Station.Vimo.Resize();
            }
        }

        private void OnRadiusChanged() {
            if (isOnChanged) {
                Station.Radius = Radius;
                Station.Vimo.Resize();
                foreach (var segment in Station.Segments) {
                    segment.Vimo.Relocation();
                }
            }
        }

        private void OnTransferWeightChanged() {
            if (isOnChanged) {
                Station.TransferWeight = TransferWeight;
                MapVimo.MapRoute.Reset();
            }
        }
    }
}
