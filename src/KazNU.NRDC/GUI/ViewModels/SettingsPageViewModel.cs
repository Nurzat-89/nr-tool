using GUI.Utils;
using GUI.Views;
using LiveCharts.Defaults;
using LiveCharts;
using NuclearCalculation;
using NuclearCalculation.Matrix;
using NuclearData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using System.Security.AccessControl;
using System.Reflection.Emit;
using Microsoft.Win32;
using System.IO;
using StructureMap.Pipeline;

namespace GUI.ViewModels
{
    internal class SettingsPageViewModel : PageViewModelBase, IProcessProgressViewModel
    {
        private CalculationPageViewModel _calculationPageViewModel;
        private IBurnUpProcess _burnUpProcess;
        private bool _isBarChartSelected;
        private bool _isCalculationReady;
        private bool _isCalculated;
        private TimeScales _selectedTimeScale;
        private TimeSpan _calculationTimeSpan;
        private double _timeCalculation;
        private double _totalCalculationSec;
        private double _percent;
        private string _statusText;
        private MatrixExpType _selectedMatrixExp;
        private IMatrixExp _currentMatrixExp;
        private int _fluxIterations;
        private string _initialFluxText;
        private double _initialNeutronFlux;
        private List<TimeDensties> _timeMeshDensities;

        private object _progressLock = new object();
        private static object _syncLock = new object();

        public SettingsPageViewModel(CalculationPageViewModel calculationPageViewModel)
        {
            _calculationPageViewModel = calculationPageViewModel;
            InitalDensities = new ObservableCollection<INuclideDensity>();
            Densities = new ObservableCollection<INuclideDensity>();
            FluxHeatDensities = new ObservableCollection<FluxHeatDensity>();
            _timeMeshDensities = new List<TimeDensties>();

            BindingOperations.EnableCollectionSynchronization(InitalDensities, _syncLock);
            BindingOperations.EnableCollectionSynchronization(Densities, _syncLock);
            BindingOperations.EnableCollectionSynchronization(FluxHeatDensities, _syncLock);

            var mapper = Mappers.Xy<ObservablePoint>()
               .X(point => point.X)
               .Y(point => Math.Log(point.Y, 10));

            SeriesCollection = new SeriesCollection(mapper)
            {
                new ColumnSeries
                {
                    Title = "Concentration",
                    Values = new ChartValues<double>()
                }
            };
            LineCollection = new SeriesCollection(mapper)
            {
                new LineSeries
                {
                    Title = "Concentration",
                    Values = new ChartValues<double>()
                }
            };
            TimeMeshLineCollection = new SeriesCollection(mapper);

            Formatter = value => value.ToString("N");
            FormatterY = value => Math.Pow(10, value).ToString("N0");

            TimeScales = Enum.GetValues(typeof(TimeScales)).Cast<TimeScales>();
            _selectedMatrixExp = MatrixExpType.CRAM;
            _selectedTimeScale = ViewModels.TimeScales.Year;

            MatrixExpList = Enum.GetValues(typeof(MatrixExpType)).Cast<MatrixExpType>();
            _isCalculationReady = true;

            InitialFluxText = "1E14";
            FluxIterations = 10;

            GoToBackCommand = new Command(OnGoToBack);
            ExportRawDataCommand = new Command(OnExportRawData);
            ExportGroupdDataCommand = new Command(OnExportRawDataLight);
            CalculateCommand = new Command(OnCalculate);
            FluxCalculateCommand = new Command(OnFluxCalculate);
            TimeMeshCalculateCommand = new Command(OnTimeMeshCalculate);
        }

