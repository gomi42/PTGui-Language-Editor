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
    public class HelpPagesViewModel : ViewModelBaseNavi
    {
        private List<LanguageString> referenceStrings;
        private List<LanguageString> translationStrings;
        private Action setModified;

        private bool setFromCode;
        private List<EditHelpPage> editHelpPages = null!;
        private EditHelpPage? currentHelpPage;
        
        private FlowDocument refHelpPagePreview = null!;
        private FlowDocument transHelpPagePreview = null!;
        private string? transHelpPageEdit;

        public HelpPagesViewModel(List<EditHelpPage> editHelpPages, List<LanguageString> referenceStrings, List<LanguageString> translationStrings, Action setModifiedAction)
        {
            this.referenceStrings = referenceStrings;
            this.translationStrings = translationStrings;
            setModified = setModifiedAction;
            IsPageSelectionVisible = false;
            EditHelpPages = editHelpPages;
        }

        public List<EditHelpPage> EditHelpPages
        {
            get
            {
                return editHelpPages;
            }

            set
            {
                editHelpPages = value;

                NumberItems = editHelpPages.Count;
                SelectedItemsPerPage = 1;
            }
        }

        // Binding properties
        public string? Number => currentHelpPage?.Number.ToString();

        public string Id => currentHelpPage != null ? currentHelpPage.Reference.Id : string.Empty;

        public FlowDocument ReferenceHelpPagePreview
        {
            get
            {
                return refHelpPagePreview;
            }

            set
            {
                refHelpPagePreview = value;
                NotifyPropertyChanged();
            }
        }

        public FlowDocument TranslationHelpPagePreview
        {
            get => transHelpPagePreview;
            set
            {
                transHelpPagePreview = value;
                NotifyPropertyChanged();
            }
        }

        public string? TranslationHelpPageEdit
        {
            get => transHelpPageEdit;
            set
            {
                transHelpPageEdit = value;

                if (!string.IsNullOrEmpty(transHelpPageEdit))
                {
                    TranslationHelpPagePreview = PTGuiTextConverter.ConvertToFlowDocument(transHelpPageEdit, true, y => translationStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                    if (!setFromCode)
                    {
                        var translateHelpPage = currentHelpPage!.Translation;
                        translateHelpPage.Helptext = PTGuiTextConverter.ConvertToHtml(transHelpPageEdit);

                        translateHelpPage.Machinetranslated = null;
                        setModified();
                    }
                }
                else
                {
                    TranslationHelpPagePreview = new FlowDocument();
                }

                NotifyPropertyChanged();
            }
        }

        protected override void ShowPage(int currentIndex)
        {
            setFromCode = true;

            if (editHelpPages.Count != 0)
            {
                currentHelpPage = editHelpPages[currentIndex];

                ReferenceHelpPagePreview = PTGuiTextConverter.ConvertToFlowDocument(currentHelpPage.Reference.Helptext, true, y => referenceStrings.FirstOrDefault(x => x.Id == y)?.Txt);
                var translateHelpPage = currentHelpPage.Translation;
                TranslationHelpPageEdit = PTGuiTextConverter.ConvertFromHtml(translateHelpPage.Helptext);
            }
            else
            {
                currentHelpPage = null;
                ReferenceHelpPagePreview = new FlowDocument();
                TranslationHelpPageEdit = string.Empty;
            }

            NotifyPropertyChanged(nameof(Number));
            NotifyPropertyChanged(nameof(Id));
            setFromCode = false;
        }
    }
}
