using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGui_Language_Editor
{
    public class GeneralViewModel : ViewModelBase
    {
        private Action setModified;
        
        public GeneralViewModel(Action setModifiedAction)
        {
            setModified = setModifiedAction;
        }

        public EditorRef EditorRef { get; set; }
        public EditorTrans EditorTrans { get; set; }

        public string RefContributors
        {
            get
            {
                var contributors = EditorRef.Contributors;

                if (contributors == null)
                {
                    return string.Empty;
                }

                return string.Join("\n", contributors);
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    EditorRef.Contributors = null;
                }
                else
                {
                    EditorRef.Contributors = value.Split('\n').ToList();
                }

                NotifyPropertyChanged();
            }
        }

        public string? RefLanguageMameLocalized
        {
            get
            {
                return EditorRef.LanguageMameLocalized;
            }

            set
            {
                EditorRef.LanguageMameLocalized = value;
                NotifyPropertyChanged();
            }
        }

        public string? RefStartupMessage
        {
            get
            {
                return EditorRef.StartupMessage;
            }

            set
            {
                EditorRef.StartupMessage = value;
                NotifyPropertyChanged();
            }
        }

        public string TransContributors
        {
            get
            {
                var contributors = EditorTrans.Contributors;

                if (contributors == null)
                {
                    return string.Empty;
                }

                return string.Join("\n", contributors);
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    EditorTrans.Contributors = null;
                }
                else
                {
                    EditorTrans.Contributors = value.Split('\n').ToList();
                }

                setModified();
            }
        }

        public string? TransLanguageMameLocalized
        {
            get
            {
                return EditorTrans.LanguageMameLocalized;
            }

            set
            {
                EditorTrans.LanguageMameLocalized= value;
                setModified();
            }
        }

        public string? TransStartupMessage
        {
            get
            {
                return EditorTrans.StartupMessage;
            }

            set
            {
                EditorTrans.StartupMessage = value;
                setModified();
            }
        }
    }
}
