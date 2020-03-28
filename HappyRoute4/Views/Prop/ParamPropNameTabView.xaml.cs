// <copyright file="ParamPropNameTabView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HappyRoute4.Views.Prop {
    public sealed partial class ParamPropNameTabView : UserControl {
        public ParamPropNameTabView() {
            this.InitializeComponent();
            Loaded += ParamPropNameTabView_Loaded;
        }

        private void ParamPropNameTabView_Loaded(object sender, RoutedEventArgs e) {
            firstFocus.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e) {
            // (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            var viewModel = DataContext as ViewModels.Prop.ParamPropViewModel;
            if (viewModel != null && viewModel.MapVimo.Map != null) {
                viewModel.MapVimo.Map.Name1 = (sender as TextBox).Text;
            }
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e) {
            var viewModel = DataContext as ViewModels.Prop.ParamPropViewModel;
            if (viewModel != null && viewModel.MapVimo.Map != null) {
                viewModel.MapVimo.Map.Name2 = (sender as TextBox).Text;
            }
        }

        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e) {
            var viewModel = DataContext as ViewModels.Prop.ParamPropViewModel;
            if (viewModel != null && viewModel.MapVimo.Map != null) {
                viewModel.MapVimo.Map.Param.Autors = (sender as TextBox).Text;
            }
        }
    }
}
