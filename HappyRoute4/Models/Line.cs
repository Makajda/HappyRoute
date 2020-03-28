// <copyright file="Line.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Prism.Mvvm;
using Windows.UI;

namespace HappyRoute4.Models {
    public class Line : BindableBase {
        private string name1;
        private string name2;
        private Color color;

        public string Name1 {
            get { return name1; }
            set { SetProperty(ref name1, value); }
        }

        public string Name2 {
            get { return name2; }
            set { SetProperty(ref name2, value); }
        }

        public Color Color {
            get { return color; }
            set { SetProperty(ref color, value); }
        }

        public double Indent { get; set; }
        public double Thick { get; set; }
    }
}
