// <copyright file="StoreHelperView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Microsoft.Services.Store.Engagement;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace HappyRoute4.Common {
    public sealed partial class StoreHelperView : UserControl {
        public StoreHelperView() {
            this.InitializeComponent();
        }

        public static void Show() {
            var popup = new Popup() { IsLightDismissEnabled = true };
            var child = new StoreHelperView();
            if (!StoreServicesFeedbackLauncher.IsSupported()) {
                child.feedbackButton.Visibility = Visibility.Collapsed;
            }

            popup.Child = child;
            popup.IsOpen = true;

            child.MaxWidth = Window.Current.Bounds.Width;
            child.MaxHeight = Window.Current.Bounds.Height;
            child.UpdateLayout();
            popup.HorizontalOffset = (Window.Current.Bounds.Width - child.ActualWidth) / 2d;
            popup.VerticalOffset = (Window.Current.Bounds.Height - child.ActualHeight) / 2d;
        }

        private async Task LaunchUri(string uriString) {
            try {
                var uri = new Uri(uriString);
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
            catch (Exception) { }
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

        private async void SourceCode_Tapped(object sender, TappedRoutedEventArgs e) {
            await LaunchUri("https://github.com/makajda/");
        }

        private async void LinkedIn_Tapped(object sender, TappedRoutedEventArgs e) {
            await LaunchUri("https://www.linkedin.com/in/vladimirmakayda/");
        }
    }
}
