// <copyright file="PropopupService.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace HappyRoute4.Services {
    public class PropopupService {
        private Propopup propopup;

        public PropopupService() {
        }

        public void SetHost(FrameworkElement host) {
            propopup = new Propopup(host);
        }

        public void ShowPopup(FrameworkElement view) {
            propopup.Show(view);
        }

        public void ShowPopup(FrameworkElement view, Point point) {
            propopup.Show(view, point);
        }

        public void ShowPopup(FrameworkElement view, bool isMaxSize) {
            propopup.Show(view, isMaxSize);
        }

        public void HidePopup() {
            propopup.Hide();
        }
    }
}
