using System;

namespace ppc.Commands
{
    class HelpCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine(
                "Usage: ppc <command> [<args>]\n\n" +
                "This utility works with Windows registry keys in\n" +
                "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\n" +
                "Allowing to create application launch rules with a given cpu priority level\n\n" +
                "These are basic commands:\n" +
                $"{"Command:",-10} {"Abbreviation:",-15} {"Description:",-20}\n\n" +
                $"{"create",-10} {"c",-15} {"create <file name> <level>",-20}\n" +
                $"{"",-10} {"",-15} {"Creates a key with the specified startup rules.",-20}\n\n" +
                $"{"update",-10} {"u",-15} {"update <file name> <level>",-20}\n" +
                $"{"",-10} {"",-15} {"Updates an existing key to the specified level.",-20}\n\n" +
                $"{"delete",-10} {"d",-15} {"delete <file name>",-20}\n" +
                $"{"",-10} {"",-15} {"Deletes an existing key.",-20}\n\n" +
                $"{"read",-10} {"r",-15} {"read",-20}\n" +
                $"{"",-10} {"",-15} {"Reads all keys with their values and outputs them to the console.",-20}\n\n" +
                $"{"undo",-10} {"",-15} {"undo",-20}\n" +
                $"{"",-10} {"",-15} {"Cancels the Create, Delete, Update command.",-20}\n" +
                $"{"",-10} {"",-15} {"The history of commands is stored in \"ppc.history.xml\"",-20}\n\n" +
                $"{"help",-10} {"h",-15} {"help",-20}\n" +
                $"{"",-10} {"",-15} {"Calls the current help.",-20}\n\n" +
                "If no extension is specified in <file name>, the default will be used: \".exe\"\n" +
                "The <level> value must be (case ignored):\n" +
                $"{"String:",-12} {"Digital equivalent:",-15}\n\n" +
                $"{"Idle", -12} {"1", -15}\n" +
                $"{"Normal",-12} {"2",-15}\n" +
                $"{"High",-12} {"3",-15}\n" +
                $"{"RealTime",-12} {"4",-15}\n" +
                $"{"BelowNormal",-12} {"5",-15}\n" +
                $"{"AboveNormal",-12} {"6",-15}\n\n" +
                "Example: 'create cmd.exe high' or 'c cmd 3'"
                );
        }
    }
}
