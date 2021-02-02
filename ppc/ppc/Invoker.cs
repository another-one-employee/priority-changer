using System;
using ppc.Commands;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ppc
{
    [DataContract]
    [KnownType(typeof(CreateCommand))]
    [KnownType(typeof(DeleteCommand))]
    [KnownType(typeof(UpdateCommand))]
    class Invoker
    {
        [DataMember]
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
            try
            {
                _command = _history.Pop();
            }
            catch(InvalidOperationException)
            {
                throw new InvalidOperationException("History commands is empty");
            }
            var command = _command as ICancelable;
            command.Undo();
        }
    }
}
