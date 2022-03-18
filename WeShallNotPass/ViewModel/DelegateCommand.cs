using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WeShallNotPass.ViewModel
{
    public class DelegateCommand : ICommand
    {
        private Func<object, bool> canExecute;
        private Action<object> execute;

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (canExecute == null) return true;
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
                execute(parameter);
        }
    }
}
