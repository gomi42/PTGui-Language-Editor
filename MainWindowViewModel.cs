using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PTGui_Language_Editor
{
    public class MainWindowViewModel : ViewModelBase
    {
        JsonRoot RefJsonRoot = null!;
        JsonRoot TransJsonRoot = null!;
        EditorTrans EditorTrans = null!;
        EditorRef EditorRef = null!;

        private DelegateCommand loadData;
        private DelegateCommand saveData;
        private GeneralViewModel generalViewModel;
        private StringsViewModel stringsViewModel;
        private TooltipsViewModel tooltipsViewModel;
        private HelpPagesViewModel helpViewModel;

        public MainWindowViewModel()
        {
            loadData = new DelegateCommand(OnLoadData);
            saveData = new DelegateCommand(OnSaveData);
        }

        public GeneralViewModel GeneralViewModel
        {
            get => generalViewModel;
            set
            {
                generalViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public StringsViewModel StringsViewModel
        {
            get => stringsViewModel;
            set
            {
                stringsViewModel = value;
                NotifyPropertyChanged();
            }
        }
        
        public TooltipsViewModel TooltipsViewModel 
        {
            get => tooltipsViewModel;
            set
            {
                tooltipsViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public HelpPagesViewModel HelpViewModel
        {
            get => helpViewModel;
            set
            {
                helpViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand LoadData => loadData;
        public ICommand SaveData => saveData;

        private void OnLoadData()
        {
            LoadLanguageFiles();
            BuildEditorTransData();
            BuildEditorRefData();

            ShowGeneral();
            ShowHelp();
            ShowTooltip();
            ShowString();
        }

        // https://code-maze.com/csharp-read-and-process-json-file/

        private void LoadLanguageFiles()
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

        ////////////////////////////////////////////////////////////////////////////////////////////

        private void ShowGeneral()
        {
            var vm = new GeneralViewModel();
            vm.EditorRef = EditorRef;
            vm.EditorTrans = EditorTrans;
            GeneralViewModel = vm;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        private void ShowHelp()
        {
            var vm = new HelpPagesViewModel();
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefHelpPages = EditorRef.HelpPages;
            HelpViewModel = vm;
        }

        private void ShowTooltip()
        {
            var vm = new TooltipsViewModel();
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefTooltips = EditorRef.Tooltips;
            TooltipsViewModel = vm;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////

        private void ShowString()
        {
            var vm = new StringsViewModel();
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefStrings = EditorRef.Strings;
            StringsViewModel = vm;
        }

        private void OnSaveData()
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
    }
}
