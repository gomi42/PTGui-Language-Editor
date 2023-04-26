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
        bool isAskSaveChangesDialogOpen;

        private List<string> languageFiles = null!;
        private string? previousLanguageFile;
        private string? selectedLanguageFile;
        private string searchText = string.Empty;
        private GeneralViewModel generalViewModel = null!;
        private StringsViewModel stringsViewModel = null!;
        private TooltipsViewModel tooltipsViewModel = null!;
        private HelpPagesViewModel helpViewModel = null!;

        public MainWindowViewModel()
        {
            CloseCommand = new DelegateCommand(OnClose);
            LoadData = new DelegateCommand(OnLoadData);
            SaveData = new DelegateCommand(OnSaveData, CanSaveData);
            ReturnSearch = new DelegateCommand(OnReturnSearch);
            ScanLanguageFiles();
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
        public DelegateCommand LoadData { get; init; }
        public DelegateCommand SaveData { get; init; }
        public DelegateCommand ReturnSearch { get; init; }

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
                    ScanLanguageFiles();
                    LoadLanguageFiles();
                }
            }

        }

        private void ScanLanguageFiles()
        {
            if (!Directory.Exists(languageFilesRoot))
            {
                return;
            }

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

        private async Task<DialogOkNoCancel> ShowAskSaveChangesDialog()
        {
            isAskSaveChangesDialogOpen = true;

            var dlg = new NotSavedDialogViewModel();
            dlg.Title = "Language Modified";
            dlg.ErrorMessage = "Your changes haven't been saved yet. Do you want to save them now?";

            var ret = await dlg.ShowDialog();

            isAskSaveChangesDialogOpen = false;

            return ret;
        }

        private async void AskContinue()
        {
            var ret = await ShowAskSaveChangesDialog();

            switch (ret)
            {
                case DialogOkNoCancel.Yes:
                    SaveModifiedData(previousLanguageFile!);
                    EditLanguage();
                    break;

                case DialogOkNoCancel.No:
                    EditLanguage();
                    break;

                case DialogOkNoCancel.Cancel:
                    //sorry for this trick that resets the language
                    //without using the setter which in turn restarts
                    //the change check
                    selectedLanguageFile = previousLanguageFile;
                    NotifyPropertyChanged(nameof(SelectedLanguageFile));
                    break;
            }
        }

        private async void OnClose()
        {
            if (isAskSaveChangesDialogOpen)
            {
                return;
            }

            if (!isModified)
            {
                Environment.Exit(0);
                return;
            }

            var ret = await ShowAskSaveChangesDialog();

            switch (ret)
            {
                case DialogOkNoCancel.Yes:
                    SaveModifiedData(selectedLanguageFile!);
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

            PrepareGeneral();
            PrepareString();
            PrepareTooltip();
            PrepareHelp();

            SearchText = string.Empty;
            isModified = false;
            SaveData.RaiseCanExecuteChanged();
        }

        private string GetLanguageFilename(string filename)
        {
            return Path.Combine(languageFilesRoot, filename + ".nhloc");
        }

        private void LoadLanguageFiles()
        {
            using (FileStream json = File.OpenRead(GetLanguageFilename("en_us")))
            {
                RefJsonRoot = JsonSerializer.Deserialize<JsonRoot>(json, JsonSerializerOptions.Default) ?? null!;
            }

            using (FileStream json = File.OpenRead(GetLanguageFilename(SelectedLanguageFile!)))
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
                };
                EditorTrans.Strings.Add(str);
            }

            EditorTrans.HelpPages = new List<EditorTransHelpPage>();

            foreach (var item in TransJsonRoot.helppages!)
            {
                var str = new EditorTransHelpPage
                {
                    Json = item,
                };
                EditorTrans.HelpPages.Add(str);
            }

            EditorTrans.Tooltips = new List<EditorTransTooltip>();

            foreach (var item in TransJsonRoot.tooltips!)
            {
                var str = new EditorTransTooltip
                {
                    Json = item,
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
                var translate = EditorTrans.Strings.FirstOrDefault(x => x.Id == item.id);

                if (translate == null)
                {
                    var js = new JsonString { id = item.id, format = item.format };
                    TransJsonRoot.strings!.Add(js);
                    translate = new EditorTransString { Json = js };
                }

                var str = new EditorRefString
                {
                    Json = item,
                    EditorTranslate = translate
                };
                EditorRef.Strings.Add(str);
            }

            EditorRef.HelpPages = new List<EditorRefHelpPage>();

            foreach (var item in RefJsonRoot.helppages!)
            {
                var translate = EditorTrans.HelpPages.FirstOrDefault(x => x.Id == item.id);
                
                if (translate == null)
                {
                    var js = new JsonHelpPage { id = item.id };
                    TransJsonRoot.helppages!.Add(js);
                    translate = new EditorTransHelpPage { Json = js };
                }

                var str = new EditorRefHelpPage
                {
                    Json = item,
                    EditorTranslate = translate
                };
                EditorRef.HelpPages.Add(str);
            }

            EditorRef.Tooltips = new List<EditorRefTooltip>();

            foreach (var item in RefJsonRoot.tooltips!)
            {
                var translate = EditorTrans.Tooltips.FirstOrDefault(x => x.Id == item.id);
                
                if (translate == null)
                {
                    var js = new JsonTooltip{ id = item.id };
                    TransJsonRoot.tooltips!.Add(js);
                    translate = new EditorTransTooltip { Json = js };
                }

                var str = new EditorRefTooltip
                {
                    Json = item,
                    EditorTranslate = translate
                };
                EditorRef.Tooltips.Add(str);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        private void PrepareGeneral()
        {
            var vm = new GeneralViewModel(SetModified);
            vm.EditorRef = EditorRef;
            vm.EditorTrans = EditorTrans;
            GeneralViewModel = vm;
        }

        private void PrepareHelp()
        {
            var vm = new HelpPagesViewModel(SetModified);
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefHelpPages = EditorRef.HelpPages;
            HelpViewModel = vm;
        }

        private void PrepareTooltip()
        {
            var vm = new TooltipsViewModel(SetModified);
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefTooltips = EditorRef.Tooltips;
            TooltipsViewModel = vm;
        }

        private void PrepareString()
        {
            var vm = new StringsViewModel(SetModified);
            vm.AllRefStrings = EditorRef.Strings;
            vm.AllTransStrings = EditorTrans.Strings;
            vm.AllDisplayRefStrings = EditorRef.Strings;
            StringsViewModel = vm;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        private void OnSaveData()
        {
            SaveModifiedData(SelectedLanguageFile!);
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
            SaveData.RaiseCanExecuteChanged();
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
                    || str.Helptext != null && str.Helptext.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || str.EditorTranslate.Helptext != null && str.EditorTranslate.Helptext.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                {
                    helppages.Add(str);
                }
            }

            HelpViewModel.AllDisplayRefHelpPages = helppages;
        }

        private void SetModified()
        {
            isModified = true;
            SaveData.RaiseCanExecuteChanged();
        }
    }
}
