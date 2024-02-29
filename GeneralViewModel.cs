//
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
using System.Linq;

namespace PTGui_Language_Editor
{
    public class GeneralViewModel : ViewModelBase
    {
        private Action setModified;
        
        public GeneralViewModel(Action setModifiedAction)
        {
            setModified = setModifiedAction;
        }

        public EditorGeneral EditGeneral { get; set; } = null!;

        public string ReferenceContributors
        {
            get
            {
                var contributors = EditGeneral.Reference.Contributors;

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
                    EditGeneral.Reference.Contributors = null;
                }
                else
                {
                    EditGeneral.Reference.Contributors = value.Split('\n').ToList();
                }

                NotifyPropertyChanged();
            }
        }

        public string? ReferenceLanguageMameLocalized
        {
            get
            {
                return EditGeneral.Reference.LanguageMameLocalized;
            }

            set
            {
                EditGeneral.Reference.LanguageMameLocalized = value;
                NotifyPropertyChanged();
            }
        }

        public string? ReferenceStartupMessage
        {
            get
            {
                return EditGeneral.Reference.StartupMessage;
            }

            set
            {
                EditGeneral.Reference.StartupMessage = value;
                NotifyPropertyChanged();
            }
        }

        public string TranslationContributors
        {
            get
            {
                var contributors = EditGeneral.Translation.Contributors;

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
                    EditGeneral.Translation.Contributors = null;
                }
                else
                {
                    EditGeneral.Translation.Contributors = value.Split('\n').ToList();
                }

                setModified();
            }
        }

        public string? TranslationLanguageMameLocalized
        {
            get
            {
                return EditGeneral.Translation.LanguageMameLocalized;
            }

            set
            {
                EditGeneral.Translation.LanguageMameLocalized= value;
                setModified();
            }
        }

        public string? TranslationStartupMessage
        {
            get
            {
                return EditGeneral.Translation.StartupMessage;
            }

            set
            {
                EditGeneral.Translation.StartupMessage = value;
                setModified();
            }
        }
    }
}
