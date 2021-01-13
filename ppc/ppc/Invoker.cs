using System.Collections.Generic;
using ppc.Commands;

namespace ppc
{
    class Invoker
    {
        private Stack<ICommand> _history;
        private ICommand _command;

        public Invoker()
        {
            _history = new Stack<ICommand>();
        }

        public void SetCommand(ICommand command)
        {
            _command = command;
        }

        public void Run()
        {
            _command.Execute();
            if (_command is ICancelable)
            {
                _history.Push(_command);
            }
        }

        public void Undo()
        {
            _command = _history.Pop();
            var command = _command as ICancelable;
            command.Undo();
        }
    }
}
