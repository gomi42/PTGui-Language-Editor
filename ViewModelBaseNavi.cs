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

using System.Collections.Generic;
using System.Windows.Input;

namespace PTGui_Language_Editor
{
    public abstract class ViewModelBaseNavi : ViewModelBase
    {
        private int maxPages;
        private int currentPage;
        private int currentPageDisplay;
        private int numberItems;
        private int selectedItemsPerPage;
        private bool isPageSelectionVisible;

        public ViewModelBaseNavi()
        {
            FirstPage = new DelegateCommand(OnFirstPage);
            LastPage = new DelegateCommand(OnLastPage);
            NextPage = new DelegateCommand(OnNextPage);
            PrevPage = new DelegateCommand(OnPrevPage);
            ReturnPressed = new DelegateCommand(OnReturnPressed);

            ItemsPerPageSelection = new List<int> { 1, 2, 5, 10, 20 };
            selectedItemsPerPage = 10;
            isPageSelectionVisible = true;
        }

        public List<int> ItemsPerPageSelection { get; set; }

        public int SelectedItemsPerPage
        {
            get => selectedItemsPerPage;
            set
            {
                var currentFirstItemOfPage = CurrentFirstItemOfPage;

                selectedItemsPerPage = value;
                NotifyPropertyChanged();

                CalcPages();
                ShowNewPage(currentFirstItemOfPage / selectedItemsPerPage);
            }
        }

        public int NumberItems
        {
            get => numberItems;
            set
            {
                numberItems = value;
                CalcPages();
                ShowNewPage(0);
            }
        }

        public int CurrentFirstItemOfPage => currentPage * selectedItemsPerPage;

        public int MaxPages
        {
            get => maxPages;
            set
            {
                maxPages = value;
                NotifyPropertyChanged();
            }
        }

        public int CurrentPageDisplay
        {
            get => currentPageDisplay;
            set
            {
                currentPageDisplay = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand FirstPage { get; init; }
        public ICommand LastPage { get; init; }
        public ICommand PrevPage { get; init; }
        public ICommand NextPage { get; init; }
        public ICommand ReturnPressed { get; init; }

        public bool IsPageSelectionVisible
        {
            get => isPageSelectionVisible;
            set
            {
                isPageSelectionVisible = value;
                NotifyPropertyChanged();
            }
        }

        protected abstract void ShowPage(int itemIndex);

        private void CalcPages()
        {
            MaxPages = numberItems / selectedItemsPerPage;

            if (numberItems % selectedItemsPerPage > 0)
            {
                MaxPages++;
            }
        }

        private void ShowNewPage(int page)
        {
            currentPage = page;
            CurrentPageDisplay = page + 1;
            ShowPage(page * selectedItemsPerPage);
        }

        private void OnReturnPressed()
        {
            var page = CurrentPageDisplay - 1;

            if (page < 0)
            {
                page = 0;
            }
            else
            if (page >= MaxPages)
            {
                page = MaxPages - 1;
            }

            ShowNewPage(page);
        }

        private void OnFirstPage()
        {
            ShowNewPage(0);
        }

        private void OnLastPage()
        {
            ShowNewPage(MaxPages - 1);
        }

        private void OnPrevPage()
        {
            var page = currentPage - 1;

            if (page < 0)
            {
                page = 0;
            }

            ShowNewPage(page);
        }

        private void OnNextPage()
        {
            var page = currentPage + 1;

            if (page >= MaxPages)
            {
                page = MaxPages - 1;
            }

            ShowNewPage(page);
        }
    }
}

