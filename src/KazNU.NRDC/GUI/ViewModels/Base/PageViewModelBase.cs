using GUI.Utils;
using GUI.ViewModels.Base;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GUI.ViewModels
{
    /// <summary>
    /// Базовый класс VM, который представляет интерфейс
    /// </summary>
    public abstract class PageViewModelBase : BindableBase, IViewModel
    {
        #region fields

        private readonly Lazy<Control> fViewLazy;

        #endregion

        #region constructor/destructor

        protected PageViewModelBase()
        {
            fViewLazy = new Lazy<Control>(() => Application.Current.Dispatcher?.Invoke(CreateView));
        }

        #endregion

        #region methods

        /// <summary>
        /// Создание контрола интерфейса (View)
        /// </summary>
        /// <returns></returns>
        protected abstract Control CreateView();

        /// <summary>
        /// Действия, выполняемые при отображении View
        /// </summary>
        public virtual void OnShow()
        {

        }

        /// <summary>
        /// Действия, выполняемые при закрытии View
        /// </summary>
        public virtual void OnClose()
        {

        }

        #endregion

        #region properties

        /// <summary>
        /// Интерфейс, управляемый данной VM
        /// </summary>
        public Control View
        {
            get
            {
                if (!fViewLazy.IsValueCreated)
                    Application.Current.Dispatcher?.Invoke(() => fViewLazy.Value.DataContext = this);
                return fViewLazy.Value;
            }
        }

        #endregion
    }
}
