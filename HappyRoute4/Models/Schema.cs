// <copyright file="Schema.cs" company="Makajda">
// Copyright (c) Makajda. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace HappyRoute4.Models {
    public class Schema {
        public Schema(string name) {
            Name = name;
        }

        public string Name { get; set; }
        public bool IsMy { get; set; }
    }
}
