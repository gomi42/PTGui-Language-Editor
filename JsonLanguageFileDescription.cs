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

