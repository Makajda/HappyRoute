// <copyright file="TabView.xaml.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace HappyRoute4.Common {
    public sealed partial class TabView : UserControl {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(object),
            typeof(TabView),
            new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty TabsLocationProperty = DependencyProperty.Register(
                "TabsLocation",
                typeof(VerticalAlignment),
                typeof(TabView),
                new PropertyMetadata(VerticalAlignment.Bottom, OnTabsLocationChanged));

        private FrameworkElement selected;
        private int contentRow = 0;
        private int headerRow = 1;

        public TabView() {
            this.InitializeComponent();
        }

        public object ItemsSource {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public VerticalAlignment TabsLocation {
            get { return (VerticalAlignment)GetValue(TabsLocationProperty); }
            set { SetValue(TabsLocationProperty, value); }
        }

        private static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            var item = o as TabView;
            if (o != null) {
                item.ItemsSourceChanged();
            }
        }

        private static void OnTabsLocationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            var item = o as TabView;
            if (o != null) {
                item.TabsLocationChanged();
            }
        }

        private void ItemsSourceChanged() {
            root.Children.Clear();

            if (ItemsSource == null) {
                return;
            }

            var source = ItemsSource as IEnumerable<Tuple<FrameworkElement, object>>;
            if (source == null) {
                return;
            }

            var count = source.Count();
            if (count == 0) {
                return;
            }

            root.RowDefinitions[contentRow].Height = new GridLength(1d, GridUnitType.Star);
            root.RowDefinitions[headerRow].Height = GridLength.Auto;

            foreach (var content in source) {
                if (content.Item1 != null) {
                    content.Item1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    root.Children.Add(content.Item1);
                    Grid.SetRow(content.Item1, contentRow);
                }
            }

            if (count == 1) {
                root.Children[contentRow].Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else {
                root.Children.Add(tabs);
                tabs.Children.Clear();
                tabs.ColumnDefinitions.Clear();

                int i = 0;
                foreach (var content in source) {
                    tabs.ColumnDefinitions.Add(new ColumnDefinition());
                    var button = new Button() {
                        Content = content.Item2,
                        Tag = content,
                        BorderThickness = new Thickness(0d),
                        Margin = new Thickness(5d),
                        VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch,
                        HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch
                    };
                    tabs.Children.Add(button);
                    Grid.SetRow(button, 1);
                    Grid.SetColumn(button, i);
                    button.Tapped += Button_Tapped;
                    if (i == 0) {
                        ChangeSelected(button);
                    }

                    i++;
                }
            }
        }

        private void Button_Tapped(object sender, RoutedEventArgs e) {
            ChangeSelected(sender as FrameworkElement);
        }

        private void ChangeSelected(FrameworkElement newSelected) {
            if (selected != null) {
                var content = selected.Tag as Tuple<FrameworkElement, object>;
                if (content != null && content.Item1 != null) {
                    content.Item1.Visibility = Visibility.Collapsed;
                }

                if (selected is Button) {
                    (selected as Button).BorderThickness = new Thickness(0d);
                }

                Grid.SetRowSpan(selected, 1);
                Grid.SetRow(selected, 1);
            }

            selected = newSelected;

            if (newSelected != null) {
                var content = newSelected.Tag as Tuple<FrameworkElement, object>;
                if (content != null && content.Item1 != null) {
                    content.Item1.Visibility = Visibility.Visible;
                }

                if (newSelected is Button) {
                    (newSelected as Button).BorderThickness = new Thickness(0.3d);
                }

                Grid.SetRow(newSelected, 0);
                Grid.SetRowSpan(newSelected, 2);

                var icollectionView = ItemsSource as ICollectionView;
                if (icollectionView != null) {
                    icollectionView.MoveCurrentTo(content);
                }
            }
        }

        private void TabsLocationChanged() {
            if (TabsLocation == Windows.UI.Xaml.VerticalAlignment.Top) {
                contentRow = 1;
                headerRow = 0;
            }
            else {
                contentRow = 0;
                headerRow = 1;
            }

            root.RowDefinitions[headerRow].Height = GridLength.Auto;
            root.RowDefinitions[contentRow].Height = new GridLength(1d, GridUnitType.Star);

            Grid.SetRow(tabs, headerRow);

            if (ItemsSource == null) {
                return;
            }

            var source = ItemsSource as IEnumerable<Tuple<FrameworkElement, object>>;
            if (source == null) {
                return;
            }

            var count = source.Count();
            if (count == 0) {
                return;
            }

            foreach (var content in source) {
                Grid.SetRow(content.Item1, contentRow);
            }
        }
    }
}
