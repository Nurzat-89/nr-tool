using GUI.Utils;
using GUI.Views;
using NuclearCalculation;
using NuclearData;
using System.Collections.Generic;
using System.Windows.Controls;

namespace GUI.ViewModels
{
    internal class SettingsPageViewModel : PageViewModelBase
    {
        private CalculationPageViewModel _calculationPageViewModel;
        private IBurnUpProcess _burnUpProcess;
        public List<INuclideDensity> _initalDensities;

        public SettingsPageViewModel(CalculationPageViewModel calculationPageViewModel)
        {
            _calculationPageViewModel = calculationPageViewModel;
            _initalDensities = new List<INuclideDensity>();

            foreach (var isotope in _calculationPageViewModel.Isotopes)
            {
                _initalDensities.Add(new NuclideDensity(isotope, 0));
            }
            OnPropertyChanged(nameof(InitalDensities));

            GoToBackCommand = new Command(OnGoToBack);
        }

        private void OnGoToBack() 
        {
            AppContext.Instance.GetInstance<PageNavigation>().ShowCalculationPage();
        }

        public IEnumerable<IIsotope> Isotopes => _calculationPageViewModel.Isotopes;

        protected override Control CreateView() => new SettingsPageView();

        public IEnumerable<INuclideDensity> InitalDensities => _initalDensities;

        public Command GoToBackCommand { get; }
    }
}
