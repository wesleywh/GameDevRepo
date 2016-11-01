using System;
using System.Linq;

namespace Octodiff.CommandLine.Support
{
    class CommandLocator : ICommandLocator
    {
        public ICommandMetadata[] List()
        {
            return
                (from t in typeof (CommandLocator).Assembly.GetTypes()
                 where typeof (ICommand).IsAssignableFrom(t)
                 let attribute = (ICommandMetadata) t.GetCustomAttributes(typeof (CommandAttribute), true).FirstOrDefault()
                 where attribute != null
                 select attribute).ToArray();
        }

        public ICommandMetadata Find(string name)
        {
            name = name.Trim().ToLowerInvariant();
            return (from t in typeof (CommandLocator).Assembly.GetTypes()
                    where typeof (ICommand).IsAssignableFrom(t)
                    let attribute = (ICommandMetadata) t.GetCustomAttributes(typeof (CommandAttribute), true).FirstOrDefault()
                    where attribute != null
                    where attribute.Name == name || attribute.Aliases.Any(a => a == name)
                    select attribute).FirstOrDefault();
        }

        public ICommand Create(ICommandMetadata metadata)
        {
            var name = metadata.Name;
            var found = (from t in typeof(CommandLocator).Assembly.GetTypes()
                         where typeof(ICommand).IsAssignableFrom(t)
                         let attribute = (ICommandMetadata)t.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault()
                         where attribute != null
                         where attribute.Name == name || attribute.Aliases.Any(a => a == name)
                         select t).FirstOrDefault();

            return found == null ? null : (ICommand)Activator.CreateInstance(found);
        }
    }
}