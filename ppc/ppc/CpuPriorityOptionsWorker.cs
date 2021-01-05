using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ppc
{
    public static class CpuPriorityOptionsWorker
    {
        private static readonly RegistryKey _workingKey = Registry.LocalMachine.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options", true);

        private static RegistryKey GetPerfOptions(string key)
        {
            RegistryKey perfOptions = _workingKey.OpenSubKey(key + @"\PerfOptions", true);

            if (perfOptions == null)
            {
                throw new ArgumentException("Key not found");
            }

            return perfOptions;
        }

        private static CpuPriorityLevel GetCpuPriorityLevel(string key)
        {
            RegistryKey perfOptions = GetPerfOptions(key);
            object keyValue = perfOptions.GetValue("CpuPriorityClass");

            if (keyValue == null)
            {
                throw new ArgumentException("CpuPriorityClass not found");
            }

            Int32.TryParse(keyValue.ToString(), out int oldPriorityLevel);

            if (oldPriorityLevel < 1 || oldPriorityLevel > 6)
            {
                oldPriorityLevel = 2;
                Console.WriteLine("Failed to get old level priority value" + "\n"
                    + "When undoing this operation, the value will be set to Normal");
            }

            return (CpuPriorityLevel)oldPriorityLevel;
        }

        public static void Create(string key, CpuPriorityLevel priorityLevel)
        {
            RegistryKey subKey = _workingKey.CreateSubKey(key);
            RegistryKey perfOptions = subKey.CreateSubKey("PerfOptions");

            perfOptions.SetValue("CpuPriorityClass", priorityLevel, RegistryValueKind.DWord);
            perfOptions.Close();
        }

        public static CpuPriorityLevel Delete(string key)
        {
            var priorityLevel = CpuPriorityLevel.Normal;

            try
            {
                priorityLevel = GetCpuPriorityLevel(key);
                _workingKey.DeleteSubKeyTree(key, true);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return priorityLevel;
        }

        public static CpuPriorityLevel Update(string key, CpuPriorityLevel priorityLevel)
        {
            var oldPriorityLevel = CpuPriorityLevel.Normal;

            try
            {
                oldPriorityLevel = GetCpuPriorityLevel(key);

                RegistryKey perfOptions = _workingKey.OpenSubKey(key + @"\PerfOptions", true);
                perfOptions.SetValue("CpuPriorityClass", priorityLevel, RegistryValueKind.DWord);
                perfOptions.Close();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oldPriorityLevel;
        }

        public static Dictionary<string, CpuPriorityLevel> ReadAll()
        {
            return _workingKey
                .GetSubKeyNames()
                .Where(subKey => _workingKey.OpenSubKey(subKey + @"\PerfOptions") != null)
                .Where(subKey => _workingKey
                    .OpenSubKey(subKey + @"\PerfOptions")
                    .GetValueNames()
                    .Contains("CpuPriorityClass"))
                .Select(s => Tuple.Create(s, GetCpuPriorityLevel(s)))
                .ToDictionary(key => key.Item1, value => value.Item2);
        }
    }
}
