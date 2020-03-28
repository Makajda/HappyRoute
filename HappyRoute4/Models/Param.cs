// <copyright file="Param.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using Prism.Mvvm;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace HappyRoute4.Models {
    public class Param : BindableBase {
        private const double RadiusDefault = 14d;
        private const double ThickDefault = 6d;
        private const double IndentDefault = 6d;
        private const double TextIndentDefault = 6d;
        private const double FontSizeDefault = 11d;
        private const double CoStationDefault = 1.5d;
        private const double CoSegmentDefault = 2d;
        private const double TransferWeightDefault = 3d;

        private string autors;
        private double radius = RadiusDefault;
        private double textIndent = TextIndentDefault;
        private double fontSize = FontSizeDefault;
        private double indent = IndentDefault;
        private double thick = ThickDefault;
        private double coStation = CoStationDefault;
        private double coSegment = CoSegmentDefault;
        private SolidColorBrush background = new SolidColorBrush(Colors.Wheat);
        private Color transferColor = Colors.White;
        private Color dimedColor = Colors.LightGray;
        private Color buildColor = Colors.Black;
        private double transferWeight = TransferWeightDefault;
        private double numberNames = 1d;

        public string Autors {
            get { return autors; }
            set { SetProperty(ref autors, value); }
        }

        public double Radius {
            get { return radius; }
            set { SetProperty(ref radius, value); }
        }

        public double TextIndent {
            get { return textIndent; }
            set { SetProperty(ref textIndent, value); }
        }

        public double FontSize {
            get { return fontSize; }
            set { SetProperty(ref fontSize, value); }
        }

        public double Indent {
            get { return indent; }
            set { SetProperty(ref indent, value); }
        }

        public double Thick {
            get { return thick; }
            set { SetProperty(ref thick, value); }
        }

        public double CoStation {
            get { return coStation; }
            set { SetProperty(ref coStation, value); }
        }

        public double CoSegment {
            get { return coSegment; }
            set { SetProperty(ref coSegment, value); }
        }

        public SolidColorBrush Background {
            get { return background; }
            set { SetProperty(ref background, value); }
        }

        public Color TransferColor {
            get { return transferColor; }
            set { SetProperty(ref transferColor, value); }
        }

        public Color DimedColor {
            get { return dimedColor; }
            set { SetProperty(ref dimedColor, value); }
        }

        public Color BuildColor {
            get { return buildColor; }
            set { SetProperty(ref buildColor, value); }
        }

        public double TransferWeight {
            get { return transferWeight; }
            set { SetProperty(ref transferWeight, value); }
        }

        public double NumberNames {
            get { return numberNames; }
            set { SetProperty(ref numberNames, value); }
        }
    }
}
