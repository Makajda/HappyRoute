// <copyright file="MapView.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System;
using HappyRoute4.Common;
using HappyRoute4.Models;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace HappyRoute4.Vimos {
    public class MapView {
        private MapVimo mapVimo;

        private Border borderEdit;
        private TextBlock sizeMap;

        public MapView(MapVimo mapVimo) {
            this.mapVimo = mapVimo;
            borderEdit = new Border() { BorderThickness = new Thickness(0.5d), BorderBrush = new SolidColorBrush(Colors.Gray), Visibility = Visibility.Collapsed };
            sizeMap = new TextBlock() { Foreground = new SolidColorBrush(Colors.Gray), Visibility = Visibility.Collapsed };
        }

        public ScrollViewer ScrollViewer { get; private set; }
        public Canvas Host { get; private set; }

        public void SetUI(ScrollViewer scrollViewer) {
            ScrollViewer = scrollViewer;
            Host = scrollViewer.Content as Canvas;
            Host.Tapped += Host_Tapped;
        }

        public void RecalcHostSize() {
            if (mapVimo.Map == null) {
                return;
            }

            double freeArea = mapVimo.IsEdit ? 188d : 33d; // pourquoi pas
            var size = mapVimo.Map.RecalcSize(freeArea);

            borderEdit.Width = size.Width;
            borderEdit.Height = size.Height;
            Canvas.SetLeft(borderEdit, freeArea);
            Canvas.SetTop(borderEdit, freeArea);

            sizeMap.Text = string.Format("{0:0}x{1:0}", size.Width, size.Height);
            Canvas.SetLeft(sizeMap, size.Width + freeArea);
            Canvas.SetTop(sizeMap, size.Height + freeArea);

            Host.Width = size.Width + freeArea + freeArea;
            Host.Height = size.Height + freeArea + freeArea;
        }

        public void Reset() {
            ScrollViewer.ChangeView(0d, 0d, 1f);
        }

        public void OnMapOrHostChanged() {
            Host.Children.Clear();
            Host.Children.Add(borderEdit);
            Host.Children.Add(sizeMap);

            Host.Background = mapVimo.Map.Param.Background;

            foreach (var segment in mapVimo.Map.Segments) {
                segment.Vimo = new SegmentVimo(segment, mapVimo);
            }

            foreach (var station in mapVimo.Map.Stations) {
                station.Vimo = new StationVimo(station, mapVimo);
            }

            RecalcHostSize();
        }

        public void OnIsEditChanged() {
            if (mapVimo.IsEdit) {
                borderEdit.Visibility = Visibility.Visible;
                sizeMap.Visibility = Visibility.Visible;
            }
            else {
                borderEdit.Visibility = Visibility.Collapsed;
                sizeMap.Visibility = Visibility.Collapsed;
            }

            RecalcHostSize();
        }

        private void Host_Tapped(object sender, TappedRoutedEventArgs e) {
            if (mapVimo.Map == null) {
                return;
            }

            if (mapVimo.IsEdit && mapVimo.EditMode == EditModes.Create) {
                var position = e.GetPosition(Host);
                var stationNew = new Station() { Left = position.X, Top = position.Y };
                mapVimo.Map.Stations.Add(stationNew);
                stationNew.Vimo = new StationVimo(stationNew, mapVimo);
                RecalcHostSize();
            }
        }
    }
}
