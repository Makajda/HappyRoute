// <copyright file="SegmentPropLineTabView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace HappyRoute4.Views.Prop {
    public sealed partial class SegmentPropLineTabView : UserControl {
        public SegmentPropLineTabView() {
            this.InitializeComponent();
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            var collectionView = list.ItemsSource as ICollectionView;
            collectionView?.MoveCurrentTo(e.ClickedItem);
        }
    }
}
