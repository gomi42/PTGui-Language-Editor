//
// Author:
//   Michael Göricke
//
// Copyright (c) 2023
//
// This file is part of PTGui Language Editor.
//
// ShapeConverter is free software: you can redistribute it and/or modify
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
        private Action setModified;
        private List<EditorRefHelpPage> allDisplayRefHelPages = null!;
        private EditorRefHelpPage? currentRefHelpPage;
        private bool setFromCode;

        public HelpPagesViewModel(Action setModifiedAction)
        {
            setModified = setModifiedAction;
            IsPageSelectionVisible = false;
        }

        public List<EditorRefString> AllRefStrings { get; set; } = null!;
        public List<EditorTransString> AllTransStrings { get; set; } = null!;

        public List<EditorRefHelpPage> AllDisplayRefHelpPages
        {
            get
            {
                return allDisplayRefHelPages;
            }

            set
            {
                allDisplayRefHelPages = value;

                NumberItems = allDisplayRefHelPages.Count;
                SelectedItemsPerPage = 1;
            }
        }

        // Binding properties
        public string Id => currentRefHelpPage != null ? "#" + currentRefHelpPage.Id : string.Empty;

        //////////////////////////////////////
        private FlowDocument refHelpPagePreview = null!;
        private FlowDocument transHelpPagePreview = null!;
        private string? transHelpPageEdit;

        public FlowDocument RefHelpPagePreview
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

        public FlowDocument TransHelpPagePreview
        {
            get => transHelpPagePreview;
            set
            {
                transHelpPagePreview = value;
                NotifyPropertyChanged();
            }
        }

        public string? TransHelpPageEdit
        {
            get => transHelpPageEdit;
            set
            {
                transHelpPageEdit = value;

                if (!string.IsNullOrEmpty(transHelpPageEdit))
                {
                    TransHelpPagePreview = PTGuiTextConverter.ConvertToFlowDocument(transHelpPageEdit, true, y => AllTransStrings.FirstOrDefault(x => x.Id == y)?.Txt);
                    var translateHelpPage = currentRefHelpPage?.EditorTranslate;

                    if (!string.IsNullOrEmpty(transHelpPageEdit))
                    {
                        translateHelpPage.Helptext = transHelpPageEdit.Replace("\n", "<br>");
                    }
                    else
                    {
                        translateHelpPage.Helptext = null;
                    }

                    if (!setFromCode)
                    {
                        translateHelpPage.Machinetranslated = null;
                        setModified();
                    }
                }
                    else
                    {
                        TransHelpPagePreview = new FlowDocument();
                    }

                NotifyPropertyChanged();
            }
        }

        protected override void ShowPage(int currentIndex)
        {
            setFromCode = true;

            if (allDisplayRefHelPages.Count != 0)
            {
                currentRefHelpPage = allDisplayRefHelPages[currentIndex];

                RefHelpPagePreview = PTGuiTextConverter.ConvertToFlowDocument(currentRefHelpPage.Helptext, true, y => AllRefStrings.FirstOrDefault(x => x.Id == y)?.Txt);
                var translateHelpPage = currentRefHelpPage.EditorTranslate;
                TransHelpPageEdit = translateHelpPage?.Helptext?.Replace("<br>", "\n");
            }
            else
            {
                currentRefHelpPage = null;
                RefHelpPagePreview = new FlowDocument();
                TransHelpPageEdit = string.Empty;
            }

            NotifyPropertyChanged(nameof(Id));
            setFromCode = false;
        }
    }
}
