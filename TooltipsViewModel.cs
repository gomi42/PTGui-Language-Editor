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
    public class TooltipsViewModel : ViewModelBaseNavi
    {
        private Action setModified;
        private List<EditorRefTooltip> allDisplayrefTooltips = null!;
        private List<OneTooltip> displayPage = null!;

        public TooltipsViewModel(Action setModifiedAction)
        {
            setModified = setModifiedAction;
        }

        public List<EditorRefString> AllRefStrings { get; set; } = null!;
        public List<EditorTransString> AllTransStrings { get; set; } = null!;

        public List<EditorRefTooltip> AllDisplayRefTooltips
        {
            get
            {
                return allDisplayrefTooltips;
            }

            set
            {
                allDisplayrefTooltips = value;

                NumberItems = allDisplayrefTooltips.Count;
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

            for (int i = showIndex; i < showIndex + numItemsPerPage && i < AllDisplayRefTooltips.Count(); i++)
            {
                var one = new OneTooltip(setModified);
                one.AllRefStrings = AllRefStrings;
                one.AllTransStrings = AllTransStrings;
                one.RefTooltip = AllDisplayRefTooltips[i];

                list.Add(one);
            }

            DisplayPage = list;
        }
    }

    public class OneTooltip : ViewModelBase
    {
        private Action setModified;
        private EditorTransTooltip translateTooltip = null!;
        private EditorRefTooltip currentRefTooltip = null!;
        private bool setFromCode;

        public OneTooltip(Action setModifiedAction)
        {
            setModified = setModifiedAction;
        }

        public List<EditorRefString> AllRefStrings { get; set; } = null!;
        public List<EditorTransString> AllTransStrings { get; set; } = null!;
        public string Id => "#" + currentRefTooltip.Id;


        public EditorRefTooltip RefTooltip
        {
            get
            {
                return currentRefTooltip;
            }

            set
            {
                setFromCode = true;

                currentRefTooltip = value;
                NotifyPropertyChanged(nameof(Id));
                translateTooltip = currentRefTooltip.EditorTranslate!;

                RefLabelPreview = PTGuiTextConverter.ConvertToFlowDocument(currentRefTooltip.Label, true, y => AllRefStrings.FirstOrDefault(x => x.Id == y)?.Txt);
                TransLabelEdit = translateTooltip?.Label?.Replace("<br>", "\n");

                RefHelpTextPreview = PTGuiTextConverter.ConvertToFlowDocument(currentRefTooltip.Helptext, true, y => AllRefStrings.FirstOrDefault(x => x.Id == y)?.Txt);
                TransHelpTextEdit = translateTooltip?.Helptext?.Replace("<br>", "\n");

                RefMoreHelpTextPreview = PTGuiTextConverter.ConvertToFlowDocument(currentRefTooltip.MoreHelptext, true, y => AllRefStrings.FirstOrDefault(x => x.Id == y)?.Txt);
                TransMoreHelpTextEdit = translateTooltip?.MoreHelptext?.Replace("<br>", "\n");

                setFromCode = false;
            }
        }

        //////////////////////////////////////
        private FlowDocument refLabelPreview = null!;
        private FlowDocument transLabelPreview = null!;
        private string? transLabelEdit;

        public FlowDocument RefLabelPreview
        {
            get
            {
                return refLabelPreview;
            }

            set
            {
                refLabelPreview = value;
                NotifyPropertyChanged();
            }
        }

        public FlowDocument TransLabelPreview
        {
            get => transLabelPreview;
            set
            {
                transLabelPreview = value;
                NotifyPropertyChanged();
            }
        }

        public string? TransLabelEdit
        {
            get => transLabelEdit;
            set
            {
                transLabelEdit = value;
                TransLabelPreview = PTGuiTextConverter.ConvertToFlowDocument(transLabelEdit, true, y => AllTransStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                if (!string.IsNullOrEmpty(transLabelEdit))
                {
                    translateTooltip.Label = transLabelEdit.Replace("\n", "<br>");
                }
                else
                {
                    translateTooltip.Label = null;
                }

                if (!setFromCode)
                {
                    translateTooltip.Machinetranslated = null;
                    setModified();
                }

                NotifyPropertyChanged();
            }
        }

        //////////////////////////////////////
        private FlowDocument refHelpTextPreview = null!;
        private FlowDocument transHelpTextPreview = null!;
        private string? transHelpTextEdit;

        public FlowDocument RefHelpTextPreview
        {
            get
            {
                return refHelpTextPreview;
            }

            set
            {
                refHelpTextPreview = value;
                NotifyPropertyChanged();
            }
        }

        public FlowDocument TransHelpTextPreview
        {
            get => transHelpTextPreview;
            set
            {
                transHelpTextPreview = value;
                NotifyPropertyChanged();
            }
        }

        public string? TransHelpTextEdit
        {
            get => transHelpTextEdit;
            set
            {
                transHelpTextEdit = value;
                TransHelpTextPreview = PTGuiTextConverter.ConvertToFlowDocument(transHelpTextEdit, true, y => AllTransStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                if (!string.IsNullOrEmpty(transHelpTextEdit))
                {
                    translateTooltip.Helptext = transHelpTextEdit.Replace("\n", "<br>");
                }
                else
                {
                    translateTooltip.Helptext = null;
                }

                if (!setFromCode)
                {
                    translateTooltip.Machinetranslated = null;
                    setModified();
                }

                NotifyPropertyChanged();
            }
        }

        //////////////////////////////////////
        private FlowDocument refMoreHelpTextPreview = null!;
        private FlowDocument transMoreHelpTextPreview = null!;
        private string? transMoreHelpTextEdit;

        public FlowDocument RefMoreHelpTextPreview
        {
            get => refMoreHelpTextPreview;
            set
            {
                refMoreHelpTextPreview = value;
                NotifyPropertyChanged();
            }
        }

        public FlowDocument TransMoreHelpTextPreview
        {
            get => transMoreHelpTextPreview;
            set
            {
                transMoreHelpTextPreview = value;
                NotifyPropertyChanged();
            }
        }

        public string? TransMoreHelpTextEdit
        {
            get => transMoreHelpTextEdit;
            set
            {
                transMoreHelpTextEdit = value;
                TransMoreHelpTextPreview = PTGuiTextConverter.ConvertToFlowDocument(transMoreHelpTextEdit, true, y => AllTransStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                if (!string.IsNullOrEmpty(transMoreHelpTextEdit))
                {
                    translateTooltip.MoreHelptext = transMoreHelpTextEdit.Replace("\n", "<br>");
                }
                else
                {
                    translateTooltip.MoreHelptext = null;
                }

                if (!setFromCode)
                {
                    translateTooltip.Machinetranslated = null;
                    setModified();
                }

                NotifyPropertyChanged();
            }
        }

    }
}
