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
using System.Linq;
using System.Windows.Documents;

namespace PTGui_Language_Editor
{
    public class StringsViewModel : ViewModelBaseNavi
    {
        private List<LanguageString> referenceStrings;
        private List<LanguageString> translationStrings;
        private Action setModified;
        private List<EditorString> editStrings = null!;
        private List<OneString> displayPage;

        public StringsViewModel(List<LanguageString> referenceStrings, List<LanguageString> translationStrings, Action setModifiedAction)
        {
            this.referenceStrings = referenceStrings;
            this.translationStrings = translationStrings;
            setModified = setModifiedAction;
            displayPage = new List<OneString>();
        }

        public List<EditorString> EditStrings
        {
            get
            {
                return editStrings;
            }
            set
            {
                editStrings = value;
                NumberItems = editStrings.Count;
            }
        }

        public List<OneString> DisplayPage
        {
            get
            {
                return displayPage;
            }

            set
            {
                displayPage = value;
                NotifyPropertyChanged();
            }
        }

        protected override void ShowPage(int showIndex)
        {
            var list = new List<OneString>();
            int numStringsPerPage = SelectedItemsPerPage;

            for (int i = showIndex; i < showIndex + numStringsPerPage && i < EditStrings.Count(); i++)
            {
                var one = new OneString(EditStrings[i], referenceStrings, translationStrings, setModified);
                list.Add(one);
            }

            DisplayPage = list;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////

    public class OneString : ViewModelBase
    {
        private EditorString editString;
        private List<LanguageString> referenceStrings;
        private List<LanguageString> translationStrings;
        private Action setModified;
        private FlowDocument translationPreview = null!;
        private string? translationEdit;
        private bool setFromCode;

        public OneString(EditorString editString, List<LanguageString> referenceStrings, List<LanguageString> translationStrings, Action setModifiedAction)
        {
            this.editString = editString;
            this.referenceStrings = referenceStrings;
            this.translationStrings = translationStrings;
            setModified = setModifiedAction;

            Init();
        }

        // Binding properties
        public FlowDocument ReferenceView { get; set; } = null!;

        public string Id => "#" + editString.Reference.Id;

        public string? Format => editString.Translation.Format;

        public FlowDocument TranslationPreview
        {
            get => translationPreview;
            set
            {
                translationPreview = value;
                NotifyPropertyChanged();
            }
        }

        public string? TranslationEdit
        {
            get => translationEdit;
            set
            {
                translationEdit = value;
                bool isHtml = editString.Translation.Format == "html";
                TranslationPreview = PTGuiTextConverter.ConvertToFlowDocument(translationEdit, isHtml, y => translationStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                if (!setFromCode)
                {
                    if (isHtml)
                    {
                        var str = translationEdit!.Replace("\n", "<br>");
                        editString.Translation.Txt = str;
                    }
                    else
                    {
                        editString.Translation.Txt = translationEdit;
                    }

                    editString.Translation.Machinetranslated = null;
                    setModified();
                }
            }
        }

        void Init()
        {
            bool isHtml = editString!.Reference.Format == "html";
            ReferenceView = PTGuiTextConverter.ConvertToFlowDocument(editString!.Reference.Txt, isHtml, y => referenceStrings.FirstOrDefault(x => x.Id == y)?.Txt);

            setFromCode = true;

            if (isHtml)
            {
                TranslationEdit = editString.Translation.Txt?.Replace("<br>", "\n");
            }
            else
            {
                TranslationEdit = editString.Translation.Txt;
            }

            setFromCode = false;
        }
    }
}

