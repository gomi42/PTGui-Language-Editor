﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace PTGui_Language_Editor
{
    public class TooltipsViewModel : ViewModelBaseNavi
    {
        const int NumItemsPerPage = 10;
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

                CurrentPage = 0;
                MaxPages = allDisplayrefTooltips.Count / NumItemsPerPage;

                if (allDisplayrefTooltips.Count % NumItemsPerPage > 0)
                {
                    MaxPages++;
                }

                ShowPage(CurrentPage);
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

        protected override void ShowPage(int currentPage)
        {
            var list = new List<OneTooltip>();
            int showIndex = currentPage * NumItemsPerPage;

            for (int i = showIndex; i < showIndex + NumItemsPerPage && i < AllDisplayRefTooltips.Count(); i++)
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

                RefLabelPreview = PTGuiTextConverter.ConvertToFlowDocument(currentRefTooltip.Label, y => AllRefStrings.FirstOrDefault(x => x.Id == y)?.Txt);
                TransLabelEdit = translateTooltip?.Label?.Replace("<br>", "\n");

                RefHelpTextPreview = PTGuiTextConverter.ConvertToFlowDocument(currentRefTooltip.Helptext, y => AllRefStrings.FirstOrDefault(x => x.Id == y)?.Txt);
                TransHelpTextEdit = translateTooltip?.Helptext?.Replace("<br>", "\n");

                RefMoreHelpTextPreview = PTGuiTextConverter.ConvertToFlowDocument(currentRefTooltip.MoreHelptext, y => AllRefStrings.FirstOrDefault(x => x.Id == y)?.Txt);
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
                TransLabelPreview = PTGuiTextConverter.ConvertToFlowDocument(transLabelEdit, y => AllTransStrings.FirstOrDefault(x => x.Id == y)?.Txt);

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
                TransHelpTextPreview = PTGuiTextConverter.ConvertToFlowDocument(transHelpTextEdit, y => AllTransStrings.FirstOrDefault(x => x.Id == y)?.Txt);

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
                TransMoreHelpTextPreview = PTGuiTextConverter.ConvertToFlowDocument(transMoreHelpTextEdit, y => AllTransStrings.FirstOrDefault(x => x.Id == y)?.Txt);

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