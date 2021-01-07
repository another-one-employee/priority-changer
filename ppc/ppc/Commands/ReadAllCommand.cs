using System;

namespace ppc.Commands
{
    class ReadAllCommand : ICommand
    {
        public void Execute()
        {
            var keys = CpuPriorityOptionsWorker.ReadAll();

            Console.WriteLine($"{"Key:", -30} {"Priority level:", 5}");
            foreach (var e in keys)
            {
                Console.WriteLine($"{e.Key, -30} {e.Value, 5}");
            }
        }
    }
}
