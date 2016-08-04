using System;
using Plossum.CommandLine;

namespace SourceCopyrightEnforcer
{
	internal class Program
	{
		private static void Main()
		{
			var options = new Options();
			var parser = new CommandLineParser(options);
			parser.Parse();

			if( string.IsNullOrWhiteSpace(options.Path))
				throw new Exception("Path is required");

			var scan = new Scanner(options.Path);
			scan.Run();
		}
	}
}