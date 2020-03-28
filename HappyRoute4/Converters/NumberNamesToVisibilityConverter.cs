// <copyright file="NumberNamesToVisibilityConverter.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HappyRoute4.Converters {
    public sealed class NumberNamesToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return (value is double && (double)value < 1.5d) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
