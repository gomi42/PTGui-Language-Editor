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

                CurrentPage = 0;
                MaxPages = allDisplayRefHelPages.Count;

                ShowPage(CurrentPage);
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

        protected override void ShowPage(int currentPage)
        {
            setFromCode = true;

            if (allDisplayRefHelPages.Count != 0)
            {
                currentRefHelpPage = allDisplayRefHelPages[currentPage];

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
