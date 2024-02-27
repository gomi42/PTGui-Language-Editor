//
// Author:
//   Michael Göricke
//
// Copyright (c) 2024
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

namespace PTGui_Language_Editor
{
    //////////////////////////////////////

    public class LanguageTypeBase
    {
        protected JsonTypeBase JsonBase { get; init; } = null!;

        public string Id => JsonBase.id!;

        public bool? Machinetranslated
        {
            get => JsonBase.machinetranslated;
            set => JsonBase.machinetranslated = value;
        }
    }

    public class LanguageString : LanguageTypeBase
    {
        public LanguageString(JsonString json)
        {
            Json = json;
        }

        protected JsonString Json
        {
            get => (JsonString)JsonBase;
            init => JsonBase = value;
        }

        public string? Txt
        {
            get => Json.txt;
            set => Json.txt = value;
        }

        public string? Format
        {
            get => Json.format;
            set => Json.format = value;
        }

        public string? TxtFortranslate
        {
            get => Json.txtfortranslate;
            set => Json.txtfortranslate = value;
        }
    }

    public class LanguageTooltip : LanguageTypeBase
    {
        public LanguageTooltip(JsonTooltip json)
        {
            Json = json;
        }

        protected JsonTooltip Json
        {
            get => (JsonTooltip)JsonBase;
            init => JsonBase = value;
        }

        public string? Label
        {
            get => Json.label;
            set => Json.label = value;
        }

        public string? Helptext
        {
            get => Json.helptext;
            set => Json.helptext = value;
        }

        public string? MoreHelptext
        {
            get => Json.morehelptext;
            set => Json.morehelptext = value;
        }
    }

    public class LanguageHelpPage : LanguageTypeBase
    {
        public LanguageHelpPage(JsonHelpPage json)
        {
            Json = json;
        }

        protected JsonHelpPage Json
        {
            get => (JsonHelpPage)JsonBase;
            init => JsonBase = value;
        }

        public string? Helptext
        {
            get => Json.helptext;
            set => Json.helptext = value;
        }
    }

    public class LanguageGeneral : LanguageTypeBase
    {
        private JsonRoot json;

        public LanguageGeneral(JsonRoot json)
        {
            this.json = json;
        }
        public List<string>? Contributors
        {
            get => json.contributors;
            set => json.contributors = value;
        }

        public string? LanguageMameLocalized
        {
            get => json.languagenamelocalized;
            set => json.languagenamelocalized = value;
        }

        public string? StartupMessage
        {
            get => json.startupmsg;
            set => json.startupmsg = value;
        }
    }

    public class Language
    {
        public LanguageGeneral General { get; set; } = null!;

        public List<LanguageString> Strings { get; set; } = null!;

        public List<LanguageTooltip> Tooltips { get; set; } = null!;

        public List<LanguageHelpPage> HelpPages { get; set; } = null!;
    }

    //////////////////////////////////////

    public record Editor(EditorGeneral General, List<EditorString> Strings, List<EditorTooltip> Tooltips, List<EditorHelpPage> HelpPages);

    public record EditorGeneral(LanguageGeneral Reference, LanguageGeneral Translation);

    public record EditorString(LanguageString Reference, LanguageString Translation);

    public record EditorTooltip(LanguageTooltip Reference, LanguageTooltip Translation);

    public record EditorHelpPage(LanguageHelpPage Reference, LanguageHelpPage Translation);
}

