// <copyright file="Extensions.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HappyRoute4.Common {
    public static class Extensions {
        public static void BringToFront(this FrameworkElement element) {
            var panel = element.Parent as Canvas;
            if (panel == null) {
                return;
            }

            var max = panel.Children.OfType<UIElement>()
                .Where(x => x != element)
                .Select(x => Canvas.GetZIndex(x))
                .Max();

            Canvas.SetZIndex(element, max + 1);
        }

        public static void SetValidFileName(this TextBox textBox) {
            var pos = textBox.SelectionStart;
            var text = textBox.Text;
            char[] charInvalidFileChars = Path.GetInvalidFileNameChars();
            foreach (char charInvalid in charInvalidFileChars) {
                text = text.Replace(charInvalid, '_');
            }

            textBox.Text = text;
            textBox.SelectionStart = pos;
        }

        // http://msdn.microsoft.com/en-us/library/bb613579.aspx
        public static childItem FindVisualChild<childItem>(this DependencyObject obj)
            where childItem : DependencyObject {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++) {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem) {
                    return (childItem)child;
                }
                else {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null) {
                        return childOfChild;
                    }
                }
            }

            return null;
        }
    }
}