// <copyright file="BnezConverter.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HappyRoute4.Converters {
    /// <summary>
    /// Convert bool, bool?, int, string and object to Visibility. Parameter "reverse" uses for reverse Visibility.
    /// </summary>
    public sealed class BnezConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            bool r = false;

            if (value is bool) {
                r = (bool)value;
            }
            else if (value is bool?) {
                r = (bool?)value ?? false;
            }
            else if (value is int) {
                r = ((int)value) != 0;
            }
            else {
                r = value != null && value.ToString().Length > 0;
            }

            if (parameter == null) {
                return r ? Visibility.Visible : Visibility.Collapsed;
            }

            var p = parameter.ToString();
            if (p.ToLower() == "reverse") {
                return r ? Visibility.Collapsed : Visibility.Visible;
            }

            // you can use invalid parameter for always visible
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
