// <copyright file="ParamPropColorsTabView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace HappyRoute4.Views.Prop {
    public sealed partial class ParamPropColorsTabView : UserControl {
        public ParamPropColorsTabView() {
            this.InitializeComponent();
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e) {
            base.OnKeyDown(e);
            switch (e.Key) {
                case VirtualKey.Escape:
                    Close();
                    e.Handled = true;
                    break;
            }
        }

        private void Close() {
            var element = this as FrameworkElement;
            while (element != null) {
                var popup = element.Parent as Popup;
                if (popup != null) {
                    popup.IsOpen = false;
                    return;
                }

                element = element.Parent as FrameworkElement;
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            var collectionView = list.ItemsSource as ICollectionView;
            if (collectionView != null) {
                collectionView.MoveCurrentTo(e.ClickedItem);
                Close();

                var viewModel = DataContext as ViewModels.Prop.ParamPropViewModel;
                if (viewModel != null) {
                    viewModel.ShowColorer();
                }
            }
        }
    }
}
