// <copyright file="RouteViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using HappyRoute4.Services;
using HappyRoute4.Views;
using HappyRoute4.Vimos;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace HappyRoute4.ViewModels {
    public class RouteViewModel : BindableBase {
        private RouteView view;
        private double totalWeight;
        private IEnumerable<RouteHop> routePath;

        public RouteViewModel(MapVimo mapVimo) {
            MapVimo = mapVimo;
            view = new RouteView() { DataContext = this };
        }

        public MapVimo MapVimo { get; private set; }

        public double TotalWeight {
            get { return totalWeight; }
            set { SetProperty(ref totalWeight, value); }
        }

        public IEnumerable<RouteHop> RoutePath {
            get { return routePath; }
            set { SetProperty(ref routePath, value, () => TotalWeight = routePath?.Last().Runs ?? 0d); }
        }

        public void Show() {
            MapVimo.PropopupService.ShowPopup(view);
        }
    }
}
