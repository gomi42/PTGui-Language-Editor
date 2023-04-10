using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PTGui_Language_Editor
{
    //////////////////////////////////////

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public abstract class ViewModelBaseNavi : ViewModelBase
    {
        private DelegateCommand firstPage;
        private DelegateCommand prevPage;
        private DelegateCommand nextPage;
        private DelegateCommand lastPage;
        private DelegateCommand returnPressed;
        private int maxPages;
        private int currentPage;
        private int currentPageDisplay;

        public ViewModelBaseNavi()
        {
            firstPage = new DelegateCommand(OnFirstPage);
            lastPage = new DelegateCommand(OnLastPage);
            nextPage = new DelegateCommand(OnNextPage);
            prevPage = new DelegateCommand(OnPrevPage);
            returnPressed = new DelegateCommand(OnReturnPressed);
        }

        public int MaxPages
        {
            get
            {
                return maxPages;
            }

            set
            {
                maxPages = value;
                NotifyPropertyChanged();
            }
        }

        public int CurrentPageDisplay
        {
            get
            {
                return currentPageDisplay;
            }

            set
            {
                currentPageDisplay = value;
                NotifyPropertyChanged();
            }
        }

        public int CurrentPage
        {
            get
            {
                return currentPage;
            }

            set
            {
                currentPage = value;
                CurrentPageDisplay = value + 1;
                NotifyPropertyChanged();
            }
        }

        public ICommand FirstPage => firstPage;
        public ICommand LastPage => lastPage;
        public ICommand PrevPage => prevPage;
        public ICommand NextPage => nextPage;
        public ICommand ReturnPressed => returnPressed;

        protected abstract void ShowPage(int pageIndex);

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

            CurrentPage = page;
            ShowPage(page);
        }
        
        private void OnFirstPage()
        {
            CurrentPage = 0;
            ShowPage(CurrentPage);
        }

        private void OnLastPage()
        {
            CurrentPage = MaxPages - 1;
            ShowPage(CurrentPage);
        }

        private void OnPrevPage()
        {
            CurrentPage--;

            if (CurrentPage < 0)
            {
                CurrentPage = 0;
            }

            ShowPage(CurrentPage);
        }

        private void OnNextPage()
        {
            CurrentPage++;

            if (CurrentPage >= MaxPages)
            {
                CurrentPage--;
            }

            ShowPage(CurrentPage);
        }
    }
}

