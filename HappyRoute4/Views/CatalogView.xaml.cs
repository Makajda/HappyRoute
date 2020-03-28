// <copyright file="CatalogView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using HappyRoute4.Repositories;
using HappyRoute4.Vimos;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HappyRoute4.Views {
    public sealed partial class CatalogView : UserControl {
        public CatalogView() {
            this.InitializeComponent();
            Loaded += CatalogView_Loaded;
        }

        private async void CatalogView_Loaded(object sender, RoutedEventArgs e) {
            gridView.Focus(FocusState.Programmatic);
            var source = await CatalogRepository.GetData();
            cvs.Source = source;
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e) {
            var mapVimo = DataContext as MapVimo;
            var schema = (Schema)e.ClickedItem;
            if (mapVimo != null && schema != null) {
                await mapVimo.MapFile.Open(schema.Name, schema.IsMy);
            }
        }
    }
}
