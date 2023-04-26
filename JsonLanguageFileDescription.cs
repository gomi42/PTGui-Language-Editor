//
// Author:
//   Michael Göricke
//
// Copyright (c) 2023
//
// This file is part of PTGui Language Editor.
//
// The PTGui Language Editor is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see<http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PTGui_Language_Editor
{
    public class JsonRoot
    {
        [JsonPropertyName("$schema")]
        public string? schema { get; set; }
        public string? line_endings { get; set; }
        public List<string>? contributors { get; set; }
        public string? languagenamelocalized { get; set; }
        public string? startupmsg { get; set; }
        public List<JsonString>? strings { get; set; }
        public List<JsonTooltip>? tooltips { get; set; }
        public List<JsonHelpPage>? helppages { get; set; }
    }

    /// <summary>
    /// The typed classes need to implement ALL properties. This ensures
    /// that the serializer keeps the order when serialized back to file.
    /// That's why the base class needs to be abstract.
    /// </summary>
    public abstract class JsonTypeBase
    {
        public abstract string? id { get; set; }
        public abstract bool? machinetranslated { get; set; }
    }

    public class JsonHelpPage : JsonTypeBase
    {
        public override string? id { get; set; }
        public override bool? machinetranslated { get; set; }
        public string? helptext { get; set; }
    }

    public class JsonString : JsonTypeBase
    {
        public override string? id { get; set; }
        public override bool? machinetranslated { get; set; }
        public string? format { get; set; }
        public string? txt { get; set; }
        public string? txtfortranslate { get; set; }
    }

    public class JsonTooltip : JsonTypeBase
    {
        public override string? id { get; set; }
        public override bool? machinetranslated { get; set; }
        public string? label { get; set; }
        public string? helptext { get; set; }
        public string? morehelptext { get; set; }
    }
}

