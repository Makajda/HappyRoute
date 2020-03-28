// <copyright file="LinePropAddLinesView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HappyRoute4.Views.Prop {
    public sealed partial class LinePropAddLinesView : UserControl {
        public LinePropAddLinesView() {
            this.InitializeComponent();
            Loaded += LinePropAddLinesView_Loaded;
        }

        private void LinePropAddLinesView_Loaded(object sender, RoutedEventArgs e) {
            firsFocus.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            firsFocus.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }
    }
}
