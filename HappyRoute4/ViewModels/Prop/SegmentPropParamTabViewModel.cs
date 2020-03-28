// <copyright file="SegmentPropParamTabViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using HappyRoute4.Views.Prop;
using HappyRoute4.Vimos;
using Prism.Mvvm;

namespace HappyRoute4.ViewModels.Prop {
    public class SegmentPropParamTabViewModel : BindableBase {
        private MapVimo mapVimo;
        private Segment segment;
        private bool isOnChanged = false;
        private double indent;
        private double thick;
        private double weight;

        public SegmentPropParamTabViewModel(MapVimo mapVimo, Segment segment) {
            this.mapVimo = mapVimo;
            this.segment = segment;

            isOnChanged = false;
            Indent = segment.Indent;
            Thick = segment.Thick;
            Weight = segment.Weight;
            isOnChanged = true;

            View = new SegmentPropParamTabView() { DataContext = this };
        }

        public SegmentPropParamTabView View { get; private set; }

        public double Indent {
            get { return indent; }
            set { SetProperty(ref indent, value, OnParamChanged); }
        }

        public double Thick {
            get { return thick; }
            set { SetProperty(ref thick, value, OnParamChanged); }
        }

        public double Weight {
            get { return weight; }
            set { SetProperty(ref weight, value, OnWeightChanged); }
        }

        private void OnParamChanged() {
            if (isOnChanged) {
                segment.Indent = Indent;
                segment.Thick = Thick;
                segment.Vimo.Relocation();
            }
        }

        private void OnWeightChanged() {
            if (isOnChanged) {
                segment.Weight = Weight;
                mapVimo.MapRoute.Reset();
            }
        }
    }
}
