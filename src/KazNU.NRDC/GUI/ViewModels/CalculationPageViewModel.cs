using GUI.Utils;
using GUI.Views;
using NuclearCalculation;
using NuclearData;
using StructureMap.Pipeline;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GUI.ViewModels
{
    internal class CalculationPageViewModel : PageViewModelBase, IProcessProgressViewModel
    {
        private Constants.DATALIBS _selectedEndfLibrary;
        private Constants.MACSDATALIBS _selectedMacsLibrary;
        private int _selectedTemperature;
        private string _selectedLowerElement;
        private string _selectedUpperElement;
        private string _neutronFluxText;      
        private string _isotopeRange;
        private double _neutronFlux;
        private TimeScales _selectedTimeScale;
        private int _halfLifeLowerLimit;
        private TimeSpan _halfLifeLowerLimitTimeSpan;
        private double _percent;
        private string _statusText;
        private string _errorMessage;
        private string _informationMessage;
        private bool _isIsotopeRange;
        private bool _isBurnupReady;

        private IEndf _currentEndf;
        private IMacsEndf _currentMacsEndf;
        private INeutronSpectra _currentNeutronSpectra;
        private IBurnUp _burnUp;
        private IEnumerable<IIsotope> _isotopes;
        private bool _isBuildInProgress;


        public CalculationPageViewModel()
        {
            InitDataLibraries();
            MacsLibraryList = Enum.GetValues(typeof(Constants.MACSDATALIBS)).Cast<Constants.MACSDATALIBS>();
            TimeScales = Enum.GetValues(typeof(TimeScales)).Cast<TimeScales>();
            ElementList = Constants.ElementNames;
            _selectedLowerElement = "Pb";
            _selectedUpperElement = "Po";

            _selectedEndfLibrary = EndfLibraryList.FirstOrDefault();
            _selectedMacsLibrary = Constants.MACSDATALIBS.ENDF_B;
            TemperatureList = new List<int>() { 5, 10, 15, 20, 25, 30, 35, 40, 50, 60 };
            _selectedTemperature = 30;
            NeutronFluxText = "1E16";
            _selectedTimeScale = ViewModels.TimeScales.Microsecond;
            _halfLifeLowerLimit = 0;
            _isIsotopeRange = true;
            _isotopeRange = "81206, 81207, 82204, 82205, 82206, 82207, 82208, 82209, 82210, 82211, 83209, 83210, 83211, 84210, 84211";

            BuildCommand = new Command(OnBuild);
            GoToNextCommand = new Command(OnGoToNext);
        }

        private void InitDataLibraries()
        {
            List<Constants.DATALIBS> endfLibraryList = new List<Constants.DATALIBS>();
            var localAppdatafolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dataFolder = $"{localAppdatafolder}\\KazNRDC\\xsdir";

            if (!Directory.Exists(dataFolder))
            {
                MessageBox.Show("Nuclear data library does not exist. Install nr-tool-data first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            foreach (var folder in Directory.GetDirectories(dataFolder))
            {
                string folderName = (new DirectoryInfo(folder)).Name;
                switch (folderName)
                {
                    case "ENDFB-VIII":
                        endfLibraryList.Add(Constants.DATALIBS.ENDFB_VIII);
                        break;
                    case "JENDL":
                        endfLibraryList.Add(Constants.DATALIBS.JENDL);
                        break;
                    case "TENDL":
                        endfLibraryList.Add(Constants.DATALIBS.TENDL);
                        break;
                    case "JEFF":
                        endfLibraryList.Add(Constants.DATALIBS.JEFF);
                        break;
                }
            }
            if (!endfLibraryList.Any())
            {
                MessageBox.Show("Nuclear data library does not exist. Install nr-tool-data first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            EndfLibraryList = endfLibraryList;
        }

        private void OnGoToNext() 
        {
            AppContext.Instance.GetInstance<PageNavigation>().ShowSettingsPage();
        }

        private void OnBuild() 
        {
            ErrorMessage = "";
            InformationMessage = "";
            IsBuildInProgress = true;
            IsBurnupReady = false;

            _currentEndf = _selectedEndfLibrary switch
            {
                Constants.DATALIBS.ENDFB_VIII => new EndfB(),
                Constants.DATALIBS.JEFF => new Jeff(),
                Constants.DATALIBS.JENDL => new Jendl(),
                Constants.DATALIBS.TENDL => new Tendl(),
                _ => throw new ArgumentOutOfRangeException("No ENDF data library for this item")
            };
            _currentEndf.DataReaderStatusEvent += OnEndfDataReaderChanged;
            _currentMacsEndf = _selectedMacsLibrary switch
            {
                Constants.MACSDATALIBS.ENDF_B => new EndfBMacs(),
                Constants.MACSDATALIBS.EAF2010 => new EafMacs(),
                _ => throw new ArgumentOutOfRangeException("No MACS data library for this item")
            };

            _currentNeutronSpectra = new NeutronSpectra(_selectedTemperature * 1000, _neutronFlux);

            Task.Factory.StartNew(() => 
            {
                if (_isIsotopeRange)
                {
                    if (!ValidateIsotopeRange())
                    {
                        return;
                    }
                    _isotopes = _currentEndf.GetIsotopes(_selectedLowerElement, _selectedUpperElement);
                }
                else
                {
                    List<int> zaids = new List<int>();
                    if (!ValidateIndividualIsotopes(_isotopeRange, out string[] tmpIsotopes))
                    {
                        return;
                    }
                    foreach (var isotope in tmpIsotopes)
                    {
                        if (!int.TryParse(isotope, out int zaid))
                        {
                            ErrorMessage = $"Ошибка: неправильно было указано zaid у элемента {zaid}";
                            IsBuildInProgress = false;
                            return;
                        }
                        if (!EndfHelper.CheckZaid(zaid))
                        {
                            ErrorMessage = $"Ошибка: неправильно было указано z чмло у элемента {zaid}";
                            IsBuildInProgress = false;
                            return;
                        }
                        zaids.Add(zaid);
                    }

                    _isotopes = _currentEndf.GetIsotopes(zaids.ToArray());

                }

                if (HalfLifeLowerLimit > 0)
                {
                    _isotopes = _isotopes.Where(x => x.HalfLife >= _halfLifeLowerLimitTimeSpan.TotalSeconds);
                }
                NuclearData.NeutronSpectra.SetMacsCrossSection(_isotopes, _currentMacsEndf.GetMacsData(), _currentNeutronSpectra);
                _burnUp = new BurnUp(_isotopes, _currentNeutronSpectra);
                _burnUp.BurnupMatrixStatusChangedEvent += OnBurnupMatrixStatusChanged;
                InformationMessage = string.Format("Success: {0} isotopes were successfully loaded from the database '{1}'. {2} of them are stable", _isotopes.Count(), _selectedEndfLibrary, _isotopes.Count(x => x.Stable));
                IsBuildInProgress = false;
                IsBurnupReady = true;
                OnPropertyChanged(nameof(Isotopes));
            });
        }

        private void OnBurnupMatrixStatusChanged(int progress)
        {
            Percent = progress;
            StatusText = "Setting burnup matrix...";
        }

        private bool ValidateIsotopeRange() 
        {
            if (!Constants.ElementNames.Contains(_selectedLowerElement) || !Constants.ElementNames.Contains(_selectedUpperElement))
            {
                ErrorMessage = $"Ошибка: Непраильное имя элемента {_selectedLowerElement}-{_selectedUpperElement}";
                IsBuildInProgress = false;
                return false;
            }

            if (Array.IndexOf(Constants.ElementNames, _selectedLowerElement) >= Array.IndexOf(Constants.ElementNames, _selectedUpperElement))
            {
                ErrorMessage = $"Ошибка: Первый элемент должен быть ниже второго";
                IsBuildInProgress = false;
                return false;
            }
            return true;
        }

        private bool ValidateIndividualIsotopes(string isotopeRange, out string[] tmpIsotopes) 
        {
            tmpIsotopes = null;
            if (string.IsNullOrEmpty(isotopeRange))
            {
                ErrorMessage = $"Ошибка: Пустое поле в диапазоне элементов";
                IsBuildInProgress = false;
                return false;
            }

            if (isotopeRange.Contains(";"))
            {
                tmpIsotopes = isotopeRange.Split(";");
            }
            else if (isotopeRange.Contains(","))
            {
                tmpIsotopes = isotopeRange.Split(",");
            }
            else if (isotopeRange.Contains(" "))
            {
                tmpIsotopes = isotopeRange.Split(" ");
            }
            else
            {
                ErrorMessage = $"Ошибка: Нет разделителя между изотопами, поставьте (, ; -) знаки";
                IsBuildInProgress = false;
                return false;
            }

            if (tmpIsotopes == null || !tmpIsotopes.Any())
            {
                ErrorMessage = $"Ошибка: Пустой списко изотопов";
                IsBuildInProgress = false;
                return false;
            }
            return true;
        }
        
        private void OnEndfDataReaderChanged(int progress, string text)
        {
            Percent = progress;
            StatusText = text;
        }

        protected override Control CreateView() => new CalculationPageView();

        /// <summary>
        /// NeutronSpectra
        /// </summary>
        public INeutronSpectra NeutronSpectra => _currentNeutronSpectra;

        /// <summary>
        /// Burn up
        /// </summary>
        public IBurnUp BurnUp => _burnUp;

        /// <summary>
        /// CurrentMacsEndf
        /// </summary>
        public IMacsEndf CurrentMacsEndf => _currentMacsEndf;

        /// <summary>
        /// Isotopes
        /// </summary>
        public IEnumerable<IIsotope> Isotopes => _isotopes;

        /// <summary>
        /// EndfLibraryList
        /// </summary>
		public IEnumerable<Constants.DATALIBS> EndfLibraryList { get; set; }

        /// <summary>
        /// SelectedEndfLibrary
        /// </summary>
		public Constants.DATALIBS SelectedEndfLibrary
        {
            get => _selectedEndfLibrary;
            set => Set(ref _selectedEndfLibrary, value);
        }

        /// <summary>
        /// MacsLibraryList
        /// </summary>
        public IEnumerable<Constants.MACSDATALIBS> MacsLibraryList { get; set; }

        /// <summary>
        /// SelectedMacsLibrary
        /// </summary>
		public Constants.MACSDATALIBS SelectedMacsLibrary
        {
            get => _selectedMacsLibrary;
            set => Set(ref _selectedMacsLibrary, value);
        }

        /// <summary>
        /// SelectedTemperature
        /// </summary>
        public int SelectedTemperature
        {
            get => _selectedTemperature;
            set => Set(ref _selectedTemperature, value);
        }

        /// <summary>
        /// SelectedTemperature
        /// </summary>
        public string NeutronFluxText
        {
            get => _neutronFluxText;
            set
            {
                if (Set(ref _neutronFluxText, value))
                {
                    double.TryParse(value, out _neutronFlux);
                }
            }
        }

        /// <summary>
        /// TemperatureList
        /// </summary>
        public IEnumerable<int> TemperatureList { get; set; }

        /// <summary>
        /// ElementList
        /// </summary>
        public IEnumerable<string> ElementList { get; set; }

        /// <summary>
        /// SelectedUpperElement
        /// </summary>
        public string SelectedUpperElement
        {
            get => _selectedUpperElement;
            set => Set(ref _selectedUpperElement, value);
        }

        /// <summary>
        /// InformationMessage
        /// </summary>
        public string InformationMessage
        {
            get => _informationMessage;
            set => Set(ref _informationMessage, value);
        }

        /// <summary>
        /// SelectedLowerElement
        /// </summary>
        public string SelectedLowerElement
        {
            get => _selectedLowerElement;
            set => Set(ref _selectedLowerElement, value);
        }

        /// <summary>
        /// IsotopeRange
        /// </summary>
        public string IsotopeRange
        {
            get => _isotopeRange;
            set => Set(ref _isotopeRange, value);
        }

        /// <summary>
        /// NeutronFlux
        /// </summary>
        public double NeutronFlux
        {
            get => _neutronFlux;
            set => Set(ref _neutronFlux, value);
        }

        /// <summary>
        /// TimeScales
        /// </summary>
        public IEnumerable<TimeScales> TimeScales { get; set; }

        /// <summary>
        /// SelectedTimeScale
        /// </summary>
        public TimeScales SelectedTimeScale
        {
            get => _selectedTimeScale;
            set 
            {
                if (Set(ref _selectedTimeScale, value))
                {
                    switch (_selectedTimeScale)
                    {
                        case ViewModels.TimeScales.Microsecond:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromMilliseconds(_halfLifeLowerLimit / 1000.0);
                            break;
                        case ViewModels.TimeScales.Millisecond:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromMilliseconds(_halfLifeLowerLimit);
                            break;
                        case ViewModels.TimeScales.Second:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromSeconds(_halfLifeLowerLimit);
                            break;
                        case ViewModels.TimeScales.Minute:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromMinutes(_halfLifeLowerLimit);
                            break;
                        case ViewModels.TimeScales.Hour:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromHours(_halfLifeLowerLimit);
                            break;
                        case ViewModels.TimeScales.Day:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromDays(_halfLifeLowerLimit);
                            break;
                        case ViewModels.TimeScales.Year:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromDays(365 * _halfLifeLowerLimit);
                            break;
                        default:
                            break;
                    }
                } 
            }
        }

        /// <summary>
        /// HalfLifeLowerLimit
        /// </summary>
        public int HalfLifeLowerLimit
        {
            get => _halfLifeLowerLimit;
            set
            {
                if (Set(ref _halfLifeLowerLimit, value))
                {
                    switch (SelectedTimeScale)
                    {
                        case ViewModels.TimeScales.Microsecond:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromMilliseconds(value / 1000.0);
                            break;
                        case ViewModels.TimeScales.Millisecond:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromMilliseconds(value);
                            break;
                        case ViewModels.TimeScales.Second:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromSeconds(value);
                            break;
                        case ViewModels.TimeScales.Minute:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromMinutes(value);
                            break;
                        case ViewModels.TimeScales.Hour:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromHours(value);
                            break;
                        case ViewModels.TimeScales.Day:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromDays(value);
                            break;
                        case ViewModels.TimeScales.Year:
                            _halfLifeLowerLimitTimeSpan = TimeSpan.FromDays(365 * value);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// IsIsotopeRange
        /// </summary>
        public bool IsIsotopeRange
        {
            get => _isIsotopeRange;
            set => Set(ref _isIsotopeRange, value);
        }

        /// <summary>
        /// ErrorMessage
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value);
        }

        /// <summary>
        /// IsBuildInProgress
        /// </summary>
        public bool IsBuildInProgress
        {
            get => _isBuildInProgress;
            set 
            { 
                Set(ref _isBuildInProgress, value);
                OnPropertyChanged(nameof(IsBuildReady));
            }
        }

        /// <summary>
        /// IsBurnupReady
        /// </summary>
        public bool IsBurnupReady
        {
            get => _isBurnupReady;
            set => Set(ref _isBurnupReady, value);
        }

        /// <summary>
        /// IsBuildReady
        /// </summary>
        public bool IsBuildReady => !_isBuildInProgress;

        public double Percent
        {
            get => _percent;
            set
            {
                Set(ref _percent, value);
                OnPropertyChanged(nameof(PercentText));
            }
        }

        public string PercentText => $"{Percent}%";

        public Command BuildCommand { get; }

        public Command CancelCommand => throw new NotImplementedException();

        public Command GoToNextCommand { get; }

        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);            
        }
    }

    internal enum TimeScales
    {
        Microsecond,
        Millisecond,
        Second,
        Minute,
        Hour,
        Day,
        Year
    }
}
