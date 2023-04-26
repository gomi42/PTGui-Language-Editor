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
using System.Linq;

namespace PTGui_Language_Editor
{
    //////////////////////////////////////

    public class EditorBase
    {
        public JsonRoot Json { get; set; } = null!;
        
        public List<string>? Contributors
        {
            get => Json.contributors;
            set => Json.contributors = value;
        }

        public string? LanguageMameLocalized
        {
            get => Json.languagenamelocalized;
            set => Json.languagenamelocalized = value;
        }

        public string? StartupMessage
        {
            get => Json.startupmsg;
            set => Json.startupmsg = value;
        }
    }

    public class EditorTrans : EditorBase
    {
        public List<EditorTransHelpPage> HelpPages { get; set; } = null!;
        public List<EditorTransString> Strings { get; set; } = null!;
        public List<EditorTransTooltip> Tooltips { get; set; } = null!;
    }

    public class EditorTypeBase
    {
        protected JsonTypeBase JsonBase { get; init; } = null!;
        public string Id => JsonBase.id!;
        public bool? Machinetranslated
        {
            get => JsonBase.machinetranslated;
            set => JsonBase.machinetranslated = value;
        }
    }

    public class EditorTransHelpPage : EditorTypeBase
    {
        public JsonHelpPage Json
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

    public class EditorTransString : EditorTypeBase
    {
        public JsonString Json
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

    public class EditorTransTooltip : EditorTypeBase
    {
        public JsonTooltip Json
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

    //////////////////////////////////////

    public class EditorRef : EditorBase
    {
        public List<EditorRefHelpPage> HelpPages { get; set; } = null!;
        public List<EditorRefString> Strings { get; set; } = null!;
        public List<EditorRefTooltip> Tooltips { get; set; } = null!;
    }

    public class EditorRefHelpPage : EditorTransHelpPage
    {
        public EditorTransHelpPage EditorTranslate { get; init; } = null!;
    }

    public class EditorRefString : EditorTransString
    {
        public EditorTransString EditorTranslate { get; init; } = null!;
    }

    public class EditorRefTooltip : EditorTransTooltip
    {
        public EditorTransTooltip EditorTranslate { get; init; } = null!;
    }
}

