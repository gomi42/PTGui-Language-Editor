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
        JsonRoot referenceJsonRoot = null!;
        JsonRoot translationJsonRoot = null!;
        Language translationLanguage = null!;
        Language referenceLanguage = null!;
        Editor editor = null!;
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
            referenceLanguage = CreateLanguage(referenceJsonRoot);
            translationLanguage = CreateLanguage(translationJsonRoot);
            CreateEditorData();

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
                referenceJsonRoot = JsonSerializer.Deserialize<JsonRoot>(json, JsonSerializerOptions.Default) ?? null!;
            }

            using (FileStream json = File.OpenRead(GetLanguageFilename(SelectedLanguageFile!)))
            {
                translationJsonRoot = JsonSerializer.Deserialize<JsonRoot>(json, JsonSerializerOptions.Default) ?? null!;
            }
        }

        private Language CreateLanguage(JsonRoot languageJsonRoot)
        {
            var language = new Language();

            language.General = new LanguageGeneral(languageJsonRoot);

            language.Strings = new List<LanguageString>();

            foreach (var jsonItem in languageJsonRoot.strings!)
            {
                var str = new LanguageString(jsonItem);
                language.Strings.Add(str);
            }

            language.HelpPages = new List<LanguageHelpPage>();

            foreach (var jsonItem in languageJsonRoot.helppages!)
            {
                var str = new LanguageHelpPage(jsonItem);
                language.HelpPages.Add(str);
            }

            language.Tooltips = new List<LanguageTooltip>();

            foreach (var jsonItem in languageJsonRoot.tooltips!)
            {
                var str = new LanguageTooltip(jsonItem);
                language.Tooltips.Add(str);
            }

            return language;
        }

        private void CreateEditorData()
        {
            var editorGeneral = new EditorGeneral(referenceLanguage.General, translationLanguage.General);

            ///

            var editorStrings = new List<EditorString>();
            int number = 1;

            foreach (var item in referenceLanguage.Strings)
            {
                var translate = translationLanguage.Strings.FirstOrDefault(x => x.Id == item.Id);

                if (translate == null)
                {
                    var jsonItem = new JsonString 
                    {
                        id = item.Id,
                        format = item.Format,
                        txt = item.Txt
                    };
                    translationJsonRoot.strings!.Add(jsonItem);
                    translate = new LanguageString(jsonItem);
                    translationLanguage.Strings.Add(translate);
                }

                var str = new EditorString(number, item, translate);
                editorStrings.Add(str);
                number++;
            }

            ///

            var editorTooltips = new List<EditorTooltip>();
            number = 1;

            foreach (var item in referenceLanguage.Tooltips)
            {
                var translate = translationLanguage.Tooltips.FirstOrDefault(x => x.Id == item.Id);
                
                if (translate == null)
                {
                    var jsonItem = new JsonTooltip
                    {
                        id = item.Id,
                        label = item.Label,
                        helptext = item.Helptext,
                        morehelptext = item.MoreHelptext
                    };
                    translationJsonRoot.tooltips!.Add(jsonItem);
                    translate = new LanguageTooltip(jsonItem);
                    translationLanguage.Tooltips.Add(translate);
                }

                var str = new EditorTooltip(number, item, translate);
                editorTooltips.Add(str);
                number++;
            }

            ///

            var editorHelpPages = new List<EditorHelpPage>();
            number = 1;

            foreach (var item in referenceLanguage.HelpPages)
            {
                var translate = translationLanguage.HelpPages.FirstOrDefault(x => x.Id == item.Id);
                
                if (translate == null)
                {
                    var jsonItem = new JsonHelpPage
                    {
                        id = item.Id,
                        helptext = item.Helptext
                    };
                    translationJsonRoot.helppages!.Add(jsonItem);
                    translate = new LanguageHelpPage(jsonItem);
                    translationLanguage.HelpPages.Add(translate);
                }

                var str = new EditorHelpPage(number, item, translate);
                editorHelpPages.Add(str);
                number++;
            }

            ///

            editor = new Editor(editorGeneral, editorStrings, editorTooltips, editorHelpPages);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        private void PrepareGeneral()
        {
            var vm = new GeneralViewModel(SetModified);
            vm.EditGeneral = editor.General;
            GeneralViewModel = vm;
        }

        private void PrepareString()
        {
            var vm = new StringsViewModel(referenceLanguage.Strings, translationLanguage.Strings, SetModified);
            vm.EditStrings = editor.Strings;
            StringsViewModel = vm;
        }

        private void PrepareTooltip()
        {
            var vm = new TooltipsViewModel(referenceLanguage.Strings, translationLanguage.Strings, SetModified);
            vm.EditTooltips = editor.Tooltips;
            TooltipsViewModel = vm;
        }

        private void PrepareHelp()
        {
            var vm = new HelpPagesViewModel(referenceLanguage.Strings, translationLanguage.Strings, SetModified);
            vm.EditHelpPages = editor.HelpPages;
            HelpViewModel = vm;
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
            JsonSerializer.Serialize(json, translationJsonRoot, jso);

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
                StringsViewModel.EditStrings = editor.Strings;
                TooltipsViewModel.EditTooltips = editor.Tooltips;
                HelpViewModel.EditHelpPages = editor.HelpPages;
                return;
            }

            //////////////
            var strings = new List<EditorString>();

            foreach (var item in editor.Strings)
            {
                if (item.Reference.Id.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Reference.Txt != null && item.Reference.Txt.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Translation.Txt != null && item.Translation.Txt.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                {
                    strings.Add(item);
                }
            }

            StringsViewModel.EditStrings = strings;

            //////////////
            var tooltips = new List<EditorTooltip>();

            foreach (var item in editor.Tooltips)
            {
                if (item.Reference.Id.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Reference.Label != null && item.Reference.Label.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Reference.Helptext != null && item.Reference.Helptext.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Reference.MoreHelptext != null && item.Reference.MoreHelptext.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Translation.Label != null && item.Translation.Label.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Translation.Helptext != null && item.Translation.Helptext.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Translation.MoreHelptext != null && item.Translation.MoreHelptext.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                {
                    tooltips.Add(item);
                }
            }

            TooltipsViewModel.EditTooltips = tooltips;

            //////////////
            var helppages = new List<EditorHelpPage>();

            foreach (var item in editor.HelpPages)
            {
                if (item.Reference.Id.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Reference.Helptext != null && item.Reference.Helptext.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Translation.Helptext != null && item.Translation.Helptext.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                {
                    helppages.Add(item);
                }
            }

            HelpViewModel.EditHelpPages = helppages;
        }

        private void SetModified()
        {
            isModified = true;
            SaveData.RaiseCanExecuteChanged();
        }
    }
}
