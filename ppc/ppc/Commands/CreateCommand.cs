using System;

namespace ppc.Commands
{
    class CreateCommand : ICancelable
    {
        private string _key;
        private CpuPriorityLevel _priorityLevel;

        public CreateCommand(string key, int priorityLevel)
        {
            if (key == null)
            {
                throw new ArgumentNullException("The key value is null");
            }

            _key = key.Contains(".") ? key : key + ".exe";

            if (priorityLevel < 1 || priorityLevel > 6)
            {
                throw new ArgumentException($"Wrong priority level: {priorityLevel}");
            }

            _priorityLevel = (CpuPriorityLevel)priorityLevel;
        }

        public void Execute()
        {
            CpuPriorityOptionsWorker.Create(_key, _priorityLevel);
            Console.WriteLine($"Startup settings have been created for '{_key}'" +
                $" with priority level: {_priorityLevel}");
        }

        public void Undo()
        {
            CpuPriorityOptionsWorker.Delete(_key);
            Console.WriteLine($"Creation of settings for '{_key}' has been canceled");
        }
    }
}
