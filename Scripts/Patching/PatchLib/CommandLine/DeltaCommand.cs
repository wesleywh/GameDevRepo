using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using Octodiff.CommandLine.Support;
using Octodiff.Core;
using Octodiff.Diagnostics;

namespace Octodiff.CommandLine
{
    [Command("delta", Description = "Given a signature file and a new file, creates a delta file", Usage = "<signature-file> <new-file> [<delta-file>]")]
    class DeltaCommand : ICommand
    {
        private readonly List<Action<DeltaBuilder>> configuration = new List<Action<DeltaBuilder>>();
        private readonly OptionSet options;
        private string newFilePath;
        private string signatureFilePath;
        private string deltaFilePath;

        public DeltaCommand()
        {
            options = new OptionSet();
            options.Positional("signature-file", "The file containing the signature from the basis file.", v => signatureFilePath = v);
            options.Positional("new-file", "The file to create the delta from.", v => newFilePath = v);
            options.Positional("delta-file", "The file to write the delta to.", v => deltaFilePath = v);
            options.Add("progress", "Whether progress should be written to stdout", v => configuration.Add(builder => builder.ProgressReporter = new ConsoleProgressReporter()));
        }

        public void GetHelp(TextWriter writer)
        {
            options.WriteOptionDescriptions(writer);
        }

        public int Execute(string[] commandLineArguments)
        {
			options.Parse(commandLineArguments);

			if (StringOperators.IsNullOrWhiteSpace(signatureFilePath))
                throw new OptionException("No signature file was specified", "new-file");
			if (StringOperators.IsNullOrWhiteSpace(newFilePath))
                throw new OptionException("No new file was specified", "new-file");

            newFilePath = Path.GetFullPath(newFilePath);
            signatureFilePath = Path.GetFullPath(signatureFilePath);

            var delta = new DeltaBuilder();
            foreach (var config in configuration) config(delta);

            if (!File.Exists(signatureFilePath))
            {
                throw new FileNotFoundException("File not found: " + signatureFilePath, signatureFilePath);
            }

            if (!File.Exists(newFilePath))
            {
                throw new FileNotFoundException("File not found: " + newFilePath, newFilePath);
            }

			if (StringOperators.IsNullOrWhiteSpace(deltaFilePath))
            {
                deltaFilePath = newFilePath + ".octodelta";
            }
            else
            {
                deltaFilePath = Path.GetFullPath(deltaFilePath);
                var directory = Path.GetDirectoryName(deltaFilePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            using (var newFileStream = new FileStream(newFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var signatureStream = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var deltaStream = new FileStream(deltaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                delta.BuildDelta(newFileStream, new SignatureReader(signatureStream, delta.ProgressReporter), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
            }

            return 0;
        }
    }
}
