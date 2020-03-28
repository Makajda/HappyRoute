// <copyright file="FileNameView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HappyRoute4.Views {
    public sealed partial class FileNameView : UserControl {
        public FileNameView() {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            name.Focus(FocusState.Programmatic);
            name.SelectAll();
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e) {
            name.SetValidFileName();
            var viewModel = DataContext as ViewModels.FileViewModel;
            if (viewModel != null) {
                viewModel.IsAlreadyName = false;
            }
        }
    }
}
