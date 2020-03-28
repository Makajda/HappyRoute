// <copyright file="Station.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Vimos;
using Prism.Mvvm;
using System.Collections.Generic;

namespace HappyRoute4.Models {
    public class Station : BindableBase {
        private string name1;
        private string name2;
        private double angle;

        public Station() {
            Segments = new List<Segment>();
        }

        public string Name1 {
            get { return name1; }
            set { SetProperty(ref name1, value); }
        }

        public string Name2 {
            get { return name2; }
            set { SetProperty(ref name2, value); }
        }

        public double Angle {
            get { return angle; }
            set { SetProperty(ref angle, value); }
        }

        public double Radius { get; set; }
        public double FontSize { get; set; }
        public double TextIndent { get; set; }
        public double TransferWeight { get; set; }

        public double Left { get; set; }
        public double Top { get; set; }

        public List<Segment> Segments { get; private set; }

        public StationVimo Vimo { get; set; }
    }
}
