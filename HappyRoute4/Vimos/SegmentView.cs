// <copyright file="SegmentView.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace HappyRoute4.Vimos {
    public class SegmentView {
        private SegmentVimo segmentVimo;
        private List<Path> shapes;

        public SegmentView(SegmentVimo segmentVimo) {
            this.segmentVimo = segmentVimo;
            shapes = new List<Path>();
        }

        public void Relocation(bool isLarge) {
            var countLines = segmentVimo.Segment.Lines.Count;
            var difCount = countLines - shapes.Count;
            if (difCount < 0) {
                while (shapes.Count > 0 && shapes.Count != countLines) {
                    var shape = shapes.Last();
                    segmentVimo.MapVimo.MapView.Host.Children.Remove(shape);
                    shapes.Remove(shape);
                }
            }
            else {
                for (int i = 0; i < difCount; i++) {
                    var shape = new Path();
                    shapes.Add(shape);
                    segmentVimo.MapVimo.MapView.Host.Children.Add(shape);

                    var pathGeometry = new PathGeometry() { FillRule = FillRule.Nonzero };
                    var pathFigure = new PathFigure();
                    pathFigure.Segments.Add(new LineSegment());
                    pathFigure.Segments.Add(new ArcSegment() { IsLargeArc = true, SweepDirection = SweepDirection.Clockwise });
                    pathFigure.Segments.Add(new LineSegment());
                    pathFigure.Segments.Add(new ArcSegment() { IsLargeArc = true, SweepDirection = SweepDirection.Clockwise });
                    pathGeometry.Figures.Add(pathFigure);
                    shape.Data = pathGeometry;
                    shape.Fill = new SolidColorBrush();

                    shape.PointerEntered += Shape_PointerEntered;
                    shape.PointerExited += Shape_PointerExited;
                    shape.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
                    shape.ManipulationStarted += Shape_ManipulationStarted;
                    shape.Tapped += Shape_Tapped;
                    shape.RightTapped += Shape_RightTapped;
                }
            }

            var x1 = segmentVimo.Segment.Station1.Left;
            var y1 = segmentVimo.Segment.Station1.Top;
            var x2 = segmentVimo.Segment.Station2.Left;
            var y2 = segmentVimo.Segment.Station2.Top;

            var diagonal = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            if (Math.Abs(diagonal) < double.Epsilon) {
                return;
            }

            var sinx = (x2 - x1) / diagonal;
            var siny = (y2 - y1) / diagonal;

            var indent1Segment = segmentVimo.MapVimo.Map.Param.Radius + segmentVimo.MapVimo.Map.Param.Indent + segmentVimo.Segment.Indent;
            var indent2Segment = indent1Segment + segmentVimo.Segment.Station2.Radius;
            indent1Segment += segmentVimo.Segment.Station1.Radius;
            var thickSegment = segmentVimo.MapVimo.Map.Param.Thick + segmentVimo.Segment.Thick;

            double shift = 0d;
            if (countLines > 1) {
                foreach (var line in segmentVimo.Segment.Lines) {
                    shift = shift - thickSegment - (line == null ? 0d : line.Thick);
                }

                if (isLarge) {
                    shift *= segmentVimo.MapVimo.Map.Param.CoSegment / 2d;
                }
                else {
                    shift /= 2d;
                }
            }

            for (int i = 0; i < countLines; i++) {
                var shape = shapes[i];
                var line = segmentVimo.Segment.Lines[i];

                var thick = thickSegment + (line == null ? 0 : line.Thick);
                if (isLarge) {
                    thick *= segmentVimo.MapVimo.Map.Param.CoSegment;
                }

                if (thick < 0d) {
                    thick = 0d;
                }

                var halfThick = thick / 2d;

                var dxThick = halfThick * siny;
                var dyThick = -halfThick * sinx;

                var indent1 = halfThick + (line == null ? 0 : line.Indent);
                var indent2 = indent1 + indent2Segment;
                indent1 += indent1Segment;
                var dxIndend1 = indent1 * sinx;
                var dyIndent1 = indent1 * siny;
                var dxIndend2 = indent2 * sinx;
                var dyIndent2 = indent2 * siny;

                double dxShift = 0d, dyShift = 0d;
                if (countLines > 1) {
                    shift += halfThick;
                    dxShift = shift * siny;
                    dyShift = -shift * sinx;
                }

                var pathFigure = (shape.Data as PathGeometry).Figures[0];

                pathFigure.StartPoint = new Point(x1 + dxIndend1 + dxThick + dxShift, y1 + dyIndent1 + dyThick + dyShift);

                (pathFigure.Segments[0] as LineSegment).Point = new Point(x2 - dxIndend2 + dxThick + dxShift, y2 - dyIndent2 + dyThick + dyShift);

                var arcSegment = pathFigure.Segments[1] as ArcSegment;
                arcSegment.Point = new Point(x2 - dxIndend2 - dxThick + dxShift, y2 - dyIndent2 - dyThick + dyShift);
                arcSegment.Size = new Size(halfThick, halfThick);

                (pathFigure.Segments[2] as LineSegment).Point = new Point(x1 + dxIndend1 - dxThick + dxShift, y1 + dyIndent1 - dyThick + dyShift);

                arcSegment = pathFigure.Segments[3] as ArcSegment;
                arcSegment.Point = pathFigure.StartPoint;
                arcSegment.Size = new Size(halfThick, halfThick);

                shift += halfThick;
            }
        }

        public void SetBrushes(Models.Line linePath) {
            for (int i = 0; i < shapes.Count; i++) {
                var brush = shapes[i].Fill as SolidColorBrush;
                var line = segmentVimo.Segment.Lines[i];
                if (linePath == null) {
                    if (segmentVimo.IsDimed) {
                        brush.Color = segmentVimo.MapVimo.Map.Param.DimedColor;
                    }
                    else {
                        brush.Color = line == null ? segmentVimo.MapVimo.Map.Param.BuildColor : line.Color;
                    }
                }
                else {
                    brush.Color = line == linePath ? linePath.Color : segmentVimo.MapVimo.Map.Param.DimedColor;
                }
            }
        }

        public void Remove() {
            foreach (var shape in shapes) {
                segmentVimo.MapVimo.MapView.Host.Children.Remove(shape);
            }

            shapes.Clear();
        }

        public void Remove(int iLine) {
            var shape = shapes[iLine];
            segmentVimo.MapVimo.MapView.Host.Children.Remove(shape);
            shapes.Remove(shape);
            Relocation(false);
        }

        public void SetVisible() {
            var visibility = segmentVimo.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            foreach (var shape in shapes) {
                shape.Visibility = visibility;
            }
        }

        private void Shape_PointerEntered(object sender, PointerRoutedEventArgs e) {
            Relocation(true);
        }

        private void Shape_PointerExited(object sender, PointerRoutedEventArgs e) {
            Relocation(false);
        }

        private void Shape_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) {
            Relocation(false);
        }

        private void Shape_Tapped(object sender, TappedRoutedEventArgs e) {
            if (segmentVimo.MapVimo.IsEdit && segmentVimo.MapVimo.EditMode == EditModes.Create) {
                e.Handled = true;
                Windows.UI.Xaml.Controls.Canvas.SetZIndex(sender as Shape, 0);
                segmentVimo.AddStation(e.GetPosition(segmentVimo.MapVimo.MapView.Host));
            }
        }

        private void Shape_RightTapped(object sender, RightTappedRoutedEventArgs e) {
            if (segmentVimo.MapVimo.IsEdit) {
                e.Handled = true;
                segmentVimo.Prop(e.GetPosition(segmentVimo.MapVimo.MapView.Host));
            }
        }
    }
}
