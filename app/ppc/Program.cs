using ppc.Commands;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace ppc
{
    static class Program
    {
        public static readonly string Name = Process.GetCurrentProcess().ProcessName;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var help = new HelpCommand();
                help.Execute();
                return;
            }

            Invoker invoker = null;
            try
            {
                invoker = ReadInvoker();
            }
            catch (SerializationException serExc)
            {
                Console.WriteLine("Serialization Failed");
                Console.WriteLine(serExc.Message);
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine($"Started recording command history in '{Name}.history.xml'");
            }
            catch (Exception exc)
            {
                Console.WriteLine(
                $"The read history of commands from '{Name}.history.xml' failed: {0} StackTrace: {1}",
                exc.Message, exc.StackTrace);
            }
            finally
            {
                if (invoker == null)
                {
                    invoker = new Invoker();
                }
            }

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
                    try
                    {
                        invoker.Undo();
                    }
                    catch(InvalidOperationException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch(ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Failed to cancel command");
                        Console.WriteLine(invoker.GetCommand().ToString());
                    }
                    WriteInvoker(invoker);
                    return;

                case "h":
                case "help":
                    invoker.SetCommand(new HelpCommand());
                    break;

                default:
                    Console.WriteLine($"'{args.First()}' is not a command. See '{Name} help'.");
                    return;
            }

            try
            {
                invoker.Run();
            }
            catch(ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }

            WriteInvoker(invoker);
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

        private static Invoker ReadInvoker()
        {
            using (FileStream reader = new FileStream($"{Name}.history.xml", FileMode.Open))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(Invoker));
                return (Invoker)serializer.ReadObject(reader);
            }
        }

        private static void WriteInvoker(Invoker invoker)
        {
            using (FileStream writer = new FileStream($"{Name}.history.xml", FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(Invoker));
                serializer.WriteObject(writer, invoker);
            }
        }
    }
}
