using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.VisualBasic;

namespace PTGui_Language_Editor
{
    public class StringsViewModel : ViewModelBaseNavi
    {
        const int NumStringsPerPage = 10;

        private List<EditorRefString> allDisplayRefStrings = new List<EditorRefString>();
        private List<OneString> displayStrings;

        public StringsViewModel()
        {
            displayStrings = new List<OneString>();
        }

        public List<EditorRefString> AllRefStrings { get; set; } = null!;
        public List<EditorTransString> AllTransStrings { get; set; } = null!;

        public List<EditorRefString> AllDisplayRefStrings
        {
            get
            {
                return allDisplayRefStrings;
            }
            set
            {
                allDisplayRefStrings = value;
                CurrentPage = 0;
                MaxPages = allDisplayRefStrings.Count / NumStringsPerPage;
                
                if (allDisplayRefStrings.Count % NumStringsPerPage > 0)
                {
                    MaxPages++;
                }

                ShowPage(CurrentPage);
            }
        }

        public List<OneString> DisplayPage
        {
            get
            {
                return displayStrings;
            }

            set
            {
                displayStrings = value;
                NotifyPropertyChanged();
            }
        }

        protected override void ShowPage(int currentPage)
        {
            var list = new List<OneString>();
            int showIndex = currentPage * NumStringsPerPage;

            for (int i = showIndex; i < showIndex + NumStringsPerPage && i < AllDisplayRefStrings.Count(); i++)
            {
                var one = new OneString();
                one.AllRefStrings = AllRefStrings;
                one.AllTransStrings = AllTransStrings;
                one.RefString = AllDisplayRefStrings[i];

                list.Add(one);
            }

            DisplayPage = list;
        }
    }

    public class OneString : ViewModelBase
    {
        private FlowDocument transPreview = null!;
        private string? transEdit;
        private EditorRefString refString = null!;
        private EditorTransString translateString = null!;
        private bool setFromCode;

        public List<EditorRefString> AllRefStrings { get; set; } = null!;
        public List<EditorTransString> AllTransStrings { get; set; } = null!;
        public EditorRefString RefString
        {
            get
            {
                return refString;
            }

            set
            {
                refString = value;
                RefPreview = PTGuiTextConverter.ConvertToFlowDocument(refString.Txt, y => AllRefStrings.FirstOrDefault(x => x.Id == y)?.Txt);
                translateString = refString.EditorTranslate!;

                setFromCode = true;

                if (translateString?.Format == "html")
                {
                    TransEdit = translateString?.Txt?.Replace("<br>", "\n");
                }
                else
                {
                    TransEdit = translateString?.Txt;
                }

                setFromCode = false;
            }
        }

        // Binding properties
        public FlowDocument RefPreview { get; set; } = null!;
        public string Id => "#" + RefString.Id;
        public string? Format => translateString.Format;
        public FlowDocument TransPreview
        {
            get => transPreview;
            set
            {
                transPreview = value;
                NotifyPropertyChanged();
            }
        }
        public string? TransEdit
        {
            get => transEdit;
            set
            {
                transEdit = value;
                TransPreview = PTGuiTextConverter.ConvertToFlowDocument(transEdit, y => AllTransStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                if (translateString?.Format == "html")
                {
                    var str = transEdit.Replace("\n", "<br>");
                    translateString.Txt = str;
                }
                else
                {
                    translateString.Txt = transEdit;
                }

                if (!setFromCode)
                {
                    translateString.Machinetranslated = null;
                }
            }
        }
    }
}

