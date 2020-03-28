// <copyright file="LinePropViewModel.cs" company="Makajda">
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
using Windows.UI;
using Windows.UI.Xaml;

namespace HappyRoute4.ViewModels.Prop {
    public class LinePropViewModel : BindableBase {
        private TabView view;
        private LinePropNameTabView viewName;
        private LinePropColorTabView viewColor;
        private LinePropRemoveTabView viewRemove;
        private LinePropAddStationsTabView viewAddStations;
        private LinePropAddStationsViewModel addStationsVimo;
        private bool isOnChanged = false;
        private Line line;
        private double thick;
        private double indent;
        private Color color;

        public LinePropViewModel(MapVimo mapVimo) {
            this.MapVimo = mapVimo;

            RemoveCommand = new DelegateCommand(Remove);
            RemoveStationsCommand = new DelegateCommand(RemoveStations);

            viewName = new LinePropNameTabView() { DataContext = this };
            viewColor = new LinePropColorTabView() { DataContext = this };
            viewRemove = new LinePropRemoveTabView() { DataContext = this };
            viewAddStations = new LinePropAddStationsTabView() { DataContext = addStationsVimo = new LinePropAddStationsViewModel(mapVimo) };

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var res1 = loader.GetString("LinePropNames");
            var res2 = loader.GetString("LinePropColor");
            var res3 = loader.GetString("LinePropAddStations");
            var res4 = loader.GetString("LinePropRemove");

            var r = new List<Tuple<FrameworkElement, object>>();
            r.Add(new Tuple<FrameworkElement, object>(viewName, res1));
            r.Add(new Tuple<FrameworkElement, object>(viewColor, res2));
            r.Add(new Tuple<FrameworkElement, object>(viewAddStations, res3));
            r.Add(new Tuple<FrameworkElement, object>(viewRemove, res4));
            view = new TabView() { ItemsSource = r };
        }

        public MapVimo MapVimo { get; private set; }

        public DelegateCommand RemoveCommand { get; private set; }
        public DelegateCommand RemoveStationsCommand { get; private set; }

        public Line Line {
            get { return line; }
            set { SetProperty(ref line, value); }
        }

        public double Thick {
            get { return thick; }
            set { SetProperty(ref thick, value, OnParamChanged); }
        }

        public double Indent {
            get { return indent; }
            set { SetProperty(ref indent, value, OnParamChanged); }
        }

        public Color Color {
            get { return color; }
            set { SetProperty(ref color, value, OnColorChanged); }
        }

        public void Show(Line line) {
            isOnChanged = false;
            Line = line;
            Thick = line.Thick;
            Indent = line.Indent;
            Color = line.Color;
            isOnChanged = true;
            addStationsVimo.Line = line;
            MapVimo.PropopupService.ShowPopup(view);
        }

        private void Remove() {
            Remove(true);
        }

        private void RemoveStations() {
            Remove(false);
        }

        private void Remove(bool isRemoveLine) {
            var segments = MapVimo.Map.Segments.Where(n => n.Lines.Contains(Line)).ToList();
            foreach (var segment in segments) {
                if (segment.Lines.Count == 1) {
                    segment.Vimo.Remove();
                }
                else {
                    var iLine = segment.Lines.IndexOf(Line);
                    segment.Vimo.Remove(iLine);
                }
            }

            var stations = segments.Select(n => n.Station1).Union(segments.Select(n => n.Station2)).Distinct()
                .Where(t => t.Segments.Count == 0).ToList();
            foreach (var station in stations) {
                station.Vimo.Remove();
            }

            foreach (var station in MapVimo.Map.Stations) {
                station.Name1 = NameSplitChangeNumberLine(station.Name1);
                station.Name2 = NameSplitChangeNumberLine(station.Name2);
            }

            if (isRemoveLine) {
                MapVimo.Map.Lines.Remove(Line);
                MapVimo.PropopupService.HidePopup();
            }

            MapVimo.MapView.RecalcHostSize();
        }

        private string NameSplitChangeNumberLine(string nameString) {
            if (string.IsNullOrEmpty(nameString) || nameString[0] != '{') {
                return nameString;
            }

            var index = MapVimo.Map.Lines.IndexOf(Line) + 1;

            var a = nameString.Split('{');
            var newNameString = string.Empty;
            foreach (var b in a) {
                int i = 0;
                var isMinus = false;
                if (i < b.Length && b[i] == '-') {
                    isMinus = true;
                    i++;
                }

                var x = string.Empty;
                while (i < b.Length && char.IsDigit(b[i])) {
                    x += b[i++];
                }

                if (x.Length > 0 && i + 1 < b.Length && b[i] == '}') {
                    var n = b.Substring(i + 1);
                    if (n.Length > 0) {
                        int j;
                        if (int.TryParse(x, out j)) {
                            if (j >= index && j > 1) {
                                j--;
                            }

                            newNameString += string.Format("{{{0}{1}}}{2}", isMinus ? "-" : string.Empty, j, n);
                        }
                    }
                }
            }

            return newNameString;
        }

        private void OnParamChanged() {
            if (isOnChanged) {
                Line.Thick = Thick;
                Line.Indent = Indent;
                foreach (var segment in MapVimo.Map.Segments.Where(s => s.Lines.Contains(Line))) {
                    segment.Vimo.Relocation();
                }
            }
        }

        private void OnColorChanged() {
            if (isOnChanged) {
                Line.Color = Color;

                var segments = MapVimo.Map.Segments.Where(n => n.Lines.Contains(Line));
                foreach (var segment in segments) {
                    segment.Vimo.ChangeShape();
                }

                var stations = segments.Select(n => n.Station1).Union(segments.Select(n => n.Station2)).Distinct();
                foreach (var station in stations) {
                    station.Vimo.ChangeShape();
                }
            }
        }
    }
}
