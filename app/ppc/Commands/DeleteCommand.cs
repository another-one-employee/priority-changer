using System;
using System.Runtime.Serialization;

namespace ppc.Commands
{
    [DataContract]
    class DeleteCommand : ICancelable
    {
        [DataMember]
        private readonly string _key;
        [DataMember]
        private CpuPriorityLevel _oldPriorityLevel;

        public DeleteCommand(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "The key value is null");
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

        public override string ToString()
        {
            return $"Command: Delete\nKey: {_key}\nOld priority level: {_oldPriorityLevel}";
        }
    }
}
