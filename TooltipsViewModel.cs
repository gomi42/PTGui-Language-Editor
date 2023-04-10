using System;
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
        private List<EditorRefTooltip> allDisplayrefTooltips = null!;
        private EditorTransTooltip translateTooltip = null!;
        private EditorRefTooltip currentRefTooltip = null!;
        private bool setFromCode;

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
                MaxPages = allDisplayrefTooltips.Count;

                ShowPage(CurrentPage);
            }
        }

        // Binding properties
        public string Id => "#" + currentRefTooltip.Id;

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
                }

                NotifyPropertyChanged();
            }
        }

        protected override void ShowPage(int currentPage)
        {
            setFromCode = true;

            currentRefTooltip = allDisplayrefTooltips[currentPage];
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
}
