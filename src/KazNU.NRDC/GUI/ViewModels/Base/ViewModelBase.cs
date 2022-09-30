using GUI.Utils;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GUI.ViewModels.Base
{
    public abstract class ViewModelBase : BindableBase, IViewModel
    {
        private readonly Lazy<Control> fViewLazy;

        protected ViewModelBase()
        {
            fViewLazy = new Lazy<Control>(() => Application.Current?.Dispatcher?.Invoke(CreateView));
        }


        /// <summary>
        /// Создание контрола интерфейса
        /// </summary>
        /// <returns></returns>
        protected abstract Control CreateView();

        /// <summary>
        /// Интерфейс, управляемый данной VM
        /// </summary>
        public Control View
        {
            get
            {
                if (!fViewLazy.IsValueCreated)
                    fViewLazy.Value.DataContext = this;
                return fViewLazy.Value;
            }
        }
    }
}
