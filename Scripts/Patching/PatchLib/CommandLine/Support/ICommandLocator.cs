namespace Octodiff.CommandLine.Support
{
    interface ICommandLocator
    {
        ICommandMetadata[] List();
        ICommandMetadata Find(string name);
        ICommand Create(ICommandMetadata metadata);
    }
}