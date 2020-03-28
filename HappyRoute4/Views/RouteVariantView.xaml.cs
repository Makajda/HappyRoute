// <copyright file="RouteVariantView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using Microsoft.Services.Store.Engagement;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace HappyRoute4.Views {
    public sealed partial class RouteVariantView : UserControl {
        private bool might;

        public RouteVariantView() {
            this.InitializeComponent();
            Loaded += RouteVariantView_Loaded;
        }

        private void RouteVariantView_Loaded(object sender, RoutedEventArgs e) {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            var routeVariant = Statics.RouteVariantDefault;
            if (localSettings.Values.ContainsKey("RouteVariant")) {
                routeVariant = (RouteVariants)localSettings.Values["RouteVariant"];
            }

            might = false;

            switch (routeVariant) {
                case RouteVariants.Happy:
                    rbHappy.IsChecked = true;
                    break;
                case RouteVariants.Optimal:
                    rbOptimal.IsChecked = true;
                    break;
                case RouteVariants.OptimalLine:
                    rbOptimalLine.IsChecked = true;
                    break;
                default:
                    break;
            }

            might = true;
        }

        private void Checked(RouteVariants routeVariant) {
            if (might) {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (routeVariant != Statics.RouteVariantDefault) {
                    localSettings.Values["RouteVariant"] = (int)routeVariant;
                }
                else {
                    localSettings.Values.Remove("RouteVariant");
                }

                var viewModel = DataContext as Vimos.MapRoute;
                viewModel.RouteVariant = routeVariant;
            }
        }

        private void Happy_Checked(object sender, RoutedEventArgs e) {
            Checked(RouteVariants.Happy);
        }

        private void Optimal_Checked(object sender, RoutedEventArgs e) {
            Checked(RouteVariants.Optimal);
        }

        private void OptimalLine_Checked(object sender, RoutedEventArgs e) {
            Checked(RouteVariants.OptimalLine);
        }

        private async void Feedback_Tapped(object sender, TappedRoutedEventArgs e) {
            try {
                if (StoreServicesFeedbackLauncher.IsSupported()) {
                    var launcher = StoreServicesFeedbackLauncher.GetDefault();
                    await launcher.LaunchAsync();
                }
            }
            catch (Exception) { }
        }
    }
}
