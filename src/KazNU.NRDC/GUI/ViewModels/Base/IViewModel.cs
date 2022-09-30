using System.Windows.Controls;

namespace GUI.ViewModels.Base
{
    /// <summary>
    /// Интерфейс VM, который представляет интерфейс
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// Интерфейс, управляемый данной VM
        /// </summary>
        Control View { get; }
    }
}
