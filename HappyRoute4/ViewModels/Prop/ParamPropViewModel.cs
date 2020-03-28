// <copyright file="ParamPropViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Views.Prop;
using HappyRoute4.Vimos;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace HappyRoute4.ViewModels.Prop {
    public class ParamPropViewModel : BindableBase {
        private TabView view;
        private ParamPropNameTabView viewName;
        private ParamPropThickTabView viewParamers;
        private ParamPropColorsTabView viewColors;
        private ICollectionView paramList;

        public ParamPropViewModel(MapVimo mapVimo) {
            this.MapVimo = mapVimo;

            viewName = new ParamPropNameTabView() { DataContext = this };
            viewParamers = new ParamPropThickTabView() { DataContext = this };
            viewColors = new ParamPropColorsTabView() { DataContext = this };

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var res1 = loader.GetString("ParamPropNames");
            var res2 = loader.GetString("ParamPropThick");
            var res3 = loader.GetString("ParamPropColors");

            var r = new List<Tuple<FrameworkElement, object>>();
            r.Add(new Tuple<FrameworkElement, object>(viewName, res1));
            r.Add(new Tuple<FrameworkElement, object>(viewParamers, res2));
            r.Add(new Tuple<FrameworkElement, object>(viewColors, res3));

            view = new TabView() { ItemsSource = r };
        }

        public MapVimo MapVimo { get; private set; }

        public ICollectionView ParamList {
            get { return paramList; }
            set { SetProperty(ref paramList, value); }
        }

        public void Show() {
            if (MapVimo.Map != null) {
                DefineNewParam();
                MapVimo.PropopupService.ShowPopup(view);
            }
        }

        public void ShowColorer() {
            var tuple = ParamList.CurrentItem as Tuple<Color, string, string>;
            if (tuple != null) {
                var colorPicker = new ColorPicker() {
                    IsMoreButtonVisible = true,
                    IsColorSliderVisible = true,
                    IsColorChannelTextInputVisible = true,
                    IsHexInputVisible = true,
                    IsAlphaEnabled = true,
                    IsAlphaSliderVisible = false,
                    IsAlphaTextInputVisible = true,
                    ColorSpectrumShape = ColorSpectrumShape.Ring,
                    Margin = new Thickness(30d),
                };

                colorPicker.Visibility = Visibility.Visible;
                var binding = new Binding() {
                    Path = new PropertyPath(tuple.Item2),
                    Source = MapVimo.Map.Param,
                    Mode = BindingMode.TwoWay
                };

                colorPicker.SetBinding(ColorPicker.ColorProperty, binding);
                MapVimo.PropopupService.ShowPopup(colorPicker);
            }
        }

        private void DefineNewParam() {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var res1 = loader.GetString("ParamPropBackgroundColor");
            var res2 = loader.GetString("ParamPropTransferColor");
            var res3 = loader.GetString("ParamPropDimedColor");
            var res4 = loader.GetString("ParamPropBuildColor");

            var list = new List<Tuple<Color, string, string>>();
            if (MapVimo.Map != null) {
                list.Add(new Tuple<Color, string, string>(MapVimo.Map.Param.Background.Color, "Background.Color", res1));
                list.Add(new Tuple<Color, string, string>(MapVimo.Map.Param.TransferColor, "TransferColor", res2));
                list.Add(new Tuple<Color, string, string>(MapVimo.Map.Param.DimedColor, "DimedColor", res3));
                list.Add(new Tuple<Color, string, string>(MapVimo.Map.Param.BuildColor, "BuildColor", res4));
                var collectionViewSource = new CollectionViewSource() { Source = list };
                ParamList = collectionViewSource.View;
                MapVimo.Map.Param.PropertyChanged += Param_PropertyChanged;
            }
        }

        private void Param_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (MapVimo.Map != null) {
                switch (e.PropertyName) {
                    case "TransferColor":
                        foreach (var station in MapVimo.Map.Stations) {
                            station.Vimo.ChangeShape();
                        }

                        break;
                    case "BuildColor":
                        foreach (var station in MapVimo.Map.Stations) {
                            station.Vimo.ChangeShape();
                        }

                        foreach (var segment in MapVimo.Map.Segments) {
                            segment.Vimo.ChangeShape();
                        }

                        break;
                    case "Background.Color":
                    case "DimedColor":
                    case "Autor":
                    case "NumberNames":
                        break;
                    case "TransferWeight":
                        MapVimo.MapRoute.Reset();
                        break;
                    default:
                        foreach (var station in MapVimo.Map.Stations) {
                            station.Vimo.Resize();
                        }

                        foreach (var segment in MapVimo.Map.Segments) {
                            segment.Vimo.Relocation();
                        }

                        MapVimo.MapView.RecalcHostSize();
                        break;
                }
            }
        }
    }
}
