// <copyright file="MainPage.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace HappyRoute4.Views {
    public sealed partial class MainPage : Page {
        public MainPage() {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
        }

#if DEBUG
        protected override void OnKeyDown(KeyRoutedEventArgs e) {
            base.OnKeyDown(e);
            if (e.Key == Windows.System.VirtualKey.Escape) {
                App.Current.Exit();
            }
        }
#endif

        private void MainPage_Loaded(object sender, RoutedEventArgs e) {
            var viewModel = DataContext as ViewModels.MainPageViewModel;
            viewModel?.MapVimo.SetUI(scrollViewer);
        }
    }
}
