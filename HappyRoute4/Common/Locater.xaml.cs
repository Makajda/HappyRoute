// <copyright file="Locater.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace HappyRoute4.Common {
    public sealed partial class Locater : UserControl {
        public static readonly DependencyProperty X1Property = DependencyProperty.Register(
            "X1",
            typeof(double),
            typeof(Locater),
            new PropertyMetadata(0.1d, OnXyChanged));

        public static readonly DependencyProperty Y1Property = DependencyProperty.Register(
            "Y1",
            typeof(double),
            typeof(Locater),
            new PropertyMetadata(0.5d, OnXyChanged));

        public static readonly DependencyProperty X2Property = DependencyProperty.Register(
            "X2",
            typeof(double),
            typeof(Locater),
            new PropertyMetadata(0.9d, OnXyChanged));

        public static readonly DependencyProperty Y2Property = DependencyProperty.Register(
            "Y2",
            typeof(double),
            typeof(Locater),
            new PropertyMetadata(0.5d, OnXyChanged));

        public static readonly DependencyProperty IsEllipseProperty = DependencyProperty.Register(
            "IsEllipse",
            typeof(bool),
            typeof(Locater),
            new PropertyMetadata(false, OnIsEllipseChanged));

        private const double Indent = 5d;
        private bool isStartMoving;
        private bool isFinishMoving;

        public Locater() {
            this.InitializeComponent();
            SizeChanged += Locater_SizeChanged;
        }

        public double X1 {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        public double Y1 {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        public double X2 {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        public double Y2 {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        public bool IsEllipse {
            get { return (bool)GetValue(IsEllipseProperty); }
            set { SetValue(IsEllipseProperty, value); }
        }

        private static void OnXyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            var item = o as Locater;
            if (o == null) {
                return;
            }

            if ((double)e.NewValue < 0d) {
                item.SetValue(e.Property, 0d);
            }
            else
                if ((double)e.NewValue > 1d) {
                item.SetValue(e.Property, 1d);
            }

            item.Relocation();
        }

        private static void OnIsEllipseChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            var item = o as Locater;
            if (o != null) {
                item.Relocation();
            }
        }

        private void Locater_SizeChanged(object sender, SizeChangedEventArgs e) {
            Relocation();
        }

        private void Start_PointerMoved(object sender, PointerRoutedEventArgs e) {
            if (!e.Pointer.IsInContact) {
                return;
            }

            if (isFinishMoving == false) {
                e.Handled = true;
                isStartMoving = true;
            }
        }

        private void Finish_PointerMoved(object sender, PointerRoutedEventArgs e) {
            if (e.Pointer.IsInContact) {
                if (isStartMoving == false) {
                    e.Handled = true;
                    isFinishMoving = true;
                }
            }
        }

        private void Panel_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            if (ActualWidth > 0d && ActualHeight > 0d) {
                e.Handled = true;
                var delta = e.Delta.Translation;
                if (isStartMoving == true) {
                    X1 += delta.X / ActualWidth;
                    Y1 += delta.Y / ActualHeight;
                }

                if (isFinishMoving == true) {
                    X2 += delta.X / ActualWidth;
                    Y2 += delta.Y / ActualHeight;
                }
            }
        }

        private void Panel_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            e.Handled = true;
            isStartMoving = isFinishMoving = false;
        }

        private void Relocation() {
            var x1 = X1 * ActualWidth;
            var y1 = Y1 * ActualHeight;
            var x2 = X2 * ActualWidth;
            var y2 = Y2 * ActualHeight;

            Canvas.SetLeft(start, x1 - (start.ActualWidth / 2d));
            Canvas.SetTop(start, y1 - (start.ActualHeight / 2d));
            Canvas.SetLeft(finish, x2 - (finish.ActualWidth / 2d));
            Canvas.SetTop(finish, y2 - (finish.ActualHeight / 2d));

            if (IsEllipse) {
                ellipse.Visibility = Visibility.Visible;

                var max = Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
                ellipse.Width = max;
                ellipse.Height = max;

                Canvas.SetLeft(ellipse, Math.Min(x1, x2) + (Math.Abs(x2 - x1) / 2d) - (max / 2d));
                Canvas.SetTop(ellipse, Math.Min(y1, y2) + (Math.Abs(y2 - y1) / 2d) - (max / 2d));
            }
            else {
                ellipse.Visibility = Visibility.Collapsed;
            }

            var diagonal = Math.Sqrt(Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2));
            if (Math.Abs(diagonal) < double.Epsilon) {
                return;
            }

            var sinx = (X2 - X1) / diagonal;
            var siny = (Y2 - Y1) / diagonal;

            line.X1 = x1 + (((start.ActualWidth / 2d) + Indent) * sinx);
            line.Y1 = y1 + (((start.ActualHeight / 2d) + Indent) * siny);
            line.X2 = x2 - (((finish.ActualWidth / 2d) + Indent) * sinx);
            line.Y2 = y2 - (((finish.ActualHeight / 2d) + Indent) * siny);
        }
    }
}
