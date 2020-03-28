// <copyright file="Catalog.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace HappyRoute4.Models {
    public class Catalog {
        public Catalog(string name) {
            Name = name;
            Schemas = new List<Schema>();
        }

        public string Name { get; private set; }
        public List<Schema> Schemas { get; private set; }
    }
}
