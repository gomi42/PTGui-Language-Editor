//
// Author:
//   Michael Göricke
//
// Copyright (c) 2023
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

        public EditorRef EditorRef { get; set; } = null!;
        public EditorTrans EditorTrans { get; set; } = null!;

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
