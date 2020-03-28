// <copyright file="SegmentPropLineTabViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using HappyRoute4.Views.Prop;
using HappyRoute4.Vimos;
using Prism.Commands;
using Prism.Mvvm;
using Windows.UI.Xaml.Data;

namespace HappyRoute4.ViewModels.Prop {
    public class SegmentPropLineTabViewModel : BindableBase {
        private Segment segment;
        private int iLine;
        private ICollectionView mapLines;

        public SegmentPropLineTabViewModel(MapVimo mapVimo, Segment segment, int iLine) {
            this.MapVimo = mapVimo;
            this.segment = segment;
            this.iLine = iLine;

            RemoveCommand = new DelegateCommand(Remove);

            var collectionViewSource = new CollectionViewSource() { Source = mapVimo.Map.Lines };
            MapLines = collectionViewSource.View;
            MapLines.MoveCurrentTo(segment.Lines[iLine]);

            View = new SegmentPropLineTabView() { DataContext = this };
        }

        public SegmentPropLineTabView View { get; private set; }
        public MapVimo MapVimo { get; private set; }
        public DelegateCommand RemoveCommand { get; private set; }

        public ICollectionView MapLines {
            get { return mapLines; }
            set { SetProperty(ref mapLines, value, OnMapLinesChanged); }
        }

        private void OnMapLinesChanged() {
            MapLines.CurrentChanged += MapLines_CurrentChanged;
        }

        private void MapLines_CurrentChanged(object sender, object e) {
            if (segment == null) {
                return;
            }

            var newLine = MapLines.CurrentItem as Line;
            segment.Vimo.ChangeLine(iLine, newLine);
        }

        private void Remove() {
            if (segment == null) {
                return;
            }

            if (segment.Lines.Count == 1) {
                segment.Vimo.Remove();
            }
            else {
                segment.Vimo.Remove(iLine);
            }

            MapVimo.PropopupService.HidePopup(); // r must observableCollection for correct remove in tabView
        }
    }
}
