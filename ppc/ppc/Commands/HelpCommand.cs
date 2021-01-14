using System;

namespace ppc.Commands
{
    class HelpCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Some information"); //add description
        }
    }
}
