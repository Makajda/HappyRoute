// <copyright file="AppTitleService.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System;
using Windows.UI;

namespace HappyRoute4.Services {
    /// <summary>
    /// Manages of AppTitleBar
    /// </summary>
    public class AppTitleService {
        public void Initialize() {
            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            var color = Color.FromArgb(0xFF, 0x71, 0x71, 0xE3);
            titleBar.BackgroundColor = color;
            titleBar.ButtonBackgroundColor = color;
            titleBar.ButtonHoverBackgroundColor = Colors.LightBlue;
            titleBar.ButtonInactiveBackgroundColor = color;
            titleBar.ButtonPressedBackgroundColor = Colors.LightSkyBlue;
            titleBar.InactiveBackgroundColor = color;
        }

        public void SetTitle(string fileName) {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = fileName;
        }
    }
}
