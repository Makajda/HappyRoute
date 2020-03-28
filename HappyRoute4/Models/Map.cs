// <copyright file="Map.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Prism.Mvvm;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace HappyRoute4.Models {
    public class Map : BindableBase {
        private string name1;
        private string name2;

        public Map() {
            Param = new Param();
            Stations = new List<Station>();
            Segments = new List<Segment>();
            Lines = new List<Line>();
        }

        public string Name1 {
            get { return name1; }
            set { SetProperty(ref name1, value); }
        }

        public string Name2 {
            get { return name2; }
            set { SetProperty(ref name2, value); }
        }

        public Param Param { get; private set; }
        public List<Station> Stations { get; private set; }
        public List<Segment> Segments { get; private set; }
        public List<Line> Lines { get; private set; }

        public Size RecalcSize(double freeArea) {
            double left = double.MaxValue, top = double.MaxValue, right = 0d, bottom = 0d;

            foreach (var station in Stations) {
                var bord = station.Vimo.GetBord();
                left = Math.Min(left, bord.Left);
                top = Math.Min(top, bord.Top);
                right = Math.Max(right, bord.Right);
                bottom = Math.Max(bottom, bord.Bottom);
            }

            if (Math.Abs(left) > double.Epsilon || Math.Abs(top) > double.Epsilon) {
                Shift(left - freeArea, top - freeArea);
                right = Math.Max(right - left, 0d);
                bottom = Math.Max(bottom - top, 0d);
            }

            return new Windows.Foundation.Size(right, bottom);
        }

        private void Shift(double dx, double dy) {
            foreach (var station in Stations) {
                station.Left -= dx;
                station.Top -= dy;
                station.Vimo.Relocation();
            }

            foreach (var segment in Segments) {
                segment.Vimo.Relocation();
            }
        }
    }
}
