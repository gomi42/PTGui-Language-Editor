//
// Author:
//   Michael Göricke
//
// Copyright (c) 2026
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
        private readonly Action setModified;

        private bool setFromCode;
        private EditHelpPage? currentHelpPage;
        
        public HelpPagesViewModel(List<EditHelpPage> editHelpPages,
                                  List<LanguageString> referenceStrings,
                                  List<LanguageString> translationStrings,
                                  Action setModifiedAction)
        {
            TranslationHelpPagePreview = null!;
            ReferenceHelpPagePreview = null!;
            this.referenceStrings = referenceStrings;
            this.translationStrings = translationStrings;
            setModified = setModifiedAction;
            IsPageSelectionVisible = false;
            EditHelpPages = editHelpPages;
            SelectedItemsPerPage = 1;
        }

        public List<EditHelpPage> EditHelpPages
        {
            get => field;

            set
            {
                field = value;

                NumberItems = field.Count;
            }
        }

        // Binding properties
        public string? Number => currentHelpPage?.Number.ToString();

        public string Id => currentHelpPage != null ? currentHelpPage.Reference.Id : string.Empty;

        public FlowDocument ReferenceHelpPagePreview
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public FlowDocument TranslationHelpPagePreview
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public string? TranslationHelpPageEdit
        {
            get => field;
            set
            {
                field = value;

                if (!string.IsNullOrEmpty(field))
                {
                    TranslationHelpPagePreview = PTGuiTextConverter.ConvertToFlowDocument(field, true, y => translationStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                    if (!setFromCode)
                    {
                        var translateHelpPage = currentHelpPage!.Translation;
                        translateHelpPage.Helptext = PTGuiTextConverter.ConvertToHtml(field);

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

            if (EditHelpPages.Count != 0)
            {
                currentHelpPage = EditHelpPages[currentIndex];

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
