using System;
using System.Windows.Input;

namespace GraphicsSandbox
{
    public class MyElementCommand : ICommand
    {
        private readonly ElementViewModel _elementViewModel;
        private readonly Action<ElementViewModel> _command;

        public MyElementCommand(ElementViewModel elementViewModel, Action<ElementViewModel> command)
        {
            _elementViewModel = elementViewModel;
            _command = command;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _command(_elementViewModel);
        }

        public event EventHandler CanExecuteChanged;
    }
}