// <copyright file="MapRoute.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using HappyRoute4.Services;
using HappyRoute4.ViewModels;
using System;

namespace HappyRoute4.Vimos {
    public class MapRoute {
        private MapVimo mapVimo;
        private RouteCalcService routeCalcService;
        private RouteViewModel routeViewModel;
        private RouteLineViewModel routeLineViewModel;
        private AlinerViewModel alinerViewModel;

        public MapRoute(MapVimo mapVimo) {
            this.mapVimo = mapVimo;
            SelectedStations = new SelectedStationsService();
            SelectedStations.Selected += SelectedStations_Selected;
            SelectedStations.UnSelected += SelectedStations_UnSelected;
            routeCalcService = new RouteCalcService();
        }

        public SelectedStationsService SelectedStations { get; private set; }
        public RouteVariants RouteVariant { get; set; } = Statics.RouteVariantDefault;

        public void SetMap() {
            routeCalcService.SetMap(mapVimo.Map);
        }

        public void Reset() {
            routeCalcService.Reset();
        }

        public void ShowRoute() {
            if (routeViewModel == null) {
                routeViewModel = new RouteViewModel(mapVimo);
            }

            routeViewModel.RoutePath = routeCalcService.GetPath();
            routeViewModel.Show();
        }

        public void UnselectLines() {
            foreach (var s in mapVimo.Map.Segments) {
                s.Vimo.Reset(false);
            }

            foreach (var s in mapVimo.Map.Stations) {
                s.Vimo.Reset(false);
            }
        }

        private void SelectedStations_Selected(Station station) {
            if (mapVimo.IsEdit) {
                if (mapVimo.EditMode == EditModes.Align) {
                    if (SelectedStations.IsTwoSelected) {
                        if (alinerViewModel == null) {
                            alinerViewModel = new AlinerViewModel(mapVimo);
                        }

                        alinerViewModel.Show(SelectedStations.Station1, SelectedStations.Station2);
                        SelectedStations.UnselectAll();
                    }
                }
            }
            else {
                if (RouteVariant == RouteVariants.OptimalLine) {
                    if (routeLineViewModel == null) {
                        routeLineViewModel = new RouteLineViewModel(mapVimo);
                        routeLineViewModel.Selected += RouteRecalc;
                    }

                    routeLineViewModel.Select(station);
                }
                else {
                    RouteRecalc();
                }
            }
        }

        private void SelectedStations_UnSelected(Station station_) {
            if (!mapVimo.IsEdit) {
                foreach (var segment in mapVimo.Map.Segments) {
                    segment.Vimo.Reset(false);
                }

                foreach (var station in mapVimo.Map.Stations) {
                    if (RouteVariant == RouteVariants.Happy || (station != SelectedStations.Station1 && station != SelectedStations.Station2)) {
                        station.Vimo.Reset(false);
                    }
                    else {
                        if (RouteVariant != RouteVariants.OptimalLine) {
                            station.Vimo.LinePath = null;
                        }
                    }
                }
            }
        }

        private void RouteRecalc() {
            if (RouteVariant == RouteVariants.Happy) {
                if (SelectedStations.Station2 != null) {
                    SelectedStations.Change(SelectedStations.Station1, null);
                }

                var random = new Random();
                var i = mapVimo.Map.Stations.IndexOf(SelectedStations.Station1);
                int r = random.Next(mapVimo.Map.Stations.Count);
                while (r == i) {
                    r = random.Next(mapVimo.Map.Stations.Count);
                }

                mapVimo.IsReadyShowRoute = routeCalcService.Recalc(SelectedStations.Station1, mapVimo.Map.Stations[r]);
            }
            else {
                if (SelectedStations.IsTwoSelected) {
                    mapVimo.IsReadyShowRoute = routeCalcService.Recalc(SelectedStations.Station1, SelectedStations.Station2);
                }
            }
        }
    }
}
