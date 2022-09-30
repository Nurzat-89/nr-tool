using GUI.Utils;

namespace GUI.ViewModels
{
    internal class MainMenuViewModel : BindableBase
    {

        #region private fields

        private bool? fIsCalculationCheckedToggle;
        private bool? fIsSettingsCheckedToggle;
        private Command fGoToCalculationCommand;
        private Command fGoToSettingsCommand;
        #endregion

        public MainMenuViewModel(PageNavigation aPageNavigation)
        {
            PageNavigation = aPageNavigation;
            fIsCalculationCheckedToggle = true;
        }

        #region properties
        
        public PageNavigation PageNavigation { get; }

        /// <summary>
        /// Флаг активности кнопки Scan list
        /// </summary>
        public bool? IsCalculationChecked
        {
            get => fIsCalculationCheckedToggle;
            set
            {
                if (value == true)
                {
                    DisableCheckAllButton();
                    this.Set(ref fIsCalculationCheckedToggle, value);
                }
            }
        }

        /// <summary>
        /// Флаг активности кнопки System Info
        /// </summary>
        public bool? IsSettingsChecked
        {
            get => fIsSettingsCheckedToggle;
            set
            {
                if (value == true)
                {
                    DisableCheckAllButton();
                    this.Set(ref fIsSettingsCheckedToggle, value);
                }
            }
        }

        public Command GoToCalculationCommand => fGoToCalculationCommand ??= new Command(PageNavigation.ShowCalculationPage);

        public Command GoToSettingsCommand => fGoToSettingsCommand ??= new Command(PageNavigation.ShowCalculationPage);

        #endregion

        /// <summary>
        /// Отключаем выделение у всех кнопок 
        /// </summary>
        private void DisableCheckAllButton()
        {
            fIsCalculationCheckedToggle = false;
            fIsSettingsCheckedToggle = false;

            this.OnPropertyChanged(nameof(IsCalculationChecked));
            this.OnPropertyChanged(nameof(IsSettingsChecked));
        }
    }
}
