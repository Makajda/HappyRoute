// <copyright file="RemoveBracketConverter.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using Windows.UI.Xaml.Data;

namespace HappyRoute4.Converters {
    public sealed class RemoveBracketConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            if (value == null) {
                return null;
            }

            var b = new StringBuilder(value.ToString());
            int i = 0;
            while (i < b.Length) {
                if (b[i] == '{') {
                    while (i < b.Length && b[i] != '}') {
                        b.Remove(i, 1);
                    }

                    if (i < b.Length) {
                        b.Remove(i, 1);
                    }
                }
                else {
                    i++;
                }
            }

            return b.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
