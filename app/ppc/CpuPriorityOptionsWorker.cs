using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ppc
{
    public static class CpuPriorityOptionsWorker
    {
        private static readonly RegistryKey WorkingKey = Registry.LocalMachine.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options", true);

        private static RegistryKey GetPerfOptions(string key)
        {
            RegistryKey perfOptions = WorkingKey.OpenSubKey(key + @"\PerfOptions", true);

            if (perfOptions == null)
            {
                throw new ArgumentException($"Key: '{key}' not found");
            }

            return perfOptions;
        }

        private static CpuPriorityLevel GetCpuPriorityLevel(string key)
        {
            RegistryKey perfOptions = GetPerfOptions(key);
            object keyValue = perfOptions.GetValue("CpuPriorityClass");

            if (keyValue == null)
            {
                throw new ArgumentException($"CpuPriorityClass not found in '{key}'");
            }

            Int32.TryParse(keyValue.ToString(), out int oldPriorityLevel);

            if (oldPriorityLevel < 1 || oldPriorityLevel > 6)
            {
                throw new ArgumentException($"Failed to get old level priority value from: '{oldPriorityLevel}'");
            }

            return (CpuPriorityLevel)oldPriorityLevel;
        }

        public static void Create(string key, CpuPriorityLevel priorityLevel)
        {
            RegistryKey subKey = WorkingKey.CreateSubKey(key);
            RegistryKey perfOptions = subKey?.CreateSubKey("PerfOptions");

            perfOptions?.SetValue("CpuPriorityClass", priorityLevel, RegistryValueKind.DWord);
            perfOptions?.Close();
        }

        public static CpuPriorityLevel Delete(string key)
        {
            var priorityLevel = GetCpuPriorityLevel(key);
            WorkingKey.DeleteSubKeyTree(key, true);

            return priorityLevel;
        }

        public static CpuPriorityLevel Update(string key, CpuPriorityLevel priorityLevel)
        {
            var oldPriorityLevel = GetCpuPriorityLevel(key);
            
            RegistryKey perfOptions = WorkingKey.OpenSubKey(key + @"\PerfOptions", true);
            perfOptions?.SetValue("CpuPriorityClass", priorityLevel, RegistryValueKind.DWord);
            perfOptions?.Close();

            return oldPriorityLevel;
        }

        public static Dictionary<string, string> ReadAll()
        {
            return WorkingKey
                .GetSubKeyNames()
                .Where(subKey => WorkingKey.OpenSubKey(subKey + @"\PerfOptions") != null)
                .Where(subKey =>
                {
                    var perfOptionsKey = WorkingKey
                        .OpenSubKey(subKey + @"\PerfOptions");

                    if (perfOptionsKey == null)
                    {
                        return false;
                    }

                    return perfOptionsKey
                        .GetValueNames()
                        .Contains("CpuPriorityClass");
                })
                .Select(s =>
                {
                    string cpuLvl = null;
                    try
                    {
                        cpuLvl = GetCpuPriorityLevel(s).ToString();
                    }
                    catch(ArgumentException ex)
                    {
                        cpuLvl = ex.Message;
                    }
                    finally
                    {
                        cpuLvl ??= "Undefined";
                    }

                    return Tuple.Create(s, cpuLvl);
                })
                .ToDictionary(key => key.Item1, value => value.Item2);
        }
    }
}
