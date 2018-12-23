using System;
using System.Windows.Input;

namespace RVMCore.MasterView
{
    /// <summary>
    /// For UI Command.
    /// </summary>
    public class CustomCommand : ICommand
    {
        private Action Command;
        private bool canExecute { get; set; } = true;

        public CustomCommand(Action command)
        {
            this.Command = command;
        }

        public CustomCommand(Action command, bool _canExecute)
        {
            this.Command = command;
            this.canExecute = _canExecute;
        }

        public event EventHandler CanExecuteChanged = (sender, e) =>{};

        public bool CanExecute(object parameter) => this.canExecute;

        public void Execute(object parameter)
        {
            Command();
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
