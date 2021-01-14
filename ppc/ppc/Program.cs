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
                    level = CpuLevelStringToInt(args.Last());
                    key = ExtractingKeyFromArguments(args, 1, 1);
                    invoker.SetCommand(new CreateCommand(key, level));
                    break;

                case "r":
                case "read":
                    invoker.SetCommand(new ReadAllCommand());
                    break;

                case "u":
                case "update":
                    level = CpuLevelStringToInt(args.Last());
                    key = ExtractingKeyFromArguments(args, 1, 1);
                    invoker.SetCommand(new UpdateCommand(key, level));
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

            invoker.Run();
        }

        private static int CpuLevelStringToInt(string value)
        {
            int level;
            if (!Int32.TryParse(value, out level))
            {
                level = (int)Enum.Parse(typeof(CpuPriorityLevel), value);
            }

            return level;
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
