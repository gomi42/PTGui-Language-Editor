using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PTGui_Language_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JsonRoot RefJsonRoot = null!;
        JsonRoot TransJsonRoot = null!;
        EditorTrans EditorTrans = null!;
        EditorRef EditorRef = null!;
        int showHelpIndex = 0;
        int showTooltipIndex = 0;
        int showStringIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UseFileOpenReadTextWithSystemTextJson();
            BuildEditorTransData();
            BuildEditorRefData();
            ShowHelp();
            ShowTooltip();
            ShowString();
        }

        // https://code-maze.com/csharp-read-and-process-json-file/

        private void UseFileOpenReadTextWithSystemTextJson()
        {
            using (FileStream json = File.OpenRead(@"D:\Programme\Pano, Web\PtGui Localization\en_us.nhloc"))
            {
                RefJsonRoot = JsonSerializer.Deserialize<JsonRoot>(json, JsonSerializerOptions.Default) ?? null!;
            }

            using (FileStream json = File.OpenRead(@"D:\Programme\Pano, Web\PtGui Localization\de_de.nhloc"))
            {
                TransJsonRoot = JsonSerializer.Deserialize<JsonRoot>(json, JsonSerializerOptions.Default) ?? null!;
            }
        }

        private void BuildEditorTransData()
        {
            EditorTrans = new EditorTrans();
            EditorTrans.Strings = new List<EditorTransString>();

            foreach (var item in TransJsonRoot.strings!)
            {
                var str = new EditorTransString
                {
                    Json = item,
                    Txt = item.txt,
                    Format = item.format,
                    TxtFortranslate = item.txtfortranslate
                };
                EditorTrans.Strings.Add(str);
            }

            EditorTrans.HelpPages = new List<EditorTransHelpPage>();

            foreach (var item in TransJsonRoot.helppages!)
            {
                var str = new EditorTransHelpPage
                {
                    Json = item,
                    Helptext = item.helptext
                };
                EditorTrans.HelpPages.Add(str);
            }

            EditorTrans.Tooltips = new List<EditorTransTooltip>();

            foreach (var item in TransJsonRoot.tooltips!)
            {
                var str = new EditorTransTooltip
                {
                    Json = item,
                    Helptext = item.helptext
                };
                EditorTrans.Tooltips.Add(str);
            }
        }

        private void BuildEditorRefData()
        {
            EditorRef = new EditorRef();
            EditorRef.Strings = new List<EditorRefString>();

            foreach (var item in RefJsonRoot.strings!)
            {
                var str = new EditorRefString
                {
                    Json = item,
                    EditorTranslate = EditorTrans.Strings.FirstOrDefault(x => x.Id == item.id)
                };
                EditorRef.Strings.Add(str);
            }

            EditorRef.HelpPages = new List<EditorRefHelpPage>();

            foreach (var item in RefJsonRoot.helppages!)
            {
                var str = new EditorRefHelpPage
                {
                    Json = item,
                    EditorTranslate = EditorTrans.HelpPages.FirstOrDefault(x => x.Id == item.id)
                };
                EditorRef.HelpPages.Add(str);
            }

            EditorRef.Tooltips = new List<EditorRefTooltip>();

            foreach (var item in RefJsonRoot.tooltips!)
            {
                var str = new EditorRefTooltip
                {
                    Json = item,
                    EditorTranslate = EditorTrans.Tooltips.FirstOrDefault(x => x.Id == item.id)
                };
                EditorRef.Tooltips.Add(str);
            }
        }

        private void Button_ClickSave(object sender, RoutedEventArgs e)
        {
            using FileStream json = File.OpenWrite(@"D:\Programme\Pano, Web\PtGui Localization\de_de2.nhloc");

            var jso = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            JsonSerializer.Serialize(json, TransJsonRoot, jso);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        private void ShowHelp()
        {
            HelpId.Text = "#" + EditorRef.HelpPages[showHelpIndex].Id;
            HelpRef.Document = PTGuiTextConverter.ConvertToFlowDocument(EditorRef.HelpPages[showHelpIndex].Helptext, y => EditorRef.Strings.FirstOrDefault(x => x.Id == y)?.Txt);

            var str = EditorRef.HelpPages[showHelpIndex].EditorTranslate?.Helptext?.Replace("<br>", "\n");
            HelpTrans.Text = str;
        }

        private void HelpTransTextChanged(object sender, TextChangedEventArgs e)
        {
            if (EditorTrans == null)
            {
                return;
            }

            Rtb.Document = PTGuiTextConverter.ConvertToFlowDocument(HelpTrans.Text, y => EditorTrans.Strings.FirstOrDefault(x => x.Id == y)?.Txt);
            var str = HelpTrans.Text.Replace("\n", "<br>");
            EditorRef.HelpPages[showHelpIndex].EditorTranslate.Helptext = str;
            EditorRef.HelpPages[showHelpIndex].EditorTranslate.Machinetranslated = null;
        }

        private void Button_ClickPrev(object sender, RoutedEventArgs e)
        {
            showHelpIndex--;

            if (showHelpIndex < 0)
            {
                showHelpIndex = EditorRef.HelpPages.Count - 1;
            }

            ShowHelp();
        }

        private void Button_ClickNext(object sender, RoutedEventArgs e)
        {
            showHelpIndex++;

            if (showHelpIndex >= EditorRef.HelpPages.Count)
            {
                showHelpIndex = 0;
            }

            ShowHelp();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        private void ShowTooltip()
        {
            TooltipId.Text = "#" + EditorRef.Tooltips[showTooltipIndex].Id;

            ShowTooltipLabel();
            ShowTooltipHelpText();
            ShowTooltipMoreHelp();
        }

        private void ShowTooltipLabel()
        {
            TooltipRefLabel.Document = PTGuiTextConverter.ConvertToFlowDocument(EditorRef.Tooltips[showTooltipIndex].Label, y => EditorRef.Strings.FirstOrDefault(x => x.Id == y)?.Txt);

            var str = EditorRef.Tooltips[showTooltipIndex].EditorTranslate?.Label?.Replace("<br>", "\n");
            TooltipTransLabel.Text = str;
        }

        private void ShowTooltipHelpText()
        {
            TooltipRefHelpTxt.Document = PTGuiTextConverter.ConvertToFlowDocument(EditorRef.Tooltips[showTooltipIndex].Helptext, y => EditorRef.Strings.FirstOrDefault(x => x.Id == y)?.Txt);

            var str = EditorRef.Tooltips[showTooltipIndex].EditorTranslate?.Helptext?.Replace("<br>", "\n");
            TooltipTransHelpTxt.Text = str;
        }

        private void ShowTooltipMoreHelp()
        {
            TooltipRefMoreHelpTxt.Document = PTGuiTextConverter.ConvertToFlowDocument(EditorRef.Tooltips[showTooltipIndex].MoreHelptext, y => EditorRef.Strings.FirstOrDefault(x => x.Id == y)?.Txt);

            var str = EditorRef.Tooltips[showTooltipIndex].EditorTranslate?.MoreHelptext?.Replace("<br>", "\n");
            TooltipTransMoreHelpTxt.Text = str;
        }

        private void TooltipTransLabelChanged(object sender, TextChangedEventArgs e)
        {
            if (EditorTrans == null)
            {
                return;
            }

            TooltipTransLabelShow.Document = PTGuiTextConverter.ConvertToFlowDocument(TooltipTransLabel.Text, y => EditorTrans.Strings.FirstOrDefault(x => x.Id == y)?.Txt);
            var str = TooltipTransLabel.Text.Replace("\n", "<br>");
            EditorRef.Tooltips[showTooltipIndex].EditorTranslate.Label = str;
            EditorRef.Tooltips[showTooltipIndex].EditorTranslate.Machinetranslated = null;
        }

        private void TooltipTransHelpTextChanged(object sender, TextChangedEventArgs e)
        {
            if (EditorTrans == null)
            {
                return;
            }

            TooltipTransHelpTxtShow.Document = PTGuiTextConverter.ConvertToFlowDocument(TooltipTransHelpTxt.Text, y => EditorTrans.Strings.FirstOrDefault(x => x.Id == y)?.Txt);
            var str = TooltipTransHelpTxt.Text.Replace("\n", "<br>");
            EditorRef.Tooltips[showTooltipIndex].EditorTranslate.Helptext = str;
            EditorRef.Tooltips[showTooltipIndex].EditorTranslate.Machinetranslated = null;
        }

        private void TooltipTransMoreHelpChanged(object sender, TextChangedEventArgs e)
        {
            if (EditorTrans == null)
            {
                return;
            }

            TooltipTransMoreHelpTxtShow.Document = PTGuiTextConverter.ConvertToFlowDocument(TooltipTransMoreHelpTxt.Text, y => EditorTrans.Strings.FirstOrDefault(x => x.Id == y)?.Txt);
            var str = TooltipTransMoreHelpTxt.Text.Replace("\n", "<br>");
            EditorRef.Tooltips[showTooltipIndex].EditorTranslate.MoreHelptext = str;
            EditorRef.Tooltips[showTooltipIndex].EditorTranslate.Machinetranslated = null;
        }

        private void Button_TooltipPrev(object sender, RoutedEventArgs e)
        {
            showTooltipIndex--;

            if (showTooltipIndex < 0)
            {
                showTooltipIndex = EditorRef.Tooltips.Count - 1;
            }

            ShowTooltip();
        }

        private void Button_TooltipNext(object sender, RoutedEventArgs e)
        {
            showTooltipIndex++;

            if (showTooltipIndex >= EditorRef.Tooltips.Count)
            {
                showTooltipIndex = 0;
            }

            ShowTooltip();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        const int NumStringsPerPage = 10;
        
        private void ShowString()
        {
            //StringId.Text = EditorRef.Strings[showStringIndex].Id;
            //StringRef.Document = PTGuiTextConverter.ConvertToFlowDocument(EditorRef.Strings[showStringIndex].Txt, y => EditorRef.Strings.FirstOrDefault(x => x.Id == y)?.Txt);

            //if (EditorRef.Strings[showStringIndex].EditorTranslate?.Format == "html")
            //{
            //    var str = EditorRef.Strings[showStringIndex].EditorTranslate?.Txt?.Replace("<br>", "\n");
            //    StringTrans.Text = str;
            //}
            //else
            //{
            //    StringTrans.Text = EditorRef.Strings[showStringIndex].EditorTranslate?.Txt;
            //}

            var list = new List<OneString>();

            for (int i = showStringIndex; i < showStringIndex + NumStringsPerPage && showStringIndex < EditorRef.Strings.Count(); i++)
            {
                var one = new OneString();
                one.Id = "#" + EditorRef.Strings[i].Id;
                one.Ref = PTGuiTextConverter.ConvertToFlowDocument(EditorRef.Strings[i].Txt, y => EditorRef.Strings.FirstOrDefault(x => x.Id == y)?.Txt);
                one.EditorTranslate = EditorRef.Strings[i].EditorTranslate;
                one.TransStrings = EditorTrans.Strings;

                if (EditorRef.Strings[i].EditorTranslate?.Format == "html")
                {
                    var str = EditorRef.Strings[i].EditorTranslate?.Txt?.Replace("<br>", "\n");
                    one.Trans = str;
                }
                else
                {
                    one.Trans = EditorRef.Strings[i].EditorTranslate?.Txt;
                }

                one.TransShow = PTGuiTextConverter.ConvertToFlowDocument(one.Trans, y => EditorTrans.Strings.FirstOrDefault(x => x.Id == y)?.Txt);

                list.Add(one);
            }

            Strings.ItemsSource = list;
        }

        private void StringTransTextChanged(object sender, TextChangedEventArgs e)
        {
            if (EditorTrans == null)
            {
                return;
            }

            //StringTransShow.Document = PTGuiTextConverter.ConvertToFlowDocument(StringTrans.Text, y => EditorTrans.Strings.FirstOrDefault(x => x.Id == y)?.Txt);

            //if (EditorRef.Strings[showStringIndex].EditorTranslate?.Format == "html")
            //{
            //    var str = StringTrans.Text.Replace("\n", "<br>");
            //    EditorRef.Strings[showStringIndex].EditorTranslate.Txt = str;
            //}
            //else
            //{
            //    EditorRef.Strings[showStringIndex].EditorTranslate.Txt = StringTrans.Text;
            //}

            //EditorRef.Strings[showStringIndex].EditorTranslate.Machinetranslated = null;
        }

        private void Button_StringPrev(object sender, RoutedEventArgs e)
        {
            showStringIndex -= NumStringsPerPage;

            if (showStringIndex < 0)
            {
                showStringIndex = 0;
            }

            ShowString();
        }

        private void Button_StringNext(object sender, RoutedEventArgs e)
        {
            showStringIndex += NumStringsPerPage;

            if (showStringIndex >= EditorRef.Strings.Count)
            {
                showStringIndex -= NumStringsPerPage;
            }

            ShowString();
        }
    }

    //////////////////////////////////////

    public class OneString : INotifyPropertyChanged
    {
        private FlowDocument transShow;
        private string? trans;

        public string Id { get; set; }
        public FlowDocument Ref { get; set; }
        public FlowDocument TransShow
        {
            get => transShow;
            set
            {
                transShow = value;
                NotifyPropertyChanged();
            }
        }
        public string? Trans 
        {
            get => trans;
            set
            {
                trans = value;
                TransShow = PTGuiTextConverter.ConvertToFlowDocument(trans, y => TransStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                if (EditorTranslate?.Format == "html")
                {
                    var str = trans.Replace("\n", "<br>");
                    EditorTranslate.Txt = str;
                }
                else
                {
                    EditorTranslate.Txt = trans;
                }

                EditorTranslate.Machinetranslated = null;
            }
        }
        public EditorTransString EditorTranslate { get; set; }
        public List<EditorTransString> TransStrings { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //////////////////////////////////////

    public class EditorTrans
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

    public class EditorRef
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

    //////////////////////////////////////

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

