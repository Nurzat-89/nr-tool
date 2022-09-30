using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace GUI.Utils
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public bool Set<T>(ref T aStorage, T aValue, [CallerMemberName] string aPropertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(aStorage, aValue))
                return false;
            aStorage = aValue;
            OnPropertyChanged(aPropertyName);
            return true;
        }

        public void OnPropertyChanged([CallerMemberName] string aProp = "") 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aProp));
        }
    }
}
