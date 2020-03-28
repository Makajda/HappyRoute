// <copyright file="Paramer.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HappyRoute4.Common {
    public sealed partial class Paramer : UserControl {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(Paramer),
            new PropertyMetadata(null, OnHeaderChanged));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(Paramer),
            new PropertyMetadata(0d, OnValueChanged));

        public static readonly DependencyProperty IsZeroVisibleProperty = DependencyProperty.Register(
            "IsZeroVisible",
            typeof(bool),
            typeof(Paramer),
            new PropertyMetadata(false, OnIsZeroVisibleChanged));

        public static readonly DependencyProperty DeltaProperty = DependencyProperty.Register(
            "Delta",
            typeof(double),
            typeof(Paramer),
            new PropertyMetadata(1d));

        public Paramer() {
            this.InitializeComponent();
        }

        public string Header {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public double Value {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public bool IsZeroVisible {
            get { return (bool)GetValue(IsZeroVisibleProperty); }
            set { SetValue(IsZeroVisibleProperty, value); }
        }

        public double Delta {
            get { return (double)GetValue(DeltaProperty); }
            set { SetValue(DeltaProperty, value); }
        }

        private static void OnHeaderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            var item = o as Paramer;
            if (o != null) {
                item.SetHeader();
            }
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            var item = o as Paramer;
            if (o != null) {
                item.SetHeader();
            }
        }

        private static void OnIsZeroVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            var item = o as Paramer;
            if (o != null) {
                item.IsZeroVisibleChanged();
            }
        }

        private void SetHeader() {
            header.Text = $"{Header}: {(Math.Abs(Value) < double.Epsilon ? null : (Value.ToString() + ":"))}";
        }

        private void IsZeroVisibleChanged() {
            zero.Visibility = IsZeroVisible ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void Zero_Tapped(object sender, RoutedEventArgs e) {
            Value = 0d;
        }

        private void RepeatButton_Click_Less(object sender, RoutedEventArgs e) {
            Value = Math.Round(Value - Delta, 4);
        }

        private void RepeatButton_Click_More(object sender, RoutedEventArgs e) {
            Value = Math.Round(Value + Delta, 4);
        }
    }
}
