// <copyright file="Angler.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace HappyRoute4.Common {
    public sealed partial class Angler : UserControl {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle",
            typeof(double),
            typeof(Angler),
            new PropertyMetadata(0d, OnAngleChanged));

        public static readonly DependencyProperty FillRotorProperty = DependencyProperty.Register(
            "FillRotor",
            typeof(Brush),
            typeof(Angler),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnFillRotorChanged));

        private const int notchesCount = 24;

        private Point oldPosition;
        private double centerX;
        private double centerY;

        private Line[] notchesLine;
        private Ellipse[] notchesArea;

        public Angler() {
            this.InitializeComponent();

            notchesLine = new Line[notchesCount];
            notchesArea = new Ellipse[notchesCount];

            for (int i = 0; i < notchesCount; i++) {
                var line = new Line() { Stroke = ellipseRotor.Stroke, StrokeThickness = 1d };
                var ellipse = new Ellipse() {
                    Stroke = ellipseRotor.Stroke,
                    StrokeThickness = 0,
                    Fill = new SolidColorBrush(Colors.Transparent),
                    Tag = i * 360d / notchesCount
                };
                ellipse.Tapped += Notche_Ellipse_Tapped;
                panel.Children.Add(line);
                panel.Children.Add(ellipse);
                notchesLine[i] = line;
                if (i % 3 == 0) { line.StrokeThickness = 2d; }
                notchesArea[i] = ellipse;

                this.SizeChanged += Angler_SizeChanged;
            }
        }

        public double Angle {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public Brush FillRotor {
            get { return (Brush)GetValue(FillRotorProperty); }
            set { SetValue(FillRotorProperty, value); }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e) {
            base.OnPointerPressed(e);
            e.Handled = true;
            oldPosition = e.GetCurrentPoint(this).Position;
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e) {
            base.OnPointerMoved(e);

            if (!e.Pointer.IsInContact) {
                return;
            }

            e.Handled = true;

            var newPosition = e.GetCurrentPoint(this).Position;

            var delta = Math.Round(Math.Sqrt(Math.Pow(newPosition.X - oldPosition.X, 2) + Math.Pow(newPosition.Y - oldPosition.Y, 2)), 0);
            var angleDelta = ((newPosition.X - centerX) * (oldPosition.Y - centerY) >= (newPosition.Y - centerY) * (oldPosition.X - centerX)) ? -delta : delta;
            var angle = Math.Round(Angle + angleDelta, 0);

            if (angle < 0d) { angle = 360 + angle; }
            else if (angle >= 360d) { angle = angle % 360; }

            Angle = angle;

            oldPosition = newPosition;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e) {
            base.OnPointerReleased(e);
            e.Handled = true;
        }

        private static void OnAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            var item = o as Angler;
            item.Relocation();
        }

        private static void OnFillRotorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            var item = o as Angler;
            item.ellipseRotor.Fill = (Brush)e.NewValue;
        }

        private void Angler_SizeChanged(object sender, SizeChangedEventArgs e) {
            centerX = e.NewSize.Width / 2d;
            centerY = e.NewSize.Height / 2d;

            var minsize = Math.Min(e.NewSize.Width, e.NewSize.Height) / 2d;

            ellipseRotor.Width = minsize;
            ellipseRotor.Height = minsize / 4d;
            Canvas.SetLeft(ellipseRotor, centerX);
            Canvas.SetTop(ellipseRotor, centerY - (ellipseRotor.Height / 2d));

            rotateEllipse.CenterY = ellipseRotor.Height / 2d;

            double notcheRadius = minsize / 8d;
            var notcheSize = notcheRadius * 2d;
            for (int i = 0; i < notchesCount; i++) {
                var ellipse = notchesArea[i];

                ellipse.Width = notcheSize;
                ellipse.Height = notcheSize;

                var a = (double)ellipse.Tag * Math.PI / 180d;

                var x = centerX - notcheRadius + ((minsize - notcheRadius) * Math.Cos(a));
                var y = centerY - notcheRadius + ((minsize - notcheRadius) * Math.Sin(a));

                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                var line = notchesLine[i];

                x = centerX + ((minsize - notcheSize) * Math.Cos(a));
                y = centerY + ((minsize - notcheSize) * Math.Sin(a));

                Canvas.SetLeft(line, x);
                Canvas.SetTop(line, y);

                line.X2 = notcheSize * Math.Cos(a);
                line.Y2 = notcheSize * Math.Sin(a);
            }
        }

        private void Notche_Ellipse_Tapped(object sender, TappedRoutedEventArgs e) {
            var frameworkElement = sender as FrameworkElement;
            e.Handled = true;
            Angle = (double)frameworkElement.Tag;
        }

        private void Relocation() {
            rotateEllipse.Angle = Angle;
        }
    }
}
