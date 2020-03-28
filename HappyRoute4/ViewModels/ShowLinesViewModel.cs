// <copyright file="ShowLinesViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using HappyRoute4.ViewModels.Prop;
using HappyRoute4.Views;
using HappyRoute4.Vimos;
using Prism.Commands;
using Prism.Mvvm;
using Windows.UI.Xaml.Data;

namespace HappyRoute4.ViewModels {
    public class ShowLinesViewModel : BindableBase {
        private ShowLinesView view;
        private LinePropViewModel linePropViewModel;
        private LinePropAddLinesViewModel addLinesViewModel;
        private ICollectionView mapLines;

        public ShowLinesViewModel(MapVimo mapVimo) {
            MapVimo = mapVimo;
            linePropViewModel = new LinePropViewModel(MapVimo);
            addLinesViewModel = new LinePropAddLinesViewModel(MapVimo);
            AddCommand = new DelegateCommand(addLinesViewModel.Show);
            view = new ShowLinesView() { DataContext = this };
        }

        public MapVimo MapVimo { get; private set; }
        public DelegateCommand AddCommand { get; private set; }

        public ICollectionView MapLines {
            get { return mapLines; }
            set { SetProperty(ref mapLines, value); }
        }

        public void ShowLineProp() {
            var line = MapLines.CurrentItem as Line;
            linePropViewModel.Show(line);
        }

        public void Show() {
            if (MapVimo.Map == null) {
                return;
            }

            var collectionViewSource = new CollectionViewSource() { Source = MapVimo.Map.Lines };
            MapLines = collectionViewSource.View;
            MapLines.MoveCurrentToFirst();
            MapVimo.PropopupService.ShowPopup(view);
        }
    }
}
