using System.Collections.Generic;
using System.Linq;

namespace PTGui_Language_Editor
{
    //////////////////////////////////////

    public class EditorBase
    {
        public JsonRoot Json { get; set; } = null!;
        
        public string Contributors
        {
            get
            {
                if (Json.contributors == null)
                {
                    return string.Empty;
                }

                return string.Join("\n", Json.contributors);
            }
            
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Json.contributors = null;
                }
                else
                {
                    Json.contributors = value.Split('\n').ToList();
                }
            }
        }

        public string? LanguageMameLocalized
        {
            get
            {
                return Json.languagenamelocalized;
            }

            set
            {
                Json.languagenamelocalized = value;
            }
        }

        public string? StartupMessage
        {
            get
            {
                return Json.startupmsg;
            }

            set
            {
                Json.startupmsg = value;
            }
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
        public EditorTransHelpPage? EditorTranslate { get; init; }
    }

    public class EditorRefString : EditorTransString
    {
        public EditorTransString? EditorTranslate { get; init; }
    }

    public class EditorRefTooltip : EditorTransTooltip
    {
        public EditorTransTooltip? EditorTranslate { get; init; }
    }
}

