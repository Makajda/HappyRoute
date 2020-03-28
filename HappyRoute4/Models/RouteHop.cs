// <copyright file="RouteHop.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace HappyRoute4.Models {
    public class RouteHop {
        public RouteHop(double weight, double runs, Line line, Station station) {
            Weight = weight;
            Runs = runs;
            Line = line;
            Station = station;
        }

        public double Weight { get; private set; }
        public double Runs { get; private set; }
        public Line Line { get; private set; }
        public Station Station { get; private set; }
    }
}
