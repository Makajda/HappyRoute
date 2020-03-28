// <copyright file="MapDataService.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using HappyRoute4.Repositories;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace HappyRoute4.Services {
    public class MapDataService {
        private const string fileNameKey = "Name";
        private const string isMyKey = "IsMy";

        public (string, bool) GetSettings() {
            string fileName = null;
            bool isMy = false;

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey(fileNameKey)) {
                fileName = (string)localSettings.Values[fileNameKey];
            }

            if (string.IsNullOrWhiteSpace(fileName)) {
                fileName = Statics.LovelyMapName;
            }

            if (localSettings.Values.ContainsKey(isMyKey)) {
                isMy = (bool)localSettings.Values[isMyKey];
            }
            else {
                isMy = false;
            }

            return (fileName, isMy);
        }

        public void SaveSettings(string fileName, bool isMy) {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (string.IsNullOrWhiteSpace(fileNameKey)) {
                localSettings.Values.Remove(fileNameKey);
            }
            else {
                localSettings.Values[fileNameKey] = fileName;
            }

            if (isMy) {
                localSettings.Values[isMyKey] = isMy;
            }
            else {
                localSettings.Values.Remove(isMyKey);
            }
        }

        public async Task<Map> GetMap(string fileName, bool isMy) {
            Map map;
            try {
                map = await MapRepository.Open(fileName, isMy);
            }
            catch (Exception exception) {
                var title = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("MapVimoOpenError");
                await AlertMessageService.ShowAsync(exception.Message, $"{title} - {fileName}");
                map = new Map() { Name1 = fileName };
            }

            return map;
        }

        public async Task SaveMap(string fileName, Map map) {
            try {
                await MapRepository.Save(map, fileName);
            }
            catch (Exception exception) {
                var title = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("MapVimoSaveError");
                await AlertMessageService.ShowAsync(exception.Message, $"{title} - {fileName}");
            }
        }

        public bool OpenMapSettings(string fileName, ScrollViewer scrollViewer, double width, double height) {
            bool isName2 = false;

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            var nameSetting = fileName + ":N";
            if (localSettings.Values.ContainsKey(nameSetting)) {
                isName2 = (bool)localSettings.Values[nameSetting];
            }

            float zoom = 0f;
            double horizontalOffset = 0d, verticalOffset = 0d;

            nameSetting = fileName + ":Z";
            if (localSettings.Values.ContainsKey(nameSetting)) {
                zoom = ((float)localSettings.Values[nameSetting] / 100f) + 1f;
            }
            else {
                if (width > double.Epsilon && height > double.Epsilon) {
                    zoom = Math.Min((float)(scrollViewer.ViewportWidth / width), (float)(scrollViewer.ViewportHeight / height));
                }
            }

            nameSetting = fileName + ":H";
            if (localSettings.Values.ContainsKey(nameSetting)) {
                horizontalOffset = (double)localSettings.Values[nameSetting];
            }

            nameSetting = fileName + ":V";
            if (localSettings.Values.ContainsKey(nameSetting)) {
                verticalOffset = (double)localSettings.Values[nameSetting];
            }

            scrollViewer.UpdateLayout();
            scrollViewer.ChangeView(horizontalOffset, verticalOffset, zoom);

            return isName2;
        }

        public void SaveMapSettings(string fileName, bool isName2, ScrollViewer scrollViewer) {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            var nameSetting = fileName + ":N";
            if (isName2) {
                localSettings.Values[nameSetting] = isName2;
            }
            else {
                localSettings.Values.Remove(nameSetting);
            }

            nameSetting = fileName + ":Z";
            localSettings.Values[nameSetting] = (float)Math.Round((scrollViewer.ZoomFactor - 1f) * 100f, 0d);

            nameSetting = fileName + ":H";
            if (Math.Abs(scrollViewer.HorizontalOffset) < double.Epsilon) {
                localSettings.Values.Remove(nameSetting);
            }
            else {
                localSettings.Values[nameSetting] = scrollViewer.HorizontalOffset;
            }

            nameSetting = fileName + ":V";
            if (Math.Abs(scrollViewer.VerticalOffset) < double.Epsilon) {
                localSettings.Values.Remove(nameSetting);
            }
            else {
                localSettings.Values[nameSetting] = scrollViewer.VerticalOffset;
            }
        }

        public void RemoveMapSettings(string fileName) {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Action<string> remove = (key) => {
                var name = $"{fileName}:{key}";
                if (localSettings.Values.ContainsKey(name)) {
                    localSettings.Values.Remove(name);
                }
            };

            remove("N");
            remove("Z");
            remove("H");
            remove("V");
        }
    }
}
