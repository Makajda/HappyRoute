﻿// <copyright file="LinePropNameTabView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Windows.UI.Xaml.Controls;

namespace HappyRoute4.Views.Prop {
    public sealed partial class LinePropNameTabView : UserControl {
        public LinePropNameTabView() {
            this.InitializeComponent();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e) {
            // (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            var viewModel = DataContext as ViewModels.Prop.LinePropViewModel;
            if (viewModel != null) {
                viewModel.Line.Name1 = (sender as TextBox).Text;
            }
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e) {
            var viewModel = DataContext as ViewModels.Prop.LinePropViewModel;
            if (viewModel != null) {
                viewModel.Line.Name2 = (sender as TextBox).Text;
            }
        }
    }
}
