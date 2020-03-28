// <copyright file="SearchStationService.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRoute4.Services {
    public class SearchStationService : BindableBase {
        private string searchText;
        private IEnumerable<string> autoSuggestSource;
        private Map map;
        private bool isName2;

        public string SearchText {
            get { return searchText; }
            set { SetProperty(ref searchText, value, OnSearchChanged); }
        }

        public IEnumerable<string> AutoSuggestSource {
            get { return autoSuggestSource; }
            set { SetProperty(ref autoSuggestSource, value); }
        }

        public void SetMap(Map map) {
            this.map = map;
            OnSearchChanged();
        }

        public void SetIsName2(bool isName2) {
            this.isName2 = isName2;
            OnSearchChanged();
        }

        private void OnSearchChanged() {
            if (map == null) {
                return;
            }

            IEnumerable<string> searchSuggest = null;

            if (string.IsNullOrEmpty(SearchText)) {
                foreach (var station in map.Stations) {
                    station.Vimo.IsVisible = true;
                }

                foreach (var segment in map.Segments) {
                    segment.Vimo.IsVisible = true;
                }
            }
            else {
                foreach (var station in map.Stations) {
                    var name = isName2 ? station.Name2 : station.Name1;
                    station.Vimo.IsVisible = !string.IsNullOrEmpty(name) && name.StartsWith(SearchText, StringComparison.CurrentCultureIgnoreCase);
                }

                foreach (var segment in map.Segments) {
                    segment.Vimo.IsVisible = false;
                }

                searchSuggest = map.Stations.Select(n => isName2 ? n.Name2 : n.Name1)
                    .Where(n => !string.IsNullOrEmpty(n) && n.StartsWith(SearchText, StringComparison.CurrentCultureIgnoreCase)).Take(5);
            }

            AutoSuggestSource = searchSuggest;
        }
    }
}
