using System;

namespace ppc.Commands
{
    class DeleteCommand : ICancelable
    {
        private string _key;
        private CpuPriorityLevel _oldPriorityLevel;

        public DeleteCommand(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("The key value is null");
            }

            _key = key.Contains(".") ? key : key + ".exe";
        }

        public void Execute()
        {
            _oldPriorityLevel = CpuPriorityOptionsWorker.Delete(_key);
            Console.WriteLine($"Startup settings have been deleted for '{_key}'" +
                $" with priority level: {_oldPriorityLevel}");
        }

        public void Undo()
        {
            CpuPriorityOptionsWorker.Create(_key, _oldPriorityLevel);
            Console.WriteLine($"Deleting startup settings for '{_key}' canceled");
        }
    }
}
