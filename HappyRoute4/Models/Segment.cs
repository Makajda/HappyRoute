// <copyright file="Segment.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Vimos;
using System.Collections.Generic;

namespace HappyRoute4.Models {
    public class Segment {
        public Segment() {
            Weight = 1d;
            Lines = new List<Line>();
        }

        public Station Station1 { get; set; }
        public Station Station2 { get; set; }
        public List<Line> Lines { get; private set; }

        public SegmentVimo Vimo { get; set; }

        public double Indent { get; set; }
        public double Thick { get; set; }
        public double Weight { get; set; }
    }
}
