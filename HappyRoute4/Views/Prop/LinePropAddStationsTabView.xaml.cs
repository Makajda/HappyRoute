// <copyright file="LinePropAddStationsTabView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HappyRoute4.Views.Prop {
    public sealed partial class LinePropAddStationsTabView : UserControl {
        public LinePropAddStationsTabView() {
            this.InitializeComponent();
        }

        private void Clear_Click(object sender, RoutedEventArgs e) {
            textNames.Focus(FocusState.Programmatic);
        }
    }
}