        private void OnTimeMeshCalculate() 
        {
            if (InitalDensities.All(x => x.Density == 0.0))
            {
                MessageBox.Show("Задайде начальную концентрацю", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_fluxIterations == 0)
            {
                MessageBox.Show("Не указана время облучения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_initialNeutronFlux == 0)
            {
                MessageBox.Show("Не указана начальный поток", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Task.Factory.StartNew(() =>
            {
                IsCalculationReady = false;
                IsCalculated = false;
            });

            if (_currentMatrixExp != null)
            {
                _currentMatrixExp.CalculationStatusChangedEvent -= OnCalculationStatusChanged;
                _currentMatrixExp.CalculationStatusChangedEvent -= OnCalculationFluxStatusChanged;
            }

            _currentMatrixExp = SelectedMatrixExp switch
            {
                MatrixExpType.CRAM => new CRAM(),
                MatrixExpType.MMPA => new MMPA(),
                MatrixExpType.PADE => new PADE(),
                _ => throw new ArgumentOutOfRangeException("Invalid name exp")
            };
            _currentMatrixExp.CalculationStatusChangedEvent += OnCalculationFluxStatusChanged;

            Application.Current.Dispatcher.Invoke(() => Percent = 0);

            _burnUpProcess = new BurnUpProcess(_calculationPageViewModel.BurnUp, _currentMatrixExp, InitalDensities.Where(x => x.Density > 0).ToArray());

            _timeMeshDensities.Clear();

            var meshTime = TimeCalculation / _timeMeshCount;
            var times = new List<string>(); 
            for (int i = 0; i < _timeMeshCount; i++)
            {
                StatusText = $"Идет вычисление... '{(i + 1)}'";
                var fdensities = _burnUpProcess.Calculate((long)meshTime * (long)(i + 1) * 31536000);

                foreach (var dens in fdensities)
                {
                    dens.CalculateHeat(Constants.NaturalLeadDensity, 208);
                }
                //_timeMeshDensities.Add(new TimeDensties(meshTime * (i + 1), fdensities));
            }
            Application.Current.Dispatcher.Invoke(() => TimeMeshLineCollection.Clear());
            List<IGrouping<int, INuclideDensity>> dens1 = _timeMeshDensities
                .SelectMany(x => x.NuclideDensities)
                .GroupBy(x => x.Isotope.ZAID).ToList();

            foreach (var meshDensity in _timeMeshDensities)
            {
                var chart = new LineSeries()
                {
                    Title = "",
                    Values = new ChartValues<double>()
                };
                foreach (var density in meshDensity.NuclideDensities)
                {
                    chart.Values.Add(density.Density);
                }

                TimeMeshLineCollection.Add(chart);
            }
            Labels = _timeMeshDensities.Select(x => x.Time.ToString("F4")).ToArray();
            Application.Current.Dispatcher.Invoke(() =>
            { 
                OnPropertyChanged(nameof(TimeMeshLineCollection));
                OnPropertyChanged(nameof(Labels)); 
            });
            IsCalculationReady = true;
            IsCalculated = true;
        }

        private void FillBarchart(IEnumerable<INuclideDensity> densities) 
        {
            SeriesCollection[0].Values.Clear();
            LineCollection[0].Values.Clear();

            var names = new List<string>();
            foreach (var density in densities.Where(x => x.Density > 0))
            {
                SeriesCollection[0].Values.Add(density.Density);
                LineCollection[0].Values.Add(density.Density);
                names.Add(density.NuclideName);
            }
            Labels = names.ToArray();
            OnPropertyChanged(nameof(SeriesCollection));
        }

        private void OnFluxCalculate()
        {
            if (InitalDensities.All(x => x.Density == 0.0))
            {
                MessageBox.Show("Задайде начальную концентрацю", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_fluxIterations == 0)
            {
                MessageBox.Show("Не указана время облучения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_initialNeutronFlux == 0)
            {
                MessageBox.Show("Не указана начальный поток", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Task.Factory.StartNew(() =>
            {
                IsCalculationReady = false;
                IsCalculated = false;

                if (_currentMatrixExp != null)
                {
                    _currentMatrixExp.CalculationStatusChangedEvent -= OnCalculationStatusChanged;                
                    _currentMatrixExp.CalculationStatusChangedEvent -= OnCalculationFluxStatusChanged;
                }

                _currentMatrixExp = SelectedMatrixExp switch
                {
                    MatrixExpType.CRAM => new CRAM(),
                    MatrixExpType.MMPA => new MMPA(),
                    MatrixExpType.PADE => new PADE(),
                    _ => throw new ArgumentOutOfRangeException("Invalid name exp")
                };
                _currentMatrixExp.CalculationStatusChangedEvent += OnCalculationFluxStatusChanged;
                
                Application.Current.Dispatcher.Invoke(() => Percent = 0);
                var flux = _initialNeutronFlux;
                Application.Current.Dispatcher.Invoke(() => FluxHeatDensities.Clear());
                var filename = $"E://mesh_density_{DateTime.Now.ToShortTimeString()}.txt";
                Dictionary<int, IEnumerable<INuclideDensity>> zaidDensities = new();
                for (int i = 0; i < _fluxIterations; i++)
                {
                    var neutronSpectra = new NeutronSpectra(_calculationPageViewModel.SelectedTemperature * 1000, flux);
                    var burnUp = new BurnUp(_calculationPageViewModel.Isotopes, neutronSpectra);
                    _burnUpProcess = new BurnUpProcess(burnUp, _currentMatrixExp, InitalDensities.Where(x => x.Density > 0).ToArray());

                    StatusText = $"Идет вычисление... '{flux}'";
                    var fdensities = _burnUpProcess.Calculate(_totalCalculationSec);
                    foreach (var dens in fdensities)
                    {
                        dens.CalculateHeat(Constants.NaturalLeadDensity, 208);
                    }
                    zaidDensities.Add(i, fdensities);
                    var heatDensity = fdensities.Sum(x => x.HeatDensityMeV);
                    FluxHeatDensities.Add(new FluxHeatDensity(flux, heatDensity));
                    flux *= 10;
                }
                IsCalculationReady = true;
                IsCalculated = true;
                OnPropertyChanged(nameof(FluxHeatDensities));
            });
        }

        private void OnCalculate() 
        {

            if (InitalDensities.All(x => x.Density == 0.0))
            {
                MessageBox.Show("Задайде начальную концентрацю", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_timeCalculation == 0)
            {
                MessageBox.Show("Не указана время облучения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            Task.Factory.StartNew(() => 
            {
                IsCalculationReady = false;
                IsCalculated = false;
                Application.Current.Dispatcher.Invoke(() => Densities.Clear());
                
                if (_currentMatrixExp != null)
                {
                    _currentMatrixExp.CalculationStatusChangedEvent -= OnCalculationStatusChanged;
                    _currentMatrixExp.CalculationStatusChangedEvent -= OnCalculationFluxStatusChanged;
                }

                _currentMatrixExp = SelectedMatrixExp switch
                {
                    MatrixExpType.CRAM => new CRAM(),
                    MatrixExpType.MMPA => new MMPA(),
                    MatrixExpType.PADE => new PADE(),
                    _ => throw new ArgumentOutOfRangeException("Invalid name exp")
                };
                _currentMatrixExp.CalculationStatusChangedEvent += OnCalculationStatusChanged;
                _burnUpProcess = new BurnUpProcess(_calculationPageViewModel.BurnUp,
                    _currentMatrixExp,
                    InitalDensities.Where(x => x.Density > 0).ToArray());
                Application.Current.Dispatcher.Invoke(() => Percent = 0);
                StatusText = $"Идет вычисление... '{_selectedMatrixExp}'";
                var fdensities = _burnUpProcess.Calculate(_totalCalculationSec);
                foreach (var dens in fdensities)
                {
                    dens.CalculateHeat(Constants.NaturalLeadDensity, 208);
                }
                FillBarchart(fdensities);
                Application.Current.Dispatcher.Invoke(() => 
                {
                    Densities = new ObservableCollection<INuclideDensity>(fdensities.Where(x => x.Density > 0));
                    HeatDensity = fdensities.Sum(x => x.HeatDensityMeV);
                    
                    OnPropertyChanged(nameof(HeatDensity));
                    OnPropertyChanged(nameof(Densities));
                    OnPropertyChanged(nameof(SelectedEndfLibrary));
                    OnPropertyChanged(nameof(SelectedMacsLibrary));
                    OnPropertyChanged(nameof(Temperature));
                    OnPropertyChanged(nameof(IsotopeCount));
                    OnPropertyChanged(nameof(NeutronFlux));
                    OnPropertyChanged(nameof(SelectedMatrixExp));
                    OnPropertyChanged(nameof(IrradiationTime));
                    OnPropertyChanged(nameof(SeriesCollection));
                    OnPropertyChanged(nameof(LineCollection));
                });
                IsCalculationReady = true;
                IsCalculated = true;
            });
        }

        private void OnCalculationFluxStatusChanged(int progress)
        {
            Percent = progress / _fluxIterations;
        }

        private void OnCalculationStatusChanged(int progress)
        {
            Task.Factory.StartNew(() => 
            {
                lock (_progressLock)
                {
                    var currentPercent = Percent;
                    for (int i = (int)currentPercent; i < progress; i++)
                    {
                        Application.Current.Dispatcher.Invoke(() => Percent = i);
                        Thread.Sleep(30);
                    }
                }
            });
        }

        private string FormatTimeToString(double time)
        {
            return $"{_timeCalculation} {_selectedTimeScale}s";
        }

        public override void OnShow()
        {
            base.OnShow();
            if (Isotopes != null && InitalDensities.Count != Isotopes.Count())
            {
                var previous = InitalDensities.Where(x => x.Density > 0.0).ToList();
                Application.Current.Dispatcher.Invoke(() => InitalDensities.Clear());
                
                foreach (var isotope in _calculationPageViewModel.Isotopes)
                {
                    var dens = previous?.FirstOrDefault(x => x.Isotope.ZAID == isotope.ZAID)?.Density ?? 0;
                    Application.Current.Dispatcher.Invoke(() => InitalDensities.Add(new NuclideDensity(isotope, dens, Constants.NaturalLeadDensity, 208)));
                }
                Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(InitalDensities)));
            }
        }

        public Command FluxCalculateCommand { get; }

        public Command TimeMeshCalculateCommand { get; }

        /// <summary>
        /// FluxIterations
        /// </summary>
        public int FluxIterations
        {
            get => _fluxIterations;
            set { Set(ref _fluxIterations, value); }
        }

        /// <summary>
        /// InitialFluxText
        /// </summary>
        public string InitialFluxText
        {
            get => _initialFluxText;
            set 
            { 
                if (Set(ref _initialFluxText, value))
                {
                    double.TryParse(value, out _initialNeutronFlux);
                }
            }
        }

        private void OnGoToBack() 
        {
            AppContext.Instance.GetInstance<PageNavigation>().ShowCalculationPage();
        }

        private void OnExportRawData()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"KazNRDC-raw-data-result-{DateTime.Now:yyyy-MM-dd-hh-mm-ss}.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                using StreamWriter outputFile = new StreamWriter(saveFileDialog.FileName, true);
                outputFile.WriteLine($"Name\tZAID\tAtomicMass\t(n,g) CS\tDensity\t(N x CS)");
                foreach (var density in Densities.Where(x => x.Density > 1E-8 && x.Isotope.AvgCs != 0).OrderBy(x => x.Isotope.A))
                {
                    outputFile.WriteLine($"{density.Isotope.A}\t{density.Density}\t{density.Density * density.Isotope.AvgCs * 1E7}");
                }
                //foreach (var density in Densities.Where(x => x.Density > 0).OrderBy(x => x.Isotope.A))
                //{
                //    outputFile.WriteLine($"{density.NuclideName}\t{density.Isotope.ZAID}\t{density.Isotope.AtomicMass}\t{density.Isotope.AvgCs}\t{density.Density}\t{density.Density * density.Isotope.AvgCs}");
                //}
            }
        }

        private void OnExportRawDataLight()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"KazNRDC-raw-data-result-light-{DateTime.Now:yyyy-MM-dd-hh-mm-ss}.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                using StreamWriter outputFile = new StreamWriter(saveFileDialog.FileName, true);
                outputFile.WriteLine($"Name\tDensity\t(N x CS)");
                foreach (var density in Densities.Where(x => x.Density > 1E-8 && x.Isotope.AvgCs != 0).OrderBy(x => x.Isotope.A))
                {
                    outputFile.WriteLine($"{density.Isotope.A}\t{density.Density}\t{density.Density * density.Isotope.AvgCs * 1E7}");
                }
            }
        }

        public Func<double, string> FormatterY { get; set; }

        public IEnumerable<IIsotope> Isotopes => _calculationPageViewModel.Isotopes;

        protected override Control CreateView() => new SettingsPageView();

        public ObservableCollection<INuclideDensity> InitalDensities { get; }

        public ObservableCollection<INuclideDensity> Densities { get; private set; }

        public ObservableCollection<FluxHeatDensity> FluxHeatDensities { get; private set; }
        
        public Constants.MACSDATALIBS SelectedMacsLibrary => _calculationPageViewModel.SelectedMacsLibrary;

        public Constants.DATALIBS SelectedEndfLibrary => _calculationPageViewModel.SelectedEndfLibrary;

        public double HeatDensity { get; set; }

        public string NeutronFlux => _calculationPageViewModel.NeutronFluxText;

        public int Temperature => _calculationPageViewModel.SelectedTemperature;

        public string IrradiationTime => FormatTimeToString(_totalCalculationSec);

        public int IsotopeCount => Isotopes.Count();

        public Command GoToBackCommand { get; }

        public Command ExportRawDataCommand { get; }

        public Command ExportGroupdDataCommand { get; }

        public SeriesCollection SeriesCollection { get; set; }

        public SeriesCollection LineCollection { get; set; }

        public SeriesCollection TimeMeshLineCollection { get; set; }

        public string[] Labels { get; set; }
        
        public Func<double, string> Formatter { get; set; }

        public bool IsCalculationReady
        {
            get => _isCalculationReady;
            set => Set(ref _isCalculationReady, value);
        }

        public bool IsCalculated
        {
            get => _isCalculated;
            set => Set(ref _isCalculated, value);
        }

        /// <summary>
        /// TimeScales
        /// </summary>
        public IEnumerable<TimeScales> TimeScales { get; set; }

        private string _timeCalculationText;

        public string TimeCalculationText
        {
            get => _timeCalculationText;
            set 
            {
                Set(ref _timeCalculationText, value);
                if (double.TryParse(value, out double timeCalculation))
                {
                    TimeCalculation = timeCalculation;
                }
            }
        }

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
                            _calculationTimeSpan = TimeSpan.FromMilliseconds(_timeCalculation / 1000.0);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Millisecond:
                            _calculationTimeSpan = TimeSpan.FromMilliseconds(_timeCalculation);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Second:
                            _calculationTimeSpan = TimeSpan.FromSeconds(_timeCalculation);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Minute:
                            _calculationTimeSpan = TimeSpan.FromMinutes(_timeCalculation);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Hour:
                            _calculationTimeSpan = TimeSpan.FromHours(_timeCalculation);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Day:
                            _calculationTimeSpan = TimeSpan.FromDays(_timeCalculation);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Year:
                            _calculationTimeSpan = TimeSpan.FromDays(365 * _timeCalculation);
                            _totalCalculationSec = _timeCalculation * 365 * 86400;
                            break;
                        default:
                            break;
                    }
                    if (_totalCalculationSec < 0)
                    {
                        throw new InvalidDataException("Time is negative");
                    }
                }
            }
        }

        private int _timeMeshCount;

        /// <summary>
        /// TimeMeshCount
        /// </summary>
        public int TimeMeshCount
        {
            get { return _timeMeshCount; }
            set { Set(ref _timeMeshCount, value); }
        }

        /// <summary>
        /// HalfLifeLowerLimit
        /// </summary>
        public double TimeCalculation
        {
            get => _timeCalculation;
            set
            {
                if (Set(ref _timeCalculation, value))
                {
                    switch (SelectedTimeScale)
                    {
                        case ViewModels.TimeScales.Microsecond:
                            _calculationTimeSpan = TimeSpan.FromMilliseconds(value / 1000.0);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Millisecond:
                            _calculationTimeSpan = TimeSpan.FromMilliseconds(value);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Second:
                            _totalCalculationSec = value;
                            break;
                        case ViewModels.TimeScales.Minute:
                            _calculationTimeSpan = TimeSpan.FromMinutes(value);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Hour:
                            _calculationTimeSpan = TimeSpan.FromHours(value);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Day:
                            _calculationTimeSpan = TimeSpan.FromDays(value);
                            _totalCalculationSec = (long)_calculationTimeSpan.TotalSeconds;
                            break;
                        case ViewModels.TimeScales.Year:
                            _totalCalculationSec = (long)(value * 3.0E7);
                            break;
                        default:
                            break;
                    }

                    if (_totalCalculationSec < 0)
                    {
                        throw new InvalidDataException("Time is negative");
                    }
                }
            }
        }

        /// <summary>
        /// CalculateCommand
        /// </summary>
        public Command CalculateCommand { get; }

        /// <summary>
        /// SelectedMatrixExp
        /// </summary>
        public MatrixExpType SelectedMatrixExp
        {
            get => _selectedMatrixExp;
            set => Set(ref _selectedMatrixExp, value);
        }

        /// <summary>
        /// IsBarChartSelected
        /// </summary>
        public bool IsBarChartSelected
        {
            get => _isBarChartSelected;
            set => Set(ref _isBarChartSelected, value);
        }

        /// <summary>
        /// MatrixExpList
        /// </summary>
        public IEnumerable<MatrixExpType> MatrixExpList { get; }

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

        public Command CancelCommand => throw new NotImplementedException();

        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);            
        }
    }   
    
    enum MatrixExpType
    {
        PADE,
        CRAM,
        MMPA
    }

    class TimeDensties
    {
        public TimeDensties(long time, IEnumerable<INuclideDensity> nuclideDensities)
        {
            Time = time;
            NuclideDensities = nuclideDensities;
        }
        public long Time { get; }

        public IEnumerable<INuclideDensity> NuclideDensities { get; }
    }

    class FluxHeatDensity 
    {
        public FluxHeatDensity(double flux, double heatDensity)
        {
            Flux = flux;
            HeatDensity = heatDensity;
        }

        public double Flux { get; }

        public double HeatDensity { get; }
    }
}
