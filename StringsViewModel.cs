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
    public class StringsViewModel : ViewModelBaseNavi
    {
        private Action setModified;
        private List<EditorRefString> allDisplayRefStrings = new List<EditorRefString>();
        private List<OneString> displayPage;

        public StringsViewModel(Action setModifiedAction)
        {
            setModified = setModifiedAction;
            displayPage = new List<OneString>();
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
                NumberItems = allDisplayRefStrings.Count;
            }
        }

        public List<OneString> DisplayPage
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
            var list = new List<OneString>();
            int numStringsPerPage = SelectedItemsPerPage;

            for (int i = showIndex; i < showIndex + numStringsPerPage && i < AllDisplayRefStrings.Count(); i++)
            {
                var one = new OneString(setModified);
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
        private Action setModified;
        private FlowDocument transPreview = null!;
        private string? transEdit;
        private EditorRefString refString = null!;
        private EditorTransString translateString = null!;
        private bool setFromCode;

        public OneString(Action setModifiedAction)
        {
            setModified = setModifiedAction;
        }

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
                bool isHtml = translateString?.Format == "html";
                RefPreview = PTGuiTextConverter.ConvertToFlowDocument(refString.Txt, isHtml, y => AllRefStrings.FirstOrDefault(x => x.Id == y)?.Txt);
                translateString = refString.EditorTranslate!;

                setFromCode = true;

                if (isHtml)
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
                bool isHtml = translateString?.Format == "html";
                TransPreview = PTGuiTextConverter.ConvertToFlowDocument(transEdit, isHtml, y => AllTransStrings.FirstOrDefault(x => x.Id == y)?.Txt);

                if (isHtml)
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
                    setModified();
                }
            }
        }
    }
}

