// <copyright file="App.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Prism.Unity.Windows;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace HappyRoute4 {
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : PrismUnityApplication {
        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args) {
            NavigationService.Navigate("Main", null);

            Window.Current.Activate();
            return Task.FromResult<object>(null);
        }
    }
}
