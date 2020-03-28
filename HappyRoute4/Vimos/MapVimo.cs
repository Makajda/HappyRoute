// <copyright file="MapVimo.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using HappyRoute4.Services;
using Prism.Mvvm;
using Windows.UI.Xaml.Controls;

namespace HappyRoute4.Vimos {
    public class MapVimo : BindableBase {
        private bool isMapOrHostChanging;
        private Map map;
        private bool isName2;
        private bool isEdit;
        private EditModes editMode;
        private bool isReadyShowRoute;

        public MapVimo(
            AppTitleService appTitleService,
            MapDataService mapDataService,
            SearchStationService searchStationService,
            PropopupService propopupService) {
            SearchStationService = searchStationService;
            PropopupService = propopupService;
            MapView = new MapView(this);
            MapRoute = new MapRoute(this);
            MapProp = new MapProp(this);
            MapFile = new MapFile(this, appTitleService, mapDataService);
        }

        public SearchStationService SearchStationService { get; private set; }
        public PropopupService PropopupService { get; private set; }
        public MapView MapView { get; private set; }
        public MapRoute MapRoute { get; private set; }
        public MapProp MapProp { get; private set; }
        public MapFile MapFile { get; private set; }

        public Map Map {
            get { return map; }
            set { SetProperty(ref map, value, OnMapOrHostChanged); }
        }

        public bool IsName2 {
            get { return isName2; }
            set { SetProperty(ref isName2, value, OnIsName2Changed); }
        }

        public bool IsEdit {
            get { return isEdit; }
            set { SetProperty(ref isEdit, value, OnIsEditChanged); }
        }

        public EditModes EditMode {
            get { return editMode; }
            set { SetProperty(ref editMode, value, MapRoute.SelectedStations.UnselectAll); }
        }

        public bool IsReadyShowRoute {
            get { return isReadyShowRoute; }
            set { SetProperty(ref isReadyShowRoute, value); }
        }

        public void SetUI(ScrollViewer scrollViewer) {
            MapView.SetUI(scrollViewer);
            PropopupService.SetHost(MapView.Host);
            OnMapOrHostChanged();
        }

        public void ClearSelection() {
            MapRoute.UnselectLines();
            MapRoute.SelectedStations.UnselectAll();
        }

        private void OnMapOrHostChanged() {
            if (Map == null || MapView.Host == null) {
                return;
            }

            if (isMapOrHostChanging) {
                return;
            }

            isMapOrHostChanging = true;

            IsReadyShowRoute = false;
            MapRoute.SelectedStations.UnselectAll();
            MapView.OnMapOrHostChanged(); // 1
            MapFile.OnMapOrHostChanged(); // 2
            SearchStationService.SetMap(Map);
            MapRoute.SetMap();

            isMapOrHostChanging = false;
        }

        private void OnIsName2Changed() {
            foreach (var station in Map.Stations) {
                station.Vimo.SetIsName2();
            }
        }

        private async void OnIsEditChanged() {
            MapRoute.SelectedStations.UnselectAll();
            MapView.OnIsEditChanged();
            IsReadyShowRoute = false;
            MapRoute.Reset();
            if (!IsEdit) {
                await MapFile.SaveMap();
            }
        }
    }
}
