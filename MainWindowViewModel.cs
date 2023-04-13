using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace PTGui_Language_Editor
{
    public class MainWindowViewModel : ViewModelBase
    {
        string languageFilesRoot = @"D:\Programme\Pano, Web\PtGui Localization";
        JsonRoot RefJsonRoot = null!;
        JsonRoot TransJsonRoot = null!;
        EditorTrans EditorTrans = null!;
        EditorRef EditorRef = null!;
        bool isModified;

        private List<string> languageFiles = null!;
        private string? previousLanguageFile;
        private string? newSelectedLanguageFile;
        private string? selectedLanguageFile;
        private string searchText = string.Empty;
        private DelegateCommand returnSearch;
        private DelegateCommand loadData;
        private DelegateCommand saveData;
        private GeneralViewModel generalViewModel;
        private StringsViewModel stringsViewModel;
        private TooltipsViewModel tooltipsViewModel;
        private HelpPagesViewModel helpViewModel;

        public MainWindowViewModel()
        {
            CloseCommand = new DelegateCommand(OnClose);
            loadData = new DelegateCommand(OnLoadData);
            saveData = new DelegateCommand(OnSaveData, CanSaveData);
            returnSearch = new DelegateCommand(OnReturnSearch);
            ScanLanugageFiles();
        }

        public List<string> LanguageFiles
        {
            get => languageFiles;
            set
            {
                languageFiles = value;
                NotifyPropertyChanged();
            }
        }

        public string? SelectedLanguageFile
        {
            get => selectedLanguageFile;
            set
            {
                if (isModified)
                {
                    previousLanguageFile = selectedLanguageFile;
                    AskContinue();
                }

                selectedLanguageFile = value;
                NotifyPropertyChanged();
                
                if (!isModified)
                {
                    EditLanguage();
                }
            }
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

        public ICommand CloseCommand { get; init; }
        public ICommand LoadData => loadData;
        public ICommand SaveData => saveData;
        public ICommand ReturnSearch => returnSearch;

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                NotifyPropertyChanged();
            }
        }

        private void OnLoadData()
        {
            var dialog = new FolderBrowserDialog();
            {
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    languageFilesRoot = dialog.SelectedPath;
                    LoadLanguageFiles();
                }
            }

        }

        private void ScanLanugageFiles()
        {
            var files = Directory.GetFiles(languageFilesRoot, "*.nhloc");
            var list = new List<string>();

            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file);

                if (filename != "en_us")
                {
                    list.Add(filename);
                }
            }

            LanguageFiles = list;
            SelectedLanguageFile = list.FirstOrDefault(x => x == "de_de");
        }

        private async Task<bool> AskContinue()
        {
            var dlg = new NotSavedDialogViewModel();
            dlg.Title = "Language Modified";
            dlg.ErrorMessage = "Your changes haven't been saved yet. Do you want to save them now?";

            var ret = await dlg.ShowDialog();

            switch (ret)
            {
                case DialogOkNoCancel.Yes:
                    SaveModifiedData(previousLanguageFile);
                    EditLanguage();
                    return true;

                case DialogOkNoCancel.No:
                    EditLanguage();
                    return true;

                case DialogOkNoCancel.Cancel:
                    selectedLanguageFile = previousLanguageFile;
                    NotifyPropertyChanged(nameof(SelectedLanguageFile));
                    return false;
            }

            return false;
        }

        private async void OnClose()
        {
            if (!isModified)
            {
                Environment.Exit(0);
                return;
            }

            var dlg = new NotSavedDialogViewModel();
            dlg.Title = "Language Modified";
            dlg.ErrorMessage = "Your changes haven't been saved yet. Do you want to save them now?";

            var ret = await dlg.ShowDialog();

            switch (ret)
            {
                case DialogOkNoCancel.Yes:
                    SaveModifiedData(selectedLanguageFile);
                    Environment.Exit(0);
                    break;

                case DialogOkNoCancel.No:
                    Environment.Exit(0);
                    break;

                case DialogOkNoCancel.Cancel:
                    break;
            }
        }


        private void EditLanguage()
        {
            LoadLanguageFiles();
            BuildEditorTransData();
            BuildEditorRefData();

            ShowGeneral();
            ShowString();
            ShowTooltip();
            ShowHelp();

            SearchText = string.Empty;
            isModified = false;
            saveData.RaiseCanExecuteChanged();
        }

        private string GetLanguageFilename(string filename)
        {
            return Path.Combine(languageFilesRoot, filename + ".nhloc");
        }

        // https://code-maze.com/csharp-read-and-process-json-file/

        private void LoadLanguageFiles()
        {
            using (FileStream json = File.OpenRead(GetLanguageFilename("en_us")))
            {
                RefJsonRoot = JsonSerializer.Deserialize<JsonRoot>(json, JsonSerializerOptions.Default) ?? null!;
            }

            using (FileStream json = File.OpenRead(GetLanguageFilename(SelectedLanguageFile)))
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
            var vm = new GeneralViewModel(SetModified);
            vm.EditorRef = EditorRef;
            vm.EditorTrans = EditorTrans;
            GeneralViewModel = vm;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        private void ShowHelp()
        {
            var vm = new HelpPagesViewModel(SetModified);
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefHelpPages = EditorRef.HelpPages;
            HelpViewModel = vm;
        }

        private void ShowTooltip()
        {
            var vm = new TooltipsViewModel(SetModified);
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefTooltips = EditorRef.Tooltips;
            TooltipsViewModel = vm;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////

        private void ShowString()
        {
            var vm = new StringsViewModel(SetModified);
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefStrings = EditorRef.Strings;
            StringsViewModel = vm;
        }

        private void OnSaveData()
        {
            SaveModifiedData(SelectedLanguageFile);
        }

        private void SaveModifiedData(string filename)
            {
                using FileStream json = File.Create(GetLanguageFilename(filename));

            var jso = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            JsonSerializer.Serialize(json, TransJsonRoot, jso);

            isModified = false;
            saveData.RaiseCanExecuteChanged();
        }

        private bool CanSaveData()
        {
            return isModified;
        }

        private void OnReturnSearch()
        {
            var search = SearchText;

            if (string.IsNullOrEmpty(search))
            {
                StringsViewModel.AllDisplayRefStrings = EditorRef.Strings;
                TooltipsViewModel.AllDisplayRefTooltips = EditorRef.Tooltips;
                HelpViewModel.AllDisplayRefHelpPages = EditorRef.HelpPages;
                return;
            }

            //////////////
            var strings = new List<EditorRefString>();

            foreach (var str in EditorRef.Strings)
            {
                if (str.Id.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.Txt != null && str.Txt.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.EditorTranslate.Txt != null && str.EditorTranslate.Txt.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                {
                    strings.Add(str);
                }
            }

            StringsViewModel.AllDisplayRefStrings = strings;

            //////////////
            var tooltips = new List<EditorRefTooltip>();

            foreach (var str in EditorRef.Tooltips)
            {
                if (str.Id.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.Label != null && str.Label.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.Helptext != null && str.Helptext.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.MoreHelptext != null && str.MoreHelptext.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.EditorTranslate.Label != null && str.EditorTranslate.Label.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.EditorTranslate.Helptext != null && str.EditorTranslate.Helptext.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.EditorTranslate.MoreHelptext != null && str.EditorTranslate.MoreHelptext.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                {
                    tooltips.Add(str);
                }
            }

            TooltipsViewModel.AllDisplayRefTooltips = tooltips;

            //////////////
            var helppages = new List<EditorRefHelpPage>();

            foreach (var str in EditorRef.HelpPages)
            {
                if (str.Id.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.Helptext.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.EditorTranslate.Helptext.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                {
                    helppages.Add(str);
                }
            }

            HelpViewModel.AllDisplayRefHelpPages = helppages;
        }

        private void SetModified()
        {
            isModified = true;
            saveData.RaiseCanExecuteChanged();
        }
    }
}
