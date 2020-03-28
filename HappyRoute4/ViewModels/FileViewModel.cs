// <copyright file="FileViewModel.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Repositories;
using HappyRoute4.Services;
using HappyRoute4.Views;
using HappyRoute4.Vimos;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;

namespace HappyRoute4.ViewModels {
    public class FileViewModel : BindableBase {
        private ModeFile modeFile;
        private FileMenuView view;
        private Func<Task> go;
        private string title;
        private string titleAction;
        private bool isGetName;
        private bool isAlreadyName;
        private string newName;

        public FileViewModel(MapVimo mapVimo) {
            MapVimo = mapVimo;
            MapFile = mapVimo.MapFile;
            NewCommand = new DelegateCommand(() => ShowMode(ModeFile.New));
            DeleteCommand = new DelegateCommand(() => ShowMode(ModeFile.Delete));
            CopyCommand = new DelegateCommand(() => ShowMode(ModeFile.Copy));
            RenameCommand = new DelegateCommand(() => ShowMode(ModeFile.Rename));
            ImportCommand = new DelegateCommand(() => ShowMode(ModeFile.Import));
            ExportCommand = new DelegateCommand(() => ShowMode(ModeFile.Export));

            CancelCommand = new DelegateCommand(Cancel);
            GoCommand = new DelegateCommand(Go, CanGo);
            view = new FileMenuView() { DataContext = this };
        }

        private enum ModeFile {
            New, Delete, Copy, CopyToMy, Rename, Import, Export
        }

        public MapVimo MapVimo { get; private set; }
        public MapFile MapFile { get; private set; }

        public DelegateCommand NewCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public DelegateCommand CopyCommand { get; private set; }
        public DelegateCommand RenameCommand { get; private set; }
        public DelegateCommand ImportCommand { get; private set; }
        public DelegateCommand ExportCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand GoCommand { get; private set; }

