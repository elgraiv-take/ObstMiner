using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ObstMinerGui
{
    class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private bool _isExecutable = true;
        public bool IsExecutable
        {
            get => _isExecutable;
            set
            {
                if (Equals(_isExecutable, value))
                {
                    _isExecutable = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private Action _action;
        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return _isExecutable;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }
    }
}
