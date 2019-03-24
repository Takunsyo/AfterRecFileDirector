using System;
using System.Windows.Input;

namespace RVMCore.MasterView
{
    /// <summary>
    /// For UI Command.
    /// </summary>
    public class CustomCommand : ICommand
    {
        private Action<object> Command;
        private bool canExecute { get; set; } = true;

        public CustomCommand(Action<object> command)
        {
            this.Command = command;
        }

        public CustomCommand(Action<object> command, bool _canExecute)
        {
            this.Command = command;
            this.canExecute = _canExecute;
        }

        public event EventHandler CanExecuteChanged = (sender, e) =>{};

        public bool CanExecute(object parameter) => this.canExecute;

        public void Execute(object parameter)
        {
            Command.DynamicInvoke(parameter);
        }

        public void SetCanExecute(bool value)
        {
            if (this.canExecute == value)
            {
                this.canExecute = value;
                CanExecuteChanged.Invoke(this, null);
            }
        }
    }
}
