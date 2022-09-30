using GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Utils
{
    internal class PageNavigation
    {

        #region methods

        /// <summary>
        /// Вызов события <see cref="PageChangedEvent"/>
        /// </summary>
        /// <param name="aPageViewModel">Модель представления устанавливаемой страницы</param>
        private void OnPageChanged(PageViewModelBase aPageViewModel)
        {
            PageChangedEvent?.Invoke(aPageViewModel);
        }

        /// <summary>
        /// Позволяет перейти на страницу
        /// </summary>
        /// <param name="aPageViewModel">Модель представления страницы, на которую нужно перейти</param>
        internal void ShowPage(PageViewModelBase aPageViewModel)
        {
            // Проверяем, что страница уже не выставлена
            if (Equals(aPageViewModel, CurrentPageViewModel))
                return;

            PreviousPageViewModel = CurrentPageViewModel;
            CurrentPageViewModel = aPageViewModel;

            CurrentPageViewModel.OnShow();
            PreviousPageViewModel?.OnClose();

            OnPageChanged(aPageViewModel);
        }

        public void ShowCalculationPage() 
        {
            ShowPage(AppContext.Instance.CalculationPageViewModel);
            AppContext.Instance.MainMenuViewModel.IsCalculationChecked = true;
        }

        public void ShowSettingsPage()
        {
            ShowPage(AppContext.Instance.SettingsPageViewModel);
            AppContext.Instance.MainMenuViewModel.IsSettingsChecked = true;
        }

        #endregion

        #region events

        /// <summary>
        /// Событие при смене страницы
        /// </summary>
        public event Action<PageViewModelBase> PageChangedEvent;

        #endregion

        #region properties

        /// <summary>
        /// Предыдущая страница
        /// </summary>
        private PageViewModelBase PreviousPageViewModel { get; set; }

        /// <summary>
        /// Текущая страница
        /// </summary>
        internal PageViewModelBase CurrentPageViewModel { get; private set; }

        #endregion
    }
}
