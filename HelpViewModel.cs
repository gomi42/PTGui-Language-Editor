using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace PTGui_Language_Editor
{
    public class HelpPagesViewModel : ViewModelBaseNavi
    {
        private List<EditorRefHelpPage> allDisplayRefHelPages = null!;
        private EditorTransHelpPage translateHelpPage = null!;
        private EditorRefHelpPage currentRefHelpPage = null!;
        private bool setFromCode;

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

                CurrentPage = 0;
                MaxPages = allDisplayRefHelPages.Count;

                ShowPage(CurrentPage);
            }
        }

        // Binding properties
        public string Id => "#" + currentRefHelpPage.Id;

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
                TransHelpPagePreview = PTGuiTextConverter.ConvertToFlowDocument(transHelpPageEdit, y => AllTransStrings.FirstOrDefault(x => x.Id == y)?.Txt);

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
                }

                NotifyPropertyChanged();
            }
        }

        protected override void ShowPage(int currentPage)
        {
            setFromCode = true;

            currentRefHelpPage = allDisplayRefHelPages[currentPage];
            NotifyPropertyChanged(nameof(Id));
            translateHelpPage = currentRefHelpPage.EditorTranslate!;

            RefHelpPagePreview = PTGuiTextConverter.ConvertToFlowDocument(currentRefHelpPage.Helptext, y => AllRefStrings.FirstOrDefault(x => x.Id == y)?.Txt);
            TransHelpPageEdit = translateHelpPage?.Helptext?.Replace("<br>", "\n");

            setFromCode = false;
        }
    }
}
