namespace Octodiff.CommandLine.Support
{
    interface ICommandMetadata
    {
        string Name { get; }
        string[] Aliases { get; }
        string Description { get; }
        string Usage { get; set; }
    }
}