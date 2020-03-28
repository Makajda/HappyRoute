// <copyright file="MainPageViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Vimos;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System.Collections.Generic;

namespace HappyRoute4.ViewModels {
    public class MainPageViewModel : ViewModelBase {
        public MainPageViewModel(MapVimo mapVimo) {
            MapVimo = mapVimo;
            var mapProp = mapVimo.MapProp;

            DonateCommand = new DelegateCommand(mapProp.Donate);

            ShowCatalogCommand = new DelegateCommand(mapProp.ShowCatalog);
            FileCommand = new DelegateCommand(mapProp.FileMenu);
            SettingsCommand = new DelegateCommand(mapProp.ShowRouterVariant);
            EditOnCommand = new DelegateCommand(mapProp.EditOn);

            EditMoveCommand = new DelegateCommand(() => SetEditMode(EditModes.Move), () => mapVimo.EditMode != EditModes.Move);
            EditAlignCommand = new DelegateCommand(() => SetEditMode(EditModes.Align), () => mapVimo.EditMode != EditModes.Align);
            EditCreateCommand = new DelegateCommand(() => SetEditMode(EditModes.Create), () => mapVimo.EditMode != EditModes.Create);
            ShowLinesCommand = new DelegateCommand(mapProp.ShowLines);
            ParamCommand = new DelegateCommand(mapProp.ShowParamProp);

            CharactersCommand = new DelegateCommand(() => MapVimo.IsName2 = !MapVimo.IsName2);
            ShowRouteCommand = new DelegateCommand(MapVimo.MapRoute.ShowRoute);
            ClearSelectionCommand = new DelegateCommand(MapVimo.ClearSelection);

            EditOffCommand = new DelegateCommand(() => mapVimo.IsEdit = false);
            DiscardChangesCommand = new DelegateCommand(MapVimo.MapFile.DiscardChanges);
        }

        public MapVimo MapVimo { get; private set; }

        public DelegateCommand DonateCommand { get; private set; }
        public DelegateCommand ShowCatalogCommand { get; private set; }
        public DelegateCommand FileCommand { get; private set; }
        public DelegateCommand SettingsCommand { get; private set; }
        public DelegateCommand EditOnCommand { get; private set; }

        public DelegateCommand EditMoveCommand { get; private set; }
        public DelegateCommand EditAlignCommand { get; private set; }
        public DelegateCommand EditCreateCommand { get; private set; }
        public DelegateCommand ShowLinesCommand { get; private set; }
        public DelegateCommand ParamCommand { get; private set; }

        public DelegateCommand CharactersCommand { get; private set; }
        public DelegateCommand ShowRouteCommand { get; private set; }
        public DelegateCommand ClearSelectionCommand { get; private set; }

        public DelegateCommand EditOffCommand { get; private set; }
        public DelegateCommand DiscardChangesCommand { get; private set; }

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState) {
            base.OnNavigatedTo(e, viewModelState);
            await MapVimo.MapFile.Open();
        }

        public override async void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending) {
            base.OnNavigatingFrom(e, viewModelState, suspending);
            await MapVimo.MapFile.Save();
        }

        private void SetEditMode(EditModes editMode) {
            MapVimo.EditMode = editMode;
            EditMoveCommand.RaiseCanExecuteChanged();
            EditAlignCommand.RaiseCanExecuteChanged();
            EditCreateCommand.RaiseCanExecuteChanged();
        }
    }
}
