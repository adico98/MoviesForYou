using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MovieRenter.Command
{
    public class CommandHandler : ICommand
    {
        private Action<object> _paramAction;
        private bool _canExecute;

        private Action _action;

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="action">Action to be executed by the command</param>
        /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }
        public CommandHandler(Action<object> action, bool canExecute)
        {
            _paramAction = action;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            if (_action == null)
                _paramAction(parameter);
            else
                _action();
        }

    }
        
}
