using System;
using System.Runtime.Serialization;

namespace ppc.Commands
{
    [DataContract]
    class UpdateCommand : ICancelable
    {
        [DataMember]
        private readonly string _key;
        [DataMember]
        private CpuPriorityLevel _oldPriorityLevel;
        private readonly CpuPriorityLevel _priorityLevel;

        public UpdateCommand(string key, int priorityLevel)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "The key value is null");
            }

            _key = key.Contains(".") ? key : key + ".exe";

            if (priorityLevel < 1 || priorityLevel > 6)
            {
                throw new ArgumentException("Wrong priority level");
            }

            if (priorityLevel == 4)
            {
                throw new ArgumentException("The level cannot be RealTime, " +
                    "because the system will detect it as Normal");
            }

            _priorityLevel = (CpuPriorityLevel)priorityLevel;
        }

        public void Execute()
        {
            _oldPriorityLevel = CpuPriorityOptionsWorker.Update(_key, _priorityLevel);
            Console.WriteLine($"Priority level for '{_key}'" +
                $" update from {_oldPriorityLevel} to {_priorityLevel}");
        }

        public void Undo()
        {
            CpuPriorityOptionsWorker.Update(_key, _oldPriorityLevel);
            Console.WriteLine($"Update for '{_key}' has been canceled." +
                $" Current priority level: {_oldPriorityLevel}");
        }

        public override string ToString()
        {
            return $"Command: Update\nKey: {_key}\nOld priority level: {_oldPriorityLevel}";
        }
    }
}
