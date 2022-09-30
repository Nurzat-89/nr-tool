using System;
using System.Windows.Input;

namespace GUI.Utils
{
    internal class Command : ICommand
    {
        private Action _execute;
        Func<bool> _canExecute;

        public Command(Action execute)
        {
            _execute = execute;
        }

        public Command(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute?.Invoke();
        }
    }
}
