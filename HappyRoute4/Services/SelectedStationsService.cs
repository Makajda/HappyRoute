// <copyright file="SelectedStationsService.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace HappyRoute4.Services {
    public class SelectedStationsService {
        private Station station1;
        private Station station2;
        private Storyboard storyboard1;
        private Storyboard storyboard2;

        public SelectedStationsService() {
            storyboard1 = GetStoryboard();
            storyboard2 = GetStoryboard();
        }

        public event Action<Station> Selected;
        public event Action<Station> UnSelected;
        public bool IsTwoSelected {
            get { return station1 != null && station2 != null; }
        }

        public Station Station1 {
            get
                { return station1; }
        }

        public Station Station2 {
            get { return station2; }
        }

        public void Change(Station station, DependencyObject target) {
            if (station1 == null) {
                station1 = station;
                Storyboard.SetTarget(storyboard1.Children[0], target);
                Storyboard.SetTarget(storyboard1.Children[1], target);
                storyboard1.Begin();
                Selected?.Invoke(station);
                return;
            }

            if (station1 == station) {
                storyboard1.Stop();
                var storyboardA = storyboard1;
                storyboard1 = storyboard2;
                storyboard2 = storyboardA;
                station1 = station2;
                station2 = null;
                UnSelected?.Invoke(station);
                return;
            }

            if (station2 == null) {
                station2 = station;
                Storyboard.SetTarget(storyboard2.Children[0], target);
                Storyboard.SetTarget(storyboard2.Children[1], target);
                storyboard2.Begin();
                Selected?.Invoke(station);
                return;
            }

            if (station2 == station) {
                storyboard2.Stop();
                station2 = null;
                UnSelected?.Invoke(station);
                return;
            }

            storyboard1.Stop();
            var storyboardB = storyboard1;
            storyboard1 = storyboard2;
            storyboard2 = storyboardB;
            station1 = station2;
            station2 = station;
            Storyboard.SetTarget(storyboard2.Children[0], target);
            Storyboard.SetTarget(storyboard2.Children[1], target);
            storyboard2.Begin();
            Selected?.Invoke(station);
        }

        public void UnselectAll() {
            storyboard2.Stop();
            storyboard1.Stop();
            if (station1 != null) {
                station1.Vimo.LinePath = null;
            }

            if (station2 != null) {
                station2.Vimo.LinePath = null;
            }

            station2 = null;
            station1 = null;
        }

        private Storyboard GetStoryboard() {
            var doubleAnimationX = new DoubleAnimation();
            doubleAnimationX.From = 1d;
            doubleAnimationX.To = 2d;
            doubleAnimationX.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnimationX.AutoReverse = true;
            doubleAnimationX.RepeatBehavior = RepeatBehavior.Forever;

            var doubleAnimationY = new DoubleAnimation();
            doubleAnimationY.From = 1d;
            doubleAnimationY.To = 2d;
            doubleAnimationY.Duration = new Duration(TimeSpan.FromSeconds(1));
            doubleAnimationY.AutoReverse = true;
            doubleAnimationY.RepeatBehavior = RepeatBehavior.Forever;

            var storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimationX);
            storyboard.Children.Add(doubleAnimationY);
            Storyboard.SetTargetProperty(doubleAnimationX, "(RenderTransform).Children[0].ScaleX");
            Storyboard.SetTargetProperty(doubleAnimationY, "(RenderTransform).Children[0].ScaleY");

            // maybe rotate var doubleAnimation = new DoubleAnimation();
            // doubleAnimation.From = 0d;
            // doubleAnimation.To = 360d;
            // doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            // doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            // storyboard.Children.Add(doubleAnimation);
            // Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("RenderTransform.Children[1].Angle"));
            return storyboard;
        }
    }
}
