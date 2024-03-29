﻿//
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
    public class TooltipsViewModel : ViewModelBaseNavi
    {
        private List<LanguageString> referenceStrings;
        private List<LanguageString> translationStrings;
        private Action setModified;
        private List<EditTooltip> editTooltips = null!;
        private List<OneTooltip> displayPage = null!;

        public TooltipsViewModel(List<EditTooltip> editTooltips, List<LanguageString> referenceStrings, List<LanguageString> translationStrings, Action setModifiedAction)
        {
            this.referenceStrings = referenceStrings;
            this.translationStrings = translationStrings;
            setModified = setModifiedAction;
            EditTooltips = editTooltips;
        }

        public List<EditTooltip> EditTooltips
        {
            get
            {
                return editTooltips;
            }

            set
            {
                editTooltips = value;
                NumberItems = editTooltips.Count;
            }
        }

        public List<OneTooltip> DisplayPage
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
            var list = new List<OneTooltip>();
            int numItemsPerPage = SelectedItemsPerPage;

            for (int i = showIndex; i < showIndex + numItemsPerPage && i < EditTooltips.Count(); i++)
            {
                var one = new OneTooltip(EditTooltips[i], referenceStrings, translationStrings, setModified);
                list.Add(one);
            }

            DisplayPage = list;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////

    public class OneTooltip : ViewModelBase
    {
        EditTooltip editTooltip;
        private List<LanguageString> referenceStrings = null!;
        private List<LanguageString> translationStrings = null!;
        private Action setModified;
        private bool setFromCode;

        private FlowDocument referenceLabelPreview = null!;
        private FlowDocument referenceHelpTextView = null!;
        private FlowDocument referenceMoreHelpTextView = null!;
        private FlowDocument translationLabelPreview = null!;
        private FlowDocument translationHelpTextPreview = null!;
        private FlowDocument translationMoreHelpTextPreview = null!;
        private string? translationLabelEdit;
        private string? translationHelpTextEdit;
        private string? translationMoreHelpTextEdit;

        public OneTooltip(EditTooltip editTooltip, List<LanguageString> referenceStrings, List<LanguageString> translationStrings, Action setModifiedAction)
        {
            this.editTooltip = editTooltip;
            this.referenceStrings = referenceStrings;
            this.translationStrings = translationStrings;
            setModified = setModifiedAction;

            Init();
        }

        public int Number => editTooltip.Number;

        public string Id => editTooltip.Reference.Id;

        //////////////////////////////////////

        public FlowDocument ReferenceLabelView
        {
            get
            {
                return referenceLabelPreview;
            }

            set
            {
                referenceLabelPreview = value;
                NotifyPropertyChanged();
            }
        }

        public FlowDocument ReferenceHelpTextView
        {
            get
            {
                return referenceHelpTextView;
            }

            set
            {
                referenceHelpTextView = value;
                NotifyPropertyChanged();
            }
        }

        public FlowDocument ReferenceMoreHelpTextView
        {
            get => referenceMoreHelpTextView;
            set
            {
                referenceMoreHelpTextView = value;
                NotifyPropertyChanged();
            }
        }

        //////////////////////////////////////

        public FlowDocument TranslationLabelPreview
        {
            get => translationLabelPreview;
            set
            {
                translationLabelPreview = value;
                NotifyPropertyChanged();
            }
        }

        public FlowDocument TranslationMoreHelpTextPreview
        {
            get => translationMoreHelpTextPreview;
            set
            {
                translationMoreHelpTextPreview = value;
                NotifyPropertyChanged();
            }
        }

        public string? TranslationMoreHelpTextEdit
        {
            get => translationMoreHelpTextEdit;
            set
            {
                translationMoreHelpTextEdit = value;
                TranslationMoreHelpTextPreview = PTGuiTextConverter.ConvertToFlowDocument(translationMoreHelpTextEdit, true, y => translationStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                if (!setFromCode)
                {
                    editTooltip.Translation.MoreHelptext = PTGuiTextConverter.ConvertToHtml(translationMoreHelpTextEdit);
                    editTooltip.Translation.Machinetranslated = null;
                    setModified();
                }

                NotifyPropertyChanged();
            }
        }

        //////////////////////////////////////

        public string? TranslationLabelEdit
        {
            get => translationLabelEdit;
            set
            {
                translationLabelEdit = value;
                TranslationLabelPreview = PTGuiTextConverter.ConvertToFlowDocument(translationLabelEdit, true, y => translationStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                if (!setFromCode)
                {
                    editTooltip.Translation.Label = PTGuiTextConverter.ConvertToHtml(translationLabelEdit);
                    editTooltip.Translation.Machinetranslated = null;
                    setModified();
                }

                NotifyPropertyChanged();
            }
        }

        public FlowDocument TranslationHelpTextPreview
        {
            get => translationHelpTextPreview;
            set
            {
                translationHelpTextPreview = value;
                NotifyPropertyChanged();
            }
        }

        public string? TransHelpTextEdit
        {
            get => translationHelpTextEdit;
            set
            {
                translationHelpTextEdit = value;
                TranslationHelpTextPreview = PTGuiTextConverter.ConvertToFlowDocument(translationHelpTextEdit, true, y => translationStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                if (!setFromCode)
                {
                    editTooltip.Translation.Helptext = PTGuiTextConverter.ConvertToHtml(translationHelpTextEdit);
                    editTooltip.Translation.Machinetranslated = null;
                    setModified();
                }

                NotifyPropertyChanged();
            }
        }

        void Init()
        {
            setFromCode = true;

            NotifyPropertyChanged(nameof(Id));

            ReferenceLabelView = PTGuiTextConverter.ConvertToFlowDocument(editTooltip.Reference.Label, true, y => referenceStrings.FirstOrDefault(x => x.Id == y)?.Txt);
            ReferenceHelpTextView = PTGuiTextConverter.ConvertToFlowDocument(editTooltip.Reference.Helptext, true, y => referenceStrings.FirstOrDefault(x => x.Id == y)?.Txt);
            ReferenceMoreHelpTextView = PTGuiTextConverter.ConvertToFlowDocument(editTooltip.Reference.MoreHelptext, true, y => referenceStrings.FirstOrDefault(x => x.Id == y)?.Txt);

            TranslationLabelEdit = PTGuiTextConverter.ConvertFromHtml(editTooltip.Translation.Label);
            TransHelpTextEdit = PTGuiTextConverter.ConvertFromHtml(editTooltip.Translation.Helptext);
            TranslationMoreHelpTextEdit = PTGuiTextConverter.ConvertFromHtml(editTooltip.Translation.MoreHelptext);

            setFromCode = false;
        }
    }
}
