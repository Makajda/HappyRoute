// <copyright file="RouteLineViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using HappyRoute4.Services;
using HappyRoute4.Views;
using HappyRoute4.Vimos;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace HappyRoute4.ViewModels {
    public class RouteLineViewModel : BindableBase {
        private RouteLineView view;
        private Station station;
        private ICollectionView mapLines;

        public RouteLineViewModel(MapVimo mapVimo) {
            MapVimo = mapVimo;
            view = new RouteLineView() { DataContext = this };
        }

        public event Action Selected;
        public MapVimo MapVimo { get; private set; }

        public ICollectionView MapLines {
            get { return mapLines; }
            set { SetProperty(ref mapLines, value, OnMapLinesChanged); }
        }

        public void Select(Station station) {
            if (station != null) {
                var lines = station.Segments.SelectMany(n => n.Lines).Distinct().ToList();
                if (lines.Count > 1) {
                    this.station = station;
                    var collectionViewSource = new CollectionViewSource() { Source = lines };
                    MapLines = collectionViewSource.View;
                    MapLines.MoveCurrentToFirst();

                    MapVimo.PropopupService.ShowPopup(view, new Point(station.Left, station.Top));
                }
                else {
                    Selected?.Invoke();
                }
            }
        }

        private void OnMapLinesChanged() {
            MapLines.CurrentChanged += MapLines_CurrentChanged;
        }

        private void MapLines_CurrentChanged(object sender, object e) {
            if (station == null) {
                return;
            }

            var line = MapLines.CurrentItem as Line;
            station.Vimo.LinePath = line;
            Selected?.Invoke();
        }
    }
}
