// <copyright file="MapFile.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using HappyRoute4.Services;
using System.Threading.Tasks;

namespace HappyRoute4.Vimos {
    public class MapFile {
        private AppTitleService appTitleService;
        private MapDataService mapDataService;

        private MapVimo mapVimo;

        public MapFile(
            MapVimo mapVimo,
            AppTitleService appTitleService,
            MapDataService mapDataService) {
            this.mapVimo = mapVimo;
            this.appTitleService = appTitleService;
            this.mapDataService = mapDataService;

            appTitleService.Initialize();
        }

        public bool IsMy { get; private set; }
        public string FileName { get; private set; }

        public async Task New(string newFileName) {
            SaveCurrentMapSettings();
            var map = new Map() { Name1 = newFileName };
            await mapDataService.SaveMap(newFileName, map);
            FileName = newFileName;
            IsMy = true;
            mapVimo.Map = map;
            mapVimo.MapView.Reset();
            mapVimo.IsEdit = true;
        }

        public async Task Open() {
            (FileName, IsMy) = mapDataService.GetSettings();
            mapVimo.Map = await mapDataService.GetMap(FileName, IsMy);
        }

        public async Task Open(string newFileName, bool newIsMy) {
            SaveCurrentMapSettings();
            FileName = newFileName;
            IsMy = newIsMy;
            mapVimo.Map = await mapDataService.GetMap(FileName, IsMy);
            mapVimo.PropopupService.HidePopup();
        }

        public async Task Open(string newFileName) {
            SaveCurrentMapSettings();
            await mapDataService.SaveMap(newFileName, mapVimo.Map);
            FileName = newFileName;
            IsMy = true;
            appTitleService.SetTitle(FileName);
        }

        public async Task Save() {
            mapDataService.SaveSettings(FileName, IsMy);
            mapDataService.SaveMapSettings(FileName, mapVimo.IsName2, mapVimo.MapView.ScrollViewer);
            if (mapVimo.IsEdit && IsMy) {
                await mapDataService.SaveMap(FileName, mapVimo.Map);
            }
        }

        public void Rename(string newFileName) {
            mapDataService.RemoveMapSettings(FileName);
            FileName = newFileName;
            appTitleService.SetTitle(FileName);
        }

        public async Task Delete() {
            mapDataService.RemoveMapSettings(FileName);
            mapVimo.IsEdit = false;
            FileName = Statics.LovelyMapName;
            IsMy = false;
            mapVimo.Map = await mapDataService.GetMap(FileName, IsMy);
        }

        public void Import(string newFileName, Map map) {
            SaveCurrentMapSettings();
            FileName = newFileName;
            IsMy = true;
            mapVimo.Map = map;
        }

        public async void DiscardChanges() {
            mapVimo.Map = await mapDataService.GetMap(FileName, IsMy);
            mapVimo.IsEdit = false;
        }

        public void OnMapOrHostChanged() {
            appTitleService.SetTitle(FileName);
            var isName2 = mapDataService.OpenMapSettings(
                FileName,
                mapVimo.MapView.ScrollViewer,
                mapVimo.MapView.Host.Width,
                mapVimo.MapView.Host.Height);
            if (mapVimo.Map.Param.NumberNames < 2) {
                isName2 = false;
            }

            mapVimo.IsName2 = isName2;
        }

        public async Task SaveMap() {
            if (IsMy) {
                await mapDataService.SaveMap(FileName, mapVimo.Map);
            }
        }

        private void SaveCurrentMapSettings() {
            mapVimo.IsEdit = false;
            mapDataService.SaveMapSettings(FileName, mapVimo.IsName2, mapVimo.MapView.ScrollViewer);
        }
    }
}
