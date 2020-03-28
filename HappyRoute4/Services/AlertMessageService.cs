// <copyright file="AlertMessageService.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace HappyRoute4.Services {
    /// <summary>
    /// Shows a message
    /// </summary>
    public static class AlertMessageService {
        private static bool isShowing = false;

        public static async Task ShowAsync(string message, string title = null, [CallerMemberName] string memberName = null) {
            // Only show one dialog at a time.
            if (!isShowing) {
                var messageDialog = new MessageDialog(message, title ?? memberName);
                isShowing = true;
                await messageDialog.ShowAsync();
                isShowing = false;
            }
        }
    }
}
