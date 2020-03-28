// <copyright file="StationView.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace HappyRoute4.Vimos {
    public class StationView {
        private StationVimo stationVimo;
        private Path shape;
        private TextBlock name;

        public StationView(StationVimo stationVimo) {
            this.stationVimo = stationVimo;

            name = new TextBlock();
            stationVimo.MapVimo.MapView.Host.Children.Add(name);
            name.RenderTransform = new RotateTransform();

            name.PointerEntered += Name_shape_PointerEntered;
            name.PointerExited += Name_shape_PointerExited;
            name.PointerPressed += Name_shape_PointerPressed;
            name.PointerReleased += Name_shape_PointerReleased;
            name.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            name.ManipulationStarted += Name_shape_ManipulationStarted;
            name.ManipulationDelta += Name_shape_ManipulationDelta;
            name.Tapped += Name_shape_Tapped;
            name.RightTapped += Name_shape_RightTapped;
            shape = new Path() { Stretch = Stretch.Uniform };
            stationVimo.MapVimo.MapView.Host.Children.Add(shape);
            var pathGeometry = new PathGeometry() { FillRule = FillRule.Nonzero };
            pathGeometry.Figures.Add(new PathFigure());
            shape.Data = pathGeometry;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform() { ScaleX = 1d, ScaleY = 1d });
            transformGroup.Children.Add(new ScaleTransform() { ScaleX = 1d, ScaleY = 1d });
            shape.RenderTransform = transformGroup;

            shape.PointerEntered += Name_shape_PointerEntered;
            shape.PointerExited += Name_shape_PointerExited;
            shape.PointerPressed += Name_shape_PointerPressed;
            shape.PointerReleased += Name_shape_PointerReleased;
            shape.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            shape.ManipulationStarted += Name_shape_ManipulationStarted;
            shape.ManipulationDelta += Name_shape_ManipulationDelta;
            shape.Tapped += Name_shape_Tapped;
            shape.RightTapped += Name_shape_RightTapped;

            shape.Fill = new SolidColorBrush();
            stationVimo.Station.PropertyChanged += (s, e) => { RelocationText(); };
        }

        public void Select() {
            shape.BringToFront();
            stationVimo.OnSelected(shape);
        }

        public void SetBrushes(Models.Line linePath) {
            var brush = (SolidColorBrush)shape.Fill;
            if (linePath == null) {
                if (stationVimo.IsDimed) {
                    brush.Color = stationVimo.MapVimo.Map.Param.DimedColor;
                }
                else {
                    if (stationVimo.IsStationTransit) {
                        var line = stationVimo.Station.Segments[0].Lines[0];
                        brush.Color = line == null ? stationVimo.MapVimo.Map.Param.BuildColor : line.Color;
                    }
                    else {
                        brush.Color = stationVimo.MapVimo.Map.Param.TransferColor;
                    }
                }

                RelocationText(); // Foreground in NameSplit
            }
            else {
                brush.Color = linePath.Color;
            }
        }

        public void RelocationText() {
            var nameString = (stationVimo.MapVimo.IsName2 && stationVimo.Station.Name2 != null ? stationVimo.Station.Name2 : stationVimo.Station.Name1) ?? string.Empty;

            if (string.IsNullOrEmpty(nameString) || nameString[0] != '{') {
                if (stationVimo.IsStationTransit) {
                    name.Inlines.Clear();
                    name.Text = nameString;
                    name.Foreground = shape.Fill;
                }
                else {
                    var lines = stationVimo.Station.Segments.SelectMany(n => n.Lines).Distinct().ToList();
                    if (lines.Count == 0) {
                        name.Inlines.Clear();
                        name.Text = nameString;
                        name.Foreground = new SolidColorBrush(stationVimo.MapVimo.Map.Param.BuildColor);
                    }
                    else {
                        NameSetForeground(nameString, lines);
                    }
                }
            }
            else {
                NameSplitAndSetForeground(nameString);
            }

            name.UpdateLayout();

            var width = name.ActualWidth;
            var halfHeight = name.ActualHeight / 2d;

            var rotateTransform = name.RenderTransform as RotateTransform;
            double shift;
            if (stationVimo.Station.Angle >= 90 && stationVimo.Station.Angle < 270) {
                rotateTransform.Angle = stationVimo.Station.Angle - 180;
                shift = -width - stationVimo.MapVimo.Map.Param.Radius - (stationVimo.MapVimo.Map.Param.TextIndent + stationVimo.Station.TextIndent);
            }
            else {
                rotateTransform.Angle = stationVimo.Station.Angle;
                shift = stationVimo.MapVimo.Map.Param.Radius + (stationVimo.MapVimo.Map.Param.TextIndent + stationVimo.Station.TextIndent);
            }

            Canvas.SetLeft(name, stationVimo.Station.Left + shift);
            rotateTransform.CenterX = -shift;

            Canvas.SetTop(name, stationVimo.Station.Top - halfHeight);
            rotateTransform.CenterY = halfHeight;
        }

        public void SetOver(bool isOver) {
            shape.Opacity = isOver ? 0.1d : 1d;
        }

        public void Relocation() {
            Canvas.SetLeft(shape, stationVimo.Station.Left - (shape.Width / 2d));
            Canvas.SetTop(shape, stationVimo.Station.Top - (shape.Height / 2d));
            RelocationText();
        }

        public void Resize(double inc) {
            var param = stationVimo.MapVimo.Map.Param;
            var radius = (param.Radius + stationVimo.Station.Radius) * inc;
            if (radius < 0d) {
                radius = 0d;
            }

            shape.Width = 2d * radius;
            shape.Height = 2d * radius;

            var transformGroup = shape.RenderTransform as TransformGroup;
            ScaleTransform scaleTransform;
            scaleTransform = transformGroup.Children[0] as ScaleTransform;
            scaleTransform.CenterX = radius;
            scaleTransform.CenterY = radius;

            scaleTransform = transformGroup.Children[1] as ScaleTransform;
            scaleTransform.CenterX = radius;
            scaleTransform.CenterY = radius;

            var fontSize = (param.FontSize + stationVimo.Station.FontSize) * inc;
            if (fontSize < 1d) {
                fontSize = 1d;
            }

            name.FontSize = fontSize;
            Relocation();
        }

        public void ChangeShape() {
            var pathFigure = ((PathGeometry)shape.Data).Figures[0];

            var count = stationVimo.Station.Segments.SelectMany(n => n.Lines).Count();

            if (count < 3) {
                if (pathFigure.Segments.Count != 2) {
                    pathFigure.Segments.Clear();
                    pathFigure.StartPoint = new Point(-1, 0);
                    pathFigure.Segments.Add(new ArcSegment() { Size = new Size(1, 1), IsLargeArc = true, SweepDirection = SweepDirection.Clockwise, Point = new Point(1, 0) });
                    pathFigure.Segments.Add(new ArcSegment() { Size = new Size(1, 1), IsLargeArc = true, SweepDirection = SweepDirection.Clockwise, Point = new Point(-1, 0) });
                }
            }
            else {
                if (pathFigure.Segments.Count != count) {
                    pathFigure.Segments.Clear();
                    var da = Math.PI * 2d / count;
                    pathFigure.StartPoint = new Point(1, 0);
                    for (var a = da; a < Math.PI * 2d; a += da) {
                        var dx = Math.Cos(a);
                        var dy = Math.Sin(a);
                        pathFigure.Segments.Add(new LineSegment() { Point = new Point(dx, dy) });
                    }

                    pathFigure.Segments.Add(new LineSegment() { Point = new Point(pathFigure.StartPoint.X, pathFigure.StartPoint.Y) }); // Closed
                }
            }
        }

        public void Remove() {
            stationVimo.MapVimo.MapView.Host.Children.Remove(shape);
            stationVimo.MapVimo.MapView.Host.Children.Remove(name);
        }

        public Rect GetBord() {
            var left = stationVimo.Station.Left - (shape.ActualWidth / 2d);
            var top = stationVimo.Station.Top - (shape.ActualHeight / 2d);
            var right = stationVimo.Station.Left + (shape.ActualWidth / 2d);
            var bottom = stationVimo.Station.Top + (shape.ActualHeight / 2d);

            var leftName = Canvas.GetLeft(name);
            var topName = Canvas.GetTop(name);

            var t = name.RenderTransform;
            var b = t.TransformBounds(new Rect(0d, 0d, name.ActualWidth, name.ActualHeight));
            left = Math.Min(left, Math.Floor(leftName + b.Left));
            top = Math.Min(top, Math.Floor(topName + b.Top));
            right = Math.Max(right, Math.Ceiling(leftName + b.Right));
            bottom = Math.Max(bottom, Math.Ceiling(topName + b.Bottom));
            return new Rect(left, top, right - left, bottom - top);
        }

        public void SetVisible() {
            if (stationVimo.IsVisible) {
                shape.Visibility = Visibility.Visible;
                name.Visibility = Visibility.Visible;
            }
            else {
                shape.Visibility = Visibility.Collapsed;
                name.Visibility = Visibility.Collapsed;
            }
        }

        private void Name_shape_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            if (IsNotCaptured((UIElement)sender)) {
                Resize(stationVimo.MapVimo.Map.Param.CoStation);
            }
        }

        private void Name_shape_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            Resize(1d);
        }

        private void Name_shape_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            Resize(1d);
            shape.BringToFront();
            if (stationVimo.OnPressed()) {
                ((UIElement)sender).CapturePointer(e.Pointer);
            }
        }

        private void Name_shape_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            var element = (UIElement)sender;
            if (IsNotCaptured(element)) {
                return;
            }

            element.ReleasePointerCapture(e.Pointer);
            stationVimo.OnReleased(e.GetCurrentPoint(stationVimo.MapVimo.MapView.Host).Position);
        }

        private void Name_shape_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) {
            stationVimo.CreateStation();
        }

        private void Name_shape_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            var element = (UIElement)sender;
            if (IsNotCaptured(element)) {
                return;
            }

            if (stationVimo.MapVimo.IsEdit) {
                e.Handled = true;
                stationVimo.MoveStation(e.Delta.Translation);
            }
        }

        private void Name_shape_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            e.Handled = true;
            Select();
        }

        private void Name_shape_RightTapped(object sender, RightTappedRoutedEventArgs e) {
            if (stationVimo.MapVimo.IsEdit) {
                e.Handled = true;
                shape.BringToFront();
                stationVimo.Prop();
            }
        }

        private bool IsNotCaptured(UIElement element) {
            return element.PointerCaptures == null || element.PointerCaptures.Count == 0;
        }

        private void NameSetForeground(string nameString, List<Models.Line> lines) {
            name.Inlines.Clear();
            int len = nameString.Length / lines.Count;
            int i = 0;
            var lastLine = lines.Last();
            foreach (var line in lines) {
                string n = line == lastLine ? nameString.Substring(i) : nameString.Substring(i, len);
                var r = new Run();
                r.Foreground = stationVimo.IsDimed ? new SolidColorBrush(stationVimo.MapVimo.Map.Param.DimedColor)
                    : (line == null ? new SolidColorBrush(stationVimo.MapVimo.Map.Param.BuildColor) : new SolidColorBrush(line.Color));
                r.Text = n;
                name.Inlines.Add(r);
                i += len;
            }
        }

        private void NameSplitAndSetForeground(string nameString) {
            name.Inlines.Clear();
            var a = nameString.Split('{');
            foreach (var b in a) {
                int i = 0;
                var isNewLine = true;
                if (i < b.Length && b[i] == '-') {
                    isNewLine = false;
                    i++;
                }

                var x = string.Empty;
                while (i < b.Length && char.IsDigit(b[i])) {
                    x += b[i++];
                }

                if (x.Length > 0 && i + 1 < b.Length && b[i] == '}') {
                    var n = b.Substring(i + 1);
                    if (n.Length > 0) {
                        int j;
                        if (int.TryParse(x, out j)) {
                            if (j > 0 && j <= stationVimo.MapVimo.Map.Lines.Count) {
                                var r = new Run();
                                var line = stationVimo.MapVimo.Map.Lines[j - 1];
                                r.Foreground = stationVimo.IsDimed ? new SolidColorBrush(stationVimo.MapVimo.Map.Param.DimedColor)
                                    : (line == null ? new SolidColorBrush(stationVimo.MapVimo.Map.Param.BuildColor) : new SolidColorBrush(line.Color));
                                r.Text = n;
                                if (name.Inlines.Count > 0 && isNewLine) {
                                    name.Inlines.Add(new LineBreak());
                                }

                                name.Inlines.Add(r);
                            }
                        }
                    }
                }
            }
        }
    }
}
