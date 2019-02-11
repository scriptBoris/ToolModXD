using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ToolModXdGui.Core
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Func _del;
        public delegate void Func(object obj);

        public Command(Action action)
        {
            _del = (obj) => action();
        }

        public Command(Func action)
        {
            _del = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _del.Invoke(parameter);
        }
    }
}
