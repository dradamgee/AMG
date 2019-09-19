using System;
using System.Windows.Input;

namespace GraphicsSandbox
{
    public class MyElementCommand : ICommand
    {
        private readonly Element _element;
        private readonly Action<Element> _command;

        public MyElementCommand(Element element, Action<Element> command)
        {
            _element = element;
            _command = command;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _command(_element);
        }

        public event EventHandler CanExecuteChanged;
    }
}