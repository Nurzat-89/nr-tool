using GUI.Utils;
using GUI.Views.Windows;
using System.Windows.Controls;

namespace GUI.ViewModels
{
    class MainViewModel : BindableBase
    {

        public MainViewModel(PageNavigation aPageNavigation)
        {
            PageNavigation = aPageNavigation;

            PageNavigation.PageChangedEvent += (aModel) => 
            { 
                CurrentPageVm = aModel; 
            };
        }

        #region private fields

        private PageViewModelBase fCurrentPageVm;

        #endregion

        private PageNavigation PageNavigation { get; set; }

        public PageViewModelBase CurrentPageVm
        {
            get => fCurrentPageVm;
            set
            {
                if (Set(ref fCurrentPageVm, value))
                {
                    OnPropertyChanged(nameof(PageView));
                }
            }
        }

        public Control Menu => new MainMenuView();

        public Control PageView => CurrentPageVm?.View;
    }
}
