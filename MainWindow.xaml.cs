using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            ShowGeneral();
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
            EditorTrans.Json = TransJsonRoot;
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
            EditorRef.Json = RefJsonRoot;
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
        
        public GeneralViewModel GeneralViewModel { get; set; }

        private void ShowGeneral()
        {
            var vm = new GeneralViewModel();
            vm.EditorRef = EditorRef;
            vm.EditorTrans = EditorTrans;

            GeneralTab.DataContext = vm;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////
        
        private void ShowHelp()
        {
            var vm = new HelpPagesViewModel();
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefHelpPages = EditorRef.HelpPages;
            HelpPagesTab.DataContext = vm;
        }

        private void ShowTooltip()
        {
            var vm = new TooltipsViewModel();
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefTooltips = EditorRef.Tooltips;
            TooltipsTab.DataContext = vm;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////

        public StringsViewModel StringsViewModel { get; set; }
        public TooltipsViewModel TooltipsViewModel { get; set; }
        public HelpPagesViewModel HelpViewModel { get; set; }

        private void ShowString()
        {
            var vm = new StringsViewModel();
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefStrings = EditorRef.Strings;
            StringsTab.DataContext = vm;
        }
    }
}

