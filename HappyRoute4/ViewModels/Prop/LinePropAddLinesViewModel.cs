// <copyright file="LinePropAddLinesViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Models;
using HappyRoute4.Views.Prop;
using HappyRoute4.Vimos;
using Prism.Commands;
using Prism.Mvvm;
using System;
using Windows.UI;

namespace HappyRoute4.ViewModels.Prop {
    public class LinePropAddLinesViewModel : BindableBase {
        private LinePropAddLinesView view;
        private MapVimo mapVimo;
        private string lines;

        public LinePropAddLinesViewModel(MapVimo mapVimo) {
            this.mapVimo = mapVimo;
            AddCommand = new DelegateCommand(Add);
            ClearCommand = new DelegateCommand(Clear);
            view = new LinePropAddLinesView() { DataContext = this };
        }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }

        public string Lines {
            get { return lines; }
            set { SetProperty(ref lines, value); }
        }

        public void Show() {
            mapVimo.PropopupService.ShowPopup(view);
        }

        private void Clear() {
            Lines = null;
        }

        private void Add() {
            if (string.IsNullOrWhiteSpace(Lines)) {
                return;
            }

            var names = Lines.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var random = new Random();
            foreach (var s in names) {
                var a = s.Split('/');
                var name1 = a.Length > 0 ? a[0].Trim() : null;
                var name2 = a.Length > 1 ? a[1].Trim() : null;

                var bytes = new byte[3];
                random.NextBytes(bytes);
                var color = Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);
                var line = new Line() { Name1 = name1, Name2 = name2, Color = color };
                mapVimo.Map.Lines.Add(line);
            }

            Lines = null;
            mapVimo.PropopupService.HidePopup();
        }
    }
}
