using Plossum.CommandLine;

namespace SourceCopyrightEnforcer
{
    [CommandLineManager( ApplicationName = "copyright",
        Copyright = "Copyright 2013 EventBooking.com, LLC" )]
    internal class Options
    {
        [CommandLineOption( Description = "Displays this help text" )] public bool Help;
        [CommandLineOption( Description = "Root path of the project" )] public string Path;
    }
}