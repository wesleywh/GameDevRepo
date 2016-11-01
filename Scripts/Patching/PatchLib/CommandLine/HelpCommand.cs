using System;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using Octodiff.CommandLine.Support;

namespace Octodiff.CommandLine
{
    [Command("help", "?", "h", Description = "Prints this help text")]
    class HelpCommand : ICommand
    {
        readonly ICommandLocator commands = new CommandLocator();

        public void GetHelp(TextWriter writer)
        {
        }

        public int Execute(string[] commandLineArguments)
        {
            var executable = Path.GetFileNameWithoutExtension(new Uri(typeof(HelpCommand).Assembly.CodeBase).LocalPath);

            var commandName = commandLineArguments.FirstOrDefault();

            if (string.IsNullOrEmpty(commandName))
            {
                PrintGeneralHelp(executable);
            }
            else
            {
                var commandMeta = commands.Find(commandName);
                if (commandMeta == null)
                {
                    //Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Command '{0}' is not supported", commandName);
                    //Console.ResetColor();
                    PrintGeneralHelp(executable);
                }
                else
                {
                    var command = commands.Create(commandMeta);
                    PrintCommandHelp(executable, command, commandMeta, commandName);
                }
            }

            return 0;
        }

        void PrintCommandHelp(string executable, ICommand command, ICommandMetadata commandMetadata, string commandName)
        {
            //Console.ResetColor();
            Console.Write("Usage: ");
            //Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(executable + " " + commandName + (!StringOperators.IsNullOrWhiteSpace(commandMetadata.Usage) ? " " + commandMetadata.Usage : "") + " [<options>]");
           // Console.ResetColor();
            Console.WriteLine();
            command.GetHelp(Console.Out);

            Console.WriteLine();
        }

        void PrintGeneralHelp(string executable)
        {
            //Console.ResetColor();
            Console.Write("Usage: ");
            //Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(executable + " <command>");
            //Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Where <command> is one of: ");
            Console.WriteLine();

            foreach (var possible in commands.List().OrderBy(x => x.Name))
            {
                //Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("  " + possible.Name.PadRight(15, ' '));
                //Console.ResetColor();
                Console.WriteLine("   " + possible.Description);
            }

            Console.WriteLine();
            Console.Write("Or use ");
            //Console.ForegroundColor = ConsoleColor.White;
            Console.Write(executable + " help <command>");
            //Console.ResetColor();
            Console.WriteLine(" for more details.");
        }
    }
}