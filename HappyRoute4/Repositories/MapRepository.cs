// <copyright file="MapRepository.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using HappyRoute4.Common;
using HappyRoute4.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace HappyRoute4.Repositories {
    /// <summary>
    /// Get and save Map
    /// </summary>
    public static class MapRepository {
        public static async Task Save(Map map, string filename) {
            var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(GetFileName(filename), CreationCollisionOption.ReplaceExisting);
            await SaveFile(map, storageFile);
        }

        public static async Task<Map> Open(string filename, bool isMy) {
            var storageFolder = isMy ? ApplicationData.Current.LocalFolder : Package.Current.InstalledLocation;
            var f = GetFileName(filename);
            var storageFile = await storageFolder.GetFileAsync(f);
            var map = await OpenFile(storageFile);
            return map;
        }

        public static async Task Rename(string filename, string desiredname) {
            var storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(GetFileName(filename));
            await storageFile.RenameAsync(Path.ChangeExtension(desiredname, Statics.FileExtension), NameCollisionOption.FailIfExists);
        }

        public static async Task Delete(string filename) {
            var storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(GetFileName(filename));
            await storageFile.DeleteAsync();
        }

        public static async Task<Tuple<Map, string>> Import() {
            var fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fileOpenPicker.FileTypeFilter.Add(".hro");
            var storageFile = await fileOpenPicker.PickSingleFileAsync();
            if (storageFile == null) {
                return null;
            }

            var map = await OpenFile(storageFile);
            var name = Path.GetFileNameWithoutExtension(storageFile.Name);
            while (await IsFileExist(name)) {
                name += " imported";
            }

            await Save(map, name);
            return new Tuple<Map, string>(map, name);
        }

        public static async Task Export(Map map, string filename) {
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fileSavePicker.FileTypeChoices.Add("hro", new List<string>() { ".hro" });
            fileSavePicker.SuggestedFileName = filename;
            var storageFile = await fileSavePicker.PickSaveFileAsync();
            if (storageFile != null) {
                await SaveFile(map, storageFile);
            }
        }

        public static async Task<bool> IsFileExist(string name) {
            try {
                if (name == "App") {
                    return true;
                }

                await ApplicationData.Current.LocalFolder.GetFileAsync(GetFileName(name));
                return true;
            }

            // FileNotFoundException
            catch (Exception) {
                return false;
            }
        }

        private static XDocument MapTox(Map map) {
            var xdocument = new XDocument();
            var xe = new XElement("Map");
            xdocument.Add(xe);
            if (!string.IsNullOrEmpty(map.Name1)) { xe.Add(new XAttribute("Name1", map.Name1)); }
            if (!string.IsNullOrEmpty(map.Name2)) { xe.Add(new XAttribute("Name2", map.Name2)); }

            var xParam = new XElement("Param");
            xe.Add(xParam);
            ParamTox(xParam, map.Param);

            var xLines = new XElement("Lines");
            xe.Add(xLines);
            foreach (var line in map.Lines) {
                LineTox(xLines, line);
            }

            var xStation = new XElement("Stations");
            xe.Add(xStation);
            foreach (var station in map.Stations) {
                StationTox(xStation, station);
            }

            var xSegment = new XElement("Segments");
            xe.Add(xSegment);
            foreach (var segment in map.Segments) {
                SegmentTox(xSegment, segment, map);
            }

            return xdocument;
        }

        private static void ParamTox(XElement x, Param param) {
            if (!string.IsNullOrEmpty(param.Autors)) { x.Add(new XAttribute("Autors", param.Autors)); }

            x.Add(new XAttribute("Radius", param.Radius));
            x.Add(new XAttribute("Thick", param.Thick));
            x.Add(new XAttribute("Indent", param.Indent));
            x.Add(new XAttribute("TextIndent", param.TextIndent));
            x.Add(new XAttribute("FontSize", param.FontSize));
            x.Add(new XAttribute("CoStation", param.CoStation));
            x.Add(new XAttribute("CoSegment", param.CoSegment));
            x.Add(new XAttribute("TransferWeight", param.TransferWeight));
            if (param.NumberNames > 1.5d) { x.Add(new XAttribute("NumberNames", 2)); }

            x.Add(new XAttribute("BackgroundColor", ColorToHexstring(param.Background.Color)));
            x.Add(new XAttribute("TransferColor", ColorToHexstring(param.TransferColor)));
            x.Add(new XAttribute("DimedColor", ColorToHexstring(param.DimedColor)));
            x.Add(new XAttribute("BuildColor", ColorToHexstring(param.BuildColor)));
        }

        private static void LineTox(XElement x, Line line) {
            var xe = new XElement("Line");
            x.Add(xe);

            var hexColor = ColorToHexstring(line.Color);
            xe.Add(new XAttribute("Color", hexColor));

            if (!string.IsNullOrEmpty(line.Name1)) { xe.Add(new XAttribute("Name1", line.Name1)); }
            if (!string.IsNullOrEmpty(line.Name2)) { xe.Add(new XAttribute("Name2", line.Name2)); }

            if (Math.Abs(line.Indent) > double.Epsilon) { xe.Add(new XAttribute("Indent", Math.Round(line.Indent, 0))); }
            if (Math.Abs(line.Thick) > double.Epsilon) { xe.Add(new XAttribute("Thick", Math.Round(line.Thick, 0))); }
        }

        private static void StationTox(XElement x, Station station) {
            var xe = new XElement("Station");
            x.Add(xe);

            xe.Add(new XAttribute("Left", Math.Round(station.Left, 0)));
            xe.Add(new XAttribute("Top", Math.Round(station.Top, 0)));
            if (station.Angle != 0) { xe.Add(new XAttribute("Angle", station.Angle)); }

            if (!string.IsNullOrEmpty(station.Name1)) { xe.Add(new XAttribute("Name1", station.Name1)); }
            if (!string.IsNullOrEmpty(station.Name2)) { xe.Add(new XAttribute("Name2", station.Name2)); }

            if (Math.Abs(station.Radius) > double.Epsilon) { xe.Add(new XAttribute("Radius", Math.Round(station.Radius, 0))); }
            if (Math.Abs(station.FontSize) > double.Epsilon) { xe.Add(new XAttribute("FontSize", Math.Round(station.FontSize, 0))); }
            if (Math.Abs(station.TextIndent) > double.Epsilon) { xe.Add(new XAttribute("TextIndent", Math.Round(station.TextIndent, 0))); }
            if (Math.Abs(station.TransferWeight) > double.Epsilon) { xe.Add(new XAttribute("TransferWeight", Math.Round(station.TransferWeight, 0))); }
        }

        private static void SegmentTox(XElement x, Segment segment, Map map) {
            var xe = new XElement("Segment");
            x.Add(xe);

            var s = string.Join(",", segment.Lines.Select(n => map.Lines.IndexOf(n) + 1));
            if (!string.IsNullOrEmpty(s)) { xe.Add(new XAttribute("L", s)); }

            int index;

            index = map.Stations.IndexOf(segment.Station1) + 1;
            if (index > 0) { xe.Add(new XAttribute("S1", index)); }

            index = map.Stations.IndexOf(segment.Station2) + 1;
            if (index > 0) { xe.Add(new XAttribute("S2", index)); }

            if (Math.Abs(segment.Indent) > double.Epsilon) { xe.Add(new XAttribute("Indent", Math.Round(segment.Indent, 0))); }
            if (Math.Abs(segment.Thick) > double.Epsilon) { xe.Add(new XAttribute("Thick", Math.Round(segment.Thick, 0))); }
            if (Math.Abs(segment.Weight - 1d) > double.Epsilon) { xe.Add(new XAttribute("Weight", Math.Round(segment.Weight, 0))); }
        }

        private static Map MapParse(XDocument xdocument) {
            var map = new Map();

            var xe = xdocument.Element("Map");
            if (xe != null) {
                XAttribute xa;
                xa = xe.Attribute("Name1");
                if (xa != null) { map.Name1 = (string)xa; }
                xa = xe.Attribute("Name2");
                if (xa != null) { map.Name2 = (string)xa; }

                var xParam = xe.Element("Param");
                if (xParam != null) {
                    ParamParse(xParam, map.Param);
                }

                var xLine = xe.Element("Lines");
                if (xLine != null) {
                    foreach (XElement x in xLine.Nodes().OfType<XElement>()) {
                        map.Lines.Add(LineParse(x));
                    }
                }

                var xStation = xe.Element("Stations");
                if (xStation != null) {
                    foreach (XElement x in xStation.Nodes().OfType<XElement>()) {
                        map.Stations.Add(StationParse(x));
                    }
                }

                var xSegment = xe.Element("Segments");
                if (xSegment != null) {
                    foreach (XElement x in xSegment.Nodes().OfType<XElement>()) {
                        SegmentParse(x, map);
                    }
                }
            }

            return map;
        }

        private static void ParamParse(XElement xe, Param param) {
            try {
                XAttribute xa;

                xa = xe.Attribute("Autors");
                if (xa != null) { param.Autors = (string)xa; }
                xa = xe.Attribute("Radius");
                if (xa != null) { param.Radius = (double)xa; }
                xa = xe.Attribute("Thick");
                if (xa != null) { param.Thick = (double)xa; }
                xa = xe.Attribute("Indent");
                if (xa != null) { param.Indent = (double)xa; }
                xa = xe.Attribute("TextIndent");
                if (xa != null) { param.TextIndent = (double)xa; }
                xa = xe.Attribute("FontSize");
                if (xa != null) { param.FontSize = (double)xa; }
                xa = xe.Attribute("CoStation");
                if (xa != null) { param.CoStation = (double)xa; }
                xa = xe.Attribute("CoSegment");
                if (xa != null) { param.CoSegment = (double)xa; }
                xa = xe.Attribute("TransferWeight");
                if (xa != null) { param.TransferWeight = (double)xa; }
                xa = xe.Attribute("NumberNames");
                if (xa != null) { param.NumberNames = (double)xa; }

                xa = xe.Attribute("BackgroundColor");
                param.Background = new SolidColorBrush(HexstringToColor(xa.Value));

                xa = xe.Attribute("TransferColor");
                param.TransferColor = HexstringToColor(xa.Value);

                xa = xe.Attribute("DimedColor");
                param.DimedColor = HexstringToColor(xa.Value);

                xa = xe.Attribute("BuildColor");
                param.BuildColor = HexstringToColor(xa.Value);
            }
            catch {
            }
        }

        private static Line LineParse(XElement xe) {
            var line = new Line();

            try {
                XAttribute xa;

                xa = xe.Attribute("Color");
                line.Color = HexstringToColor(xa.Value);

                xa = xe.Attribute("Name1");
                if (xa != null) { line.Name1 = (string)xa; }
                xa = xe.Attribute("Name2");
                if (xa != null) { line.Name2 = (string)xa; }

                xa = xe.Attribute("Indent");
                if (xa != null) { line.Indent = (double)xa; }
                xa = xe.Attribute("Thick");
                if (xa != null) { line.Thick = (double)xa; }
            }
            catch {
            }

            return line;
        }

        private static Station StationParse(XElement xe) {
            var station = new Station();

            try {
                XAttribute xa;
                xa = xe.Attribute("Left");
                if (xa != null) { station.Left = (double)xa; }
                xa = xe.Attribute("Top");
                if (xa != null) { station.Top = (double)xa; }
                xa = xe.Attribute("Angle");
                if (xa != null) { station.Angle = (double)xa; }
                xa = xe.Attribute("Name1");
                if (xa != null) { station.Name1 = (string)xa; }
                xa = xe.Attribute("Name2");
                if (xa != null) { station.Name2 = (string)xa; }
                xa = xe.Attribute("Radius");
                if (xa != null) { station.Radius = (double)xa; }
                xa = xe.Attribute("FontSize");
                if (xa != null) { station.FontSize = (double)xa; }
                xa = xe.Attribute("TextIndent");
                if (xa != null) { station.TextIndent = (double)xa; }
                xa = xe.Attribute("TransferWeight");
                if (xa != null) { station.TransferWeight = (double)xa; }
            }
            catch {
            }

            return station;
        }

        private static void SegmentParse(XElement xe, Map map) {
            var segment = new Segment();

            try {
                var xLine = xe.Attribute("L");
                var xStation1 = xe.Attribute("S1");
                var xStation2 = xe.Attribute("S2");

                if (xLine != null) {
                    var aLine = xLine.Value.Split(',');
                    foreach (var a in aLine) {
                        int iLine;
                        if (int.TryParse(a, out iLine)) {
                            if (iLine > 0 && iLine <= map.Lines.Count) {
                                segment.Lines.Add(map.Lines[iLine - 1]);
                            }
                            else {
                                segment.Lines.Add(null);
                            }
                        }
                    }
                }

                int iStation1 = 0, iStation2 = 0;

                if (xStation1 != null) { iStation1 = (int)xStation1 - 1; }
                if (xStation2 != null) { iStation2 = (int)xStation2 - 1; }

                var station1 = map.Stations[iStation1];
                var station2 = map.Stations[iStation2];

                segment.Station1 = station1;
                segment.Station2 = station2;

                map.Segments.Add(segment);
                station1.Segments.Add(segment);
                station2.Segments.Add(segment);

                var xa = xe.Attribute("Indent");
                if (xa != null) { segment.Indent = (double)xa; }
                xa = xe.Attribute("Thick");
                if (xa != null) { segment.Thick = (double)xa; }
                xa = xe.Attribute("Weight");
                if (xa != null) { segment.Weight = (double)xa; }
            }
            catch {
            }
        }

        private static string ColorToHexstring(Color color) {
            return "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        private static Color HexstringToColor(string hexstring) {
            if (hexstring != null && hexstring.Length > 8) {
                var a = GetByteColor(hexstring.Substring(1, 2));
                var r = GetByteColor(hexstring.Substring(3, 2));
                var g = GetByteColor(hexstring.Substring(5, 2));
                var b = GetByteColor(hexstring.Substring(7, 2));
                return Color.FromArgb(a, r, g, b);
            }

            return default(Color);
        }

        private static byte GetByteColor(string s) {
            byte b;
            byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out b);
            return b;
        }

        private static string GetFileName(string filename) {
            return Path.Combine(Statics.DataFolderName, Path.ChangeExtension(filename, Statics.FileExtension));
        }

        private static async Task SaveFile(Map map, StorageFile storageFile) {
            var xdocument = MapTox(map);
            using (var fileStream = await storageFile.OpenStreamForWriteAsync())
            using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Create)) {
                var entry = zip.CreateEntry("Scheme.xml");
                using (var entryStream = entry.Open()) {
                    xdocument.Save(entryStream, SaveOptions.DisableFormatting);
                    entryStream.Flush();
                }
            }
        }

        private static async Task<Map> OpenFile(StorageFile storageFile) {
            using (var fileStream = await storageFile.OpenStreamForReadAsync())
            using (var zip = new ZipArchive(fileStream)) {
                var entry = zip.Entries.First();
                using (var entryStream = entry.Open()) {
                    var xdocument = XDocument.Load(entryStream);
                    return MapParse(xdocument);
                }
            }
        }
    }
}
