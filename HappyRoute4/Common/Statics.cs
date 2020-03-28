// <copyright file="Statics.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HappyRoute4.Common {
    public static class Statics {
        public const string LovelyMapName = "New York City Subway";
        public const string DataFolderName = "Data";
        public const string FileExtension = ".hro";
        public const char IsMyChar = '+';

        public static RouteVariants RouteVariantDefault { get; set; } = RouteVariants.Optimal;

        public static object GetCurrentContent() {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null) {
                return null;
            }
            else {
                return rootFrame.Content;
            }
        }
    }
}
