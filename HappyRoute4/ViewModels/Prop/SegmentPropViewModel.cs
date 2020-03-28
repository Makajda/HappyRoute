// <copyright file="SegmentPropViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using HappyRoute4.Vimos;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace HappyRoute4.ViewModels.Prop {
    public class SegmentPropViewModel : BindableBase {
        private TabView view;
        private MapVimo mapVimo;
        private List<Tuple<FrameworkElement, object>> r;

        public SegmentPropViewModel(MapVimo mapVimo) {
            this.mapVimo = mapVimo;
            view = new TabView();
        }

        public void Show(Segment segment, Point position) {
            r = new List<Tuple<FrameworkElement, object>>();
            var lines = segment.Lines;

            if (lines.Count == 0) {
                return;
            }

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var res1 = loader.GetString("SegmentPropLine");
            var res2 = loader.GetString("SegmentPropParam");

            int i = 0;
            foreach (var line in lines) {
                var segmentPropLineTabVimo = new SegmentPropLineTabViewModel(mapVimo, segment, i);
                r.Add(new Tuple<FrameworkElement, object>(segmentPropLineTabVimo.View, string.Format("{0} {1}", res1, ++i)));
            }

            var segmentPropParamTabViewModel = new SegmentPropParamTabViewModel(mapVimo, segment);
            r.Add(new Tuple<FrameworkElement, object>(segmentPropParamTabViewModel.View, res2));

            view.ItemsSource = r;

            mapVimo.PropopupService.ShowPopup(view, position);
        }
    }
}
