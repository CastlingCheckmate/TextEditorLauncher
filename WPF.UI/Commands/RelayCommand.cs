using System;
using System.Windows.Input;

namespace WPF.UI.Commands
{

    // класс команды
    // реализуем ICommand, где Execute - выполнение действия, CanExecute - проверка на то, можно ли это действие выполнить
    public class RelayCommand : ICommand
    {

        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        // сохраняем ссылки на переданные делегаты
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        // если ссылка на делегат, отвечающий за возможность выполнения, null, или на переданном параметре он возвращает true, то основное действие (Execute) можно выполнять
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        // выполняем действие, вызывая делегат с переданным параметром
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

    }

}