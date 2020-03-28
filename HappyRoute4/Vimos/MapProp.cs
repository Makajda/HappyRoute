// <copyright file="MapProp.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using HappyRoute4.ViewModels;
using HappyRoute4.ViewModels.Prop;
using HappyRoute4.Views;
using Windows.Foundation;

namespace HappyRoute4.Vimos {
    public class MapProp {
        private MapVimo mapVimo;
        private ShowLinesViewModel showLinesViewModel;
        private ParamPropViewModel paramPropViewModel;
        private StationPropViewModel stationPropViewModel;
        private SegmentPropViewModel segmentPropViewModel;

        private FileViewModel fileViewModel;

        public MapProp(MapVimo mapVimo) {
            this.mapVimo = mapVimo;
        }

        public void Donate() {
            var storeHelperView = new StoreHelperView();
            mapVimo.PropopupService.ShowPopup(storeHelperView);
        }

        public void ShowRouterVariant() {
            var routerVariantView = new RouteVariantView() { DataContext = mapVimo.MapRoute };
            mapVimo.PropopupService.ShowPopup(routerVariantView);
        }

        public void ShowCatalog() {
            var catalogView = new CatalogView() { DataContext = mapVimo };
            mapVimo.PropopupService.ShowPopup(catalogView, true);
        }

        public void EditOn() {
            if (mapVimo.MapFile.IsMy) {
                mapVimo.IsEdit = true;
            }
            else {
                if (fileViewModel == null) {
                    fileViewModel = new FileViewModel(mapVimo);
                }

                fileViewModel.CopyToMy();
            }
        }

        public void FileMenu() {
            if (fileViewModel == null) {
                fileViewModel = new FileViewModel(mapVimo);
            }

            fileViewModel.Show();
        }

        public void ShowLines() {
            if (mapVimo.IsEdit) {
                if (showLinesViewModel == null) {
                    showLinesViewModel = new ShowLinesViewModel(mapVimo);
                }

                showLinesViewModel.Show();
            }
        }

        public void ShowParamProp() {
            if (paramPropViewModel == null) {
                paramPropViewModel = new ParamPropViewModel(mapVimo);
            }

            paramPropViewModel.Show();
        }

        public void ShowSegmentProp(Segment segment, Point position) {
            if (mapVimo.IsEdit) {
                if (segmentPropViewModel == null) {
                    segmentPropViewModel = new SegmentPropViewModel(mapVimo);
                }

                segmentPropViewModel.Show(segment, position);
            }
        }

        public void ShowStationProp(Station station) {
            if (mapVimo.IsEdit) {
                if (stationPropViewModel == null) {
                    stationPropViewModel = new StationPropViewModel(mapVimo);
                }

                stationPropViewModel.Show(station);
            }
        }
    }
}