        public string Title {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string TitleAction {
            get { return titleAction; }
            set { SetProperty(ref titleAction, value); }
        }

        public bool IsGetName {
            get { return isGetName; }
            set { SetProperty(ref isGetName, value); }
        }

        public bool IsAlreadyName {
            get { return isAlreadyName; }
            set { SetProperty(ref isAlreadyName, value); }
        }

        public string NewName {
            get { return newName; }
            set { SetProperty(ref newName, value); }
        }

        public void CopyToMy() {
            ShowMode(ModeFile.CopyToMy);
        }

        public void Cancel() {
            MapVimo.PropopupService.HidePopup();
        }

        public void Show() {
            MapVimo.PropopupService.ShowPopup(view);
        }

        private void ShowMode(ModeFile newModeFile) {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            modeFile = newModeFile;
            switch (modeFile) {
                case ModeFile.New:
                    Title = loader.GetString("FileVimoNewTitle");
                    TitleAction = loader.GetString("FileVimoNewTitleAction");
                    IsGetName = true;
                    NewName = MapFile.FileName;
                    go = new Func<Task>(NewMap);
                    break;
                case ModeFile.Delete:
                    Title = loader.GetString("FileVimoDeleteTitle");
                    TitleAction = loader.GetString("FileVimoDeleteTitleAction");
                    IsGetName = false;
                    go = new Func<Task>(DeleteMap);
                    break;
                case ModeFile.CopyToMy:
                    Title = loader.GetString("FileVimoCopyToMyTitle");
                    TitleAction = loader.GetString("FileVimoCopyToMyTitleAction");
                    IsGetName = true;
                    NewName = MapFile.FileName;
                    go = new Func<Task>(CopyMapToMy);
                    break;
                case ModeFile.Copy:
                    Title = loader.GetString("FileVimoCopyTitle");
                    TitleAction = loader.GetString("FileVimoCopyTitleAction");
                    IsGetName = true;
                    NewName = MapFile.FileName;
                    go = new Func<Task>(CopyMap);
                    break;
                case ModeFile.Rename:
                    Title = loader.GetString("FileVimoRenameTitle");
                    TitleAction = loader.GetString("FileVimoRenameTitleAction");
                    IsGetName = true;
                    NewName = MapFile.FileName;
                    go = new Func<Task>(RenameMap);
                    break;
                case ModeFile.Import:
                    Title = loader.GetString("FileVimoImportTitle");
                    TitleAction = loader.GetString("FileVimoImportTitleAction");
                    IsGetName = false;
                    go = new Func<Task>(ImportMap);
                    break;
                case ModeFile.Export:
                    Title = loader.GetString("FileVimoExportTitle");
                    TitleAction = loader.GetString("FileVimoexportTitleAction");
                    IsGetName = false;
                    go = new Func<Task>(ExportMap);
                    break;
                default:
                    break;
            }

            IsAlreadyName = false;
            var view = new FileNameView() { DataContext = this };
            MapVimo.PropopupService.ShowPopup(view);
        }

        private bool CanGo() {
            return !IsGetName || !string.IsNullOrWhiteSpace(NewName);
        }

        private async void Go() {
            if (NewName != null) {
                NewName = NewName.Trim();
            }

            await go();
        }

        private async Task NewMap() {
            if (string.IsNullOrWhiteSpace(NewName)) {
                return;
            }

            if (await MapRepository.IsFileExist(NewName)) {
                IsAlreadyName = true;
            }
            else {
                await MapFile.New(NewName);
                MapVimo.PropopupService.HidePopup();
            }
        }

        private async Task CopyMap() => await CopyMapDo(false);

        private async Task CopyMapToMy() => await CopyMapDo(true);

        private async Task CopyMapDo(bool isSetEdit) {
            if (string.IsNullOrWhiteSpace(NewName)) {
                return;
            }

            if (await MapRepository.IsFileExist(NewName)) {
                IsAlreadyName = true;
            }
            else {
                await MapFile.Open(NewName);
                MapVimo.PropopupService.HidePopup();
                if (isSetEdit) {
                    MapVimo.IsEdit = true;
                }
            }
        }

        private async Task RenameMap() {
            if (string.IsNullOrWhiteSpace(NewName)) {
                return;
            }

            if (MapFile.IsMy) {
                if (await MapRepository.IsFileExist(NewName)) {
                    IsAlreadyName = true;
                }
                else {
                    try {
                        await MapRepository.Rename(MapFile.FileName, NewName);
                        MapFile.Rename(NewName);
                        MapVimo.PropopupService.HidePopup();
                    }
                    catch (Exception exception) {
                        var res = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("ErrorFileVimoRename");
                        await AlertMessageService.ShowAsync(exception.Message, $"{res} {MapFile.FileName} {NewName}");
                    }
                }
            }
        }

        private async Task DeleteMap() {
            if (MapFile.IsMy) {
                try {
                    await MapRepository.Delete(MapFile.FileName);
                    await MapFile.Delete();
                }
                catch (Exception exception) {
                    var res = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("ErrorFileVimoDelete");
                    await AlertMessageService.ShowAsync(exception.Message, $"{res} {MapFile.FileName}");
                }
            }

            MapVimo.PropopupService.HidePopup();
        }

        private async Task ImportMap() {
            try {
                var tuple = await MapRepository.Import();
                if (tuple != null) {
                    MapFile.Import(tuple.Item2, tuple.Item1);
                }
            }
            catch (Exception exception) {
                var res = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("ErrorFileVimoImport");
                await AlertMessageService.ShowAsync(exception.Message, res);
            }

            MapVimo.PropopupService.HidePopup();
        }

        private async Task ExportMap() {
            try {
                await MapRepository.Export(MapVimo.Map, MapFile.FileName);
            }
            catch (Exception exception) {
                var res = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("ErrorFileVimoExport");
                await AlertMessageService.ShowAsync(exception.Message, $"{res} {MapFile.FileName}");
            }

            MapVimo.PropopupService.HidePopup();
        }
    }
}
