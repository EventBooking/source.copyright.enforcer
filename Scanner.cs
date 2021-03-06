﻿using System;
using System.IO;

namespace SourceCopyrightEnforcer
{
    public class Scanner
    {
        private readonly string _rootPath;

        public Scanner( string rootPath )
        {
            _rootPath = rootPath;
        }

        public void Run()
        {
            var dirInfo = new DirectoryInfo( _rootPath );
            if (dirInfo.Exists)
            {
                ScanDirectory( dirInfo );
            }
        }

        private static void ScanDirectory( DirectoryInfo dirInfo )
        {
            if (dirInfo.Name == "obj"
                || dirInfo.Name == "bin"
                || dirInfo.Name == "packages"
                || dirInfo.Name == "generated"
                || dirInfo.Name == "_generated"
                || dirInfo.Name.StartsWith( "." ))
                return;

            try
            {
                foreach (var file in dirInfo.EnumerateFiles())
                {
                    if (file.Extension != ".cs"
                        || file.FullName.EndsWith( ".designer.cs" )
                        || file.Length < 16)
                        continue;

                    Console.WriteLine( file.FullName );
                    var fu = new CSharpFileFixer( file );
                    if (fu.NeedsFix)
                        fu.Fix();
                }

                foreach (var dir in dirInfo.EnumerateDirectories())
                {
                    ScanDirectory( dir );
                }
            }
            catch (PathTooLongException)
            {
                Console.WriteLine( "Path was too long. Continuing." );
            }
        }
    }
}