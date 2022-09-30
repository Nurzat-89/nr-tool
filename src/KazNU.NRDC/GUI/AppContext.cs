using GUI.Utils;
using GUI.ViewModels;
using StructureMap;

namespace GUI
{
    internal class AppContext : Container
    {
        #region static fields

        private static AppContext fInstance;

        #endregion

        #region constructor/destructor

        private AppContext()
        {
            Configure(service => 
            {
                service.ForSingletonOf<MainViewModel>().Use<MainViewModel>();
                // Навигация страниц   
                service.ForSingletonOf<PageNavigation>().Use<PageNavigation>();
                // Главное меню
                service.ForSingletonOf<MainMenuViewModel>().Use<MainMenuViewModel>();
                // CalculationPageViewModel
                service.ForSingletonOf<CalculationPageViewModel>().Use<CalculationPageViewModel>();
                // SettingsPageViewModel
                service.ForSingletonOf<SettingsPageViewModel>().Use<SettingsPageViewModel>();
            });
        }

        #endregion

        #region static methods

        /// <summary>
        /// Деинициализация
        /// </summary>
        public static void Done()
        {
            fInstance?.Dispose();
        }

        #endregion

        #region static properties

        public static AppContext Instance => fInstance ??= new AppContext();

        #endregion

        #region properties

        public MainViewModel MainWindowViewModel => Instance.GetInstance<MainViewModel>();

        public MainMenuViewModel MainMenuViewModel => Instance.GetInstance<MainMenuViewModel>();

        public CalculationPageViewModel CalculationPageViewModel => Instance.GetInstance<CalculationPageViewModel>();

        public SettingsPageViewModel SettingsPageViewModel => Instance.GetInstance<SettingsPageViewModel>();

        #endregion
    }
}
