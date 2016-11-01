using System;

namespace Octodiff.CommandLine.Support
{
    class CommandException : Exception
    {
        public CommandException(string message)
            : base(message)
        {
        }
    }
}