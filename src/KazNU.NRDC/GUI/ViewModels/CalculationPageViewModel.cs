using GUI.Views;
using System.Windows.Controls;

namespace GUI.ViewModels
{
    internal class CalculationPageViewModel : PageViewModelBase
    {
        protected override Control CreateView() => new CalculationPageView();
    }
}
