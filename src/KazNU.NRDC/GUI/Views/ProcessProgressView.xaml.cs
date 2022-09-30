using GUI.Utils;
using System;
using System.Threading;
using System.Windows.Controls;

namespace GUI.Views
{
    /// <summary>
    /// Interaction logic for ProcessProgressView.xaml
    /// </summary>
    public partial class ProcessProgressView : UserControl
    {
        public ProcessProgressView()
        {
            InitializeComponent();
        }
    }

    interface IProcessProgressViewModel
    {
        /// <summary>
        /// Значение прогресса
        /// </summary>
        double Percent { get; set; }

        /// <summary>
        /// Строковое представление значения прогресса
        /// </summary>
        string PercentText { get; }

        /// <summary>
        /// Команда отмены 
        /// </summary>
        Command CancelCommand { get; }
    }

    class DesignProcessProgressViewModel : BindableBase, IProcessProgressViewModel
    {
        #region fields

        private double fPercent;
        private Command fCancelCommand;
        private CancellationTokenSource fCancellationTokenSource;
        private CancellationToken fCancellationToken;

        #endregion

        #region methods

        public void ChangeData(double aPercent, CancellationToken aCancellationToken)
        {
            this.Percent = aPercent;
        }

        public void Start()
        {
            fCancellationTokenSource = new CancellationTokenSource();
            fCancellationToken = fCancellationTokenSource.Token;

            StartOperationsEvent?.Invoke(fCancellationToken);
        }

        #endregion

        #region properties

        public double Percent
        {
            get => fPercent;
            set => this.Set(ref fPercent, value);
        }
        public string PercentText
        {
            get
            {
                return $"{fPercent} %";
            }
        }

        public Command CancelCommand => fCancelCommand ??= new Command(() =>
        {

        });

        #endregion

        #region events

        public event Action<CancellationToken> StartOperationsEvent;

        #endregion

    }
}
