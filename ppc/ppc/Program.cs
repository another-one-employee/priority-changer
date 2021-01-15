using ppc.Commands;
using System;
using System.Linq;

namespace ppc
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var help = new HelpCommand();
                help.Execute();
                return;
            }

            Invoker invoker = new Invoker();
            int level;
            string key;

            switch (args.First())
            {
                case "c":
                case "create":
                    if (!TryParseCpuLevelStringToInt(args.Last(), out level))
                    {
                        Console.WriteLine($"Unknown priority level: '{args.Last()}'");
                        return;
                    }
                    key = ExtractingKeyFromArguments(args, 1, 1);

                    try
                    {
                        invoker.SetCommand(new CreateCommand(key, level));
                    }
                    catch(ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    break;

                case "r":
                case "read":
                    invoker.SetCommand(new ReadAllCommand());
                    break;

                case "u":
                case "update":
                    if (!TryParseCpuLevelStringToInt(args.Last(), out level))
                    {
                        Console.WriteLine($"Unknown priority level: '{args.Last()}'");
                        return;
                    }
                    key = ExtractingKeyFromArguments(args, 1, 1);

                    try
                    {
                        invoker.SetCommand(new UpdateCommand(key, level));
                    }
                    catch(ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    break;

                case "d":
                case "delete":
                    key = ExtractingKeyFromArguments(args, 1, 0);
                    invoker.SetCommand(new DeleteCommand(key));
                    break;

                case "undo":
                    invoker.Undo();
                    return;

                default:
                    invoker.SetCommand(new HelpCommand());
                    break;
            }

            try
            {
                invoker.Run();
            }
            catch(ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool TryParseCpuLevelStringToInt(string value, out int result)
        {
            CpuPriorityLevel temp;
            if (Enum.TryParse(value, true, out temp))
            {
                result = (int)temp;
                return true;
            }
            else
            {
                result = 0;
                return false;
            }
        }

        private static string ExtractingKeyFromArguments(string[] args, int countOfCommandWords, int countOfOtherArgs)
        {
            var temp = args
                .TakeWhile((x, i) => i != args.Length - countOfOtherArgs)
                .Skip(countOfCommandWords);

            return String.Join(" ", temp);
        }
    }
}
