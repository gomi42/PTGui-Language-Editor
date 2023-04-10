using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PTGui_Language_Editor
{
    // https://json2csharp.com/
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

    public class JsonTypeBase
    {
        public string? id { get; set; }
        public bool? machinetranslated { get; set; }
    }

    public class JsonHelpPage : JsonTypeBase
    {
        public string? helptext { get; set; }
    }

    public class JsonString : JsonTypeBase
    {
        public string? txt { get; set; }
        public string? format { get; set; }
        public string? txtfortranslate { get; set; }
    }

    public class JsonTooltip : JsonTypeBase
    {
        public string? label { get; set; }
        public string? helptext { get; set; }
        public string? morehelptext { get; set; }
    }
}

