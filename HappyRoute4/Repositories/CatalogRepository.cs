// <copyright file="CatalogRepository.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using HappyRoute4.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace HappyRoute4.Repositories {
    public static class CatalogRepository {
        private const string samplesFileName = "Samples.txt";

#if DEBUG
        public static async void SamplesSave() {
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fileSavePicker.FileTypeChoices.Add("txt", new List<string>() { ".txt" });
            fileSavePicker.SuggestedFileName = samplesFileName;
            var newStorageFile = await fileSavePicker.PickSaveFileAsync();
            if (newStorageFile != null) {
                var storageFolder = await Package.Current.InstalledLocation.GetFolderAsync(Statics.DataFolderName);
                var catalog = await GetCatalog(storageFolder, null, false);
                if (catalog != null) {
                    using (var fileStream = await newStorageFile.OpenStreamForWriteAsync())
                    using (var writer = new StreamWriter(fileStream)) {
                        foreach (var scheme in catalog.Schemas) {
                            writer.WriteLine(scheme.Name);
                        }
                    }

                    throw new FileNotFoundException(string.Format("Re-created list of examples {0}", newStorageFile.Path));
                }
            }
        }
#endif

        public static async Task<List<Catalog>> GetData() {
            var list = new List<Catalog>();
            try {
                Catalog catalog;

                catalog = await GetCatalogMy();
                if (catalog != null) {
                    list.Add(catalog);
                }

                catalog = await GetCatalogSamples();
                if (catalog != null) {
                    list.Add(catalog);
                }
            }
            catch (Exception exception) {
                var res = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("ErrorReadFiles");
                await AlertMessageService.ShowAsync(exception.Message, res);
            }

            if (list.Count == 0) {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                var res1 = loader.GetString("CatalogStart");
                var res2 = loader.GetString("CatalogSchemeStart");

                var catalog = new Catalog(res1);
                catalog.Schemas.Add(new Schema(res2) { IsMy = true });
                list.Add(catalog);
            }

            return list;
        }

        private static async Task<Catalog> GetCatalog(StorageFolder storageFolder, string nameCatalog, bool isMy) {
            var files = await storageFolder.GetFilesAsync();

            var names = files.Where(n => Path.GetExtension(n.Name) == Statics.FileExtension).Select(n => Path.GetFileNameWithoutExtension(n.Name));
            if (names.Count() > 0) {
                var catalog = new Catalog(nameCatalog);
                foreach (var name in names.OrderBy(n => n)) {
                    catalog.Schemas.Add(new Schema(name) { IsMy = isMy });
                }

                return catalog;
            }

            return null;
        }

        private static async Task<Catalog> GetCatalogMy() {
            var catalogName = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("CatalogMyName");

            var storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(Statics.DataFolderName, CreationCollisionOption.OpenIfExists);
            return await GetCatalog(storageFolder, catalogName, true);
        }

        private static async Task<Catalog> GetCatalogSamples() {
            var storageFile = await Package.Current.InstalledLocation.GetFileAsync(samplesFileName);
            var lines = await PathIO.ReadLinesAsync(storageFile.Path);
            if (lines.Count == 0) {
                return null;
            }

            var catalogName = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("CatalogSamplesName");
            var catalog = new Catalog(catalogName);
            foreach (var line in lines) {
                catalog.Schemas.Add(new Schema(line));
            }

            return catalog;
        }
    }
}
