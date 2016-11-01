using System.IO;

namespace Octodiff.CommandLine.Support
{
    interface ICommand
    {
        void GetHelp(TextWriter writer);
        int Execute(string[] commandLineArguments);
    }
}
