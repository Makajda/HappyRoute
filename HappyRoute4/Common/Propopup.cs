// <copyright file="Propopup.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace HappyRoute4.Common {
    public class Propopup {
        private const double indent = 88d;
        private const double opacity = 90d;
        private const double borderThick = 1d;

        private Popup popup;
        private ScrollViewer container;
        private FrameworkElement host;

        public Propopup()
            : this(null) {
        }

        public Propopup(FrameworkElement host) {
            this.host = host;

            var border = new Border() {
                Opacity = opacity,
                BorderThickness = new Thickness(borderThick),
                Background = App.Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush,
                BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x71, 0x71, 0xE3))
            };

            container = new ScrollViewer() {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            border.Child = container;
            popup = new Popup() { IsLightDismissEnabled = true, Child = border };

            container.SizeChanged += Content_SizeChanged;
        }

        public void Show(FrameworkElement view) {
            popup.Tag = null;
            ShowReal(view);
        }

        public void Show(FrameworkElement view, Point point) {
            popup.Tag = point;
            ShowReal(view);
        }

        // on window
        public void Show(FrameworkElement view, bool isMaxSize) {
            popup.Tag = isMaxSize;
            var rect = Window.Current.CoreWindow.Bounds;
            var w = rect.Width - indent - indent - borderThick - borderThick;
            view.Width = w > 0d ? w : indent * 3d;
            var h = rect.Height - indent - indent - borderThick - borderThick;
            view.Height = h > 0d ? h : indent * 3d;
            ShowReal(view);
        }

        public void Hide() {
            popup.IsOpen = false;
        }

        private void ShowReal(FrameworkElement view) {
            container.Content = view; // 1
            popup.IsOpen = true;    // 2
            container.UpdateLayout(); // 3 view.ActualWidth
        }

        private void Content_SizeChanged(object sender, SizeChangedEventArgs e) {
            var view = popup.Child as FrameworkElement;
            if (view == null) {
                return;
            }

            var rect = Window.Current.CoreWindow.Bounds;
            view.MaxHeight = rect.Height - indent;
            view.MaxWidth = rect.Width - indent;

            if (popup.Tag is Point) {
                var element = Window.Current.Content as FrameworkElement;

                var point = (Point)popup.Tag;

                if (host != null) {
                    var generalTransform = host.TransformToVisual(element);
                    point = generalTransform.TransformPoint(point);
                }

                var cx = element.ActualWidth / 2d;
                var cy = element.ActualHeight / 2d;

                double left, top;

                if (point.X <= cx) {
                    left = point.X;
                }
                else {
                    left = point.X - view.ActualWidth;
                }

                if (point.Y <= cy) {
                    top = point.Y;
                }
                else {
                    top = point.Y - view.ActualHeight;
                }

                popup.HorizontalOffset = left;
                popup.VerticalOffset = top;

                return; // <-------------
            }

            // var horizontalOffset = rect.Width - view.ActualWidth - indent; //on right
            // var horizontalOffset = indent; // on left
            var horizontalOffset = (rect.Width - view.ActualWidth) / 2d;
            if (horizontalOffset < indent) {
                horizontalOffset = indent;
            }

            var verticalOffset = (rect.Height - view.ActualHeight) / 2d;
            if (verticalOffset < indent) {
                verticalOffset = indent;
            }

            popup.HorizontalOffset = horizontalOffset;
            popup.VerticalOffset = verticalOffset;
        }
    }
}
