using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace RVMCore.MasterView
{    
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender,e)=> { };
        #region Property Changed call methods.
        [Obsolete("This method is no longer in use due to implementation of INotifyPropertyChanged.Fody namespace.", true)]
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
        [Obsolete("This method is no longer in use due to implementation of INotifyPropertyChanged.Fody namespace.", true)]
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //[Obsolete("This method is no longer in use due to implementation of INotifyPropertyChanged.Fody namespace.", false)]
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        protected ViewModelBase()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
        #region Multi thread UI
        /// <summary>
        /// Gets the dispatcher used by this view model to execute actions on the thread it is associated with.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Threading.Dispatcher"/> used by this view model to 
        /// execute actions on the thread it is associated with. 
        /// The default value is the <see cref="System.Windows.Threading.Dispatcher.CurrentDispatcher"/>.
        /// </value>
        protected Dispatcher Dispatcher
        {
            get
            {
                return _dispatcher;
            }
        }
        private readonly Dispatcher _dispatcher;
        /// <summary>
        /// Executes the specified <paramref name="action"/> synchronously on the thread 
        /// the <see cref="ViewModelBase"/> is associated with.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to execute.</param>
        protected void Execute(Action action)
        {
            if (this.Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                this.Dispatcher.Invoke(DispatcherPriority.DataBind, action);
            }
        }
        #endregion

    }
}
