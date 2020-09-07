using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;

namespace ItsaMultiCopy
{
    class Program
    {
        [Value(0, HelpText = "The source directory.", Required = true)]
        public string Source { get; set; }

        [Value(1, HelpText = "The destination directory.", Required = true)]
        public string Destination { get; set; }

        [Option('n', "no-recurse", HelpText = "If specified, will not recurse. (Subdirectories of the source directory will not be copied.)")]
        public bool NoRecurse { get; set; }

        [Option('d', "data", HelpText = "If specified, will only copy the binary file data and no ACLs or alternate streams.")]
        public bool DataLevelCopy { get; set; }

        [Option('c', "changed", HelpText = "If specified, will not copy files with a older last-modified time.")]
        public bool ChangedOnly { get; set; }

        [Option('v', "verbose", HelpText = "If specified, will print the name of each file copied.")]
        public bool Verbose { get; set; }

        [Option('m', "matches", HelpText = "If specified, will only copy files matching the glob pattern.")]
        public string Matches { get; set; } = "*";

        [Option('M', "subdir-matches", HelpText = "If specified, will only copy subdirectories matching the glob pattern.")]
        public string SubdirectoryMatches { get; set; } = "*";

        static int Main(string[] args)
        {
            int returnCode = 2;
            var parsedArgs = Parser.Default.ParseArguments<Program>(args);
            parsedArgs.WithParsed(a => returnCode = a.Main2());
            return returnCode;
        }

        int Main2()
        {
            try
            {
                if (!Directory.Exists(Source)) throw new Exception("Source directory doesn't exist.");
                CopyDirectory(Source, Destination);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ItsaMultiCopy: " + ex.Message);
                return 1;
            }
        }

        void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (string file in Directory.GetFiles(source, Matches))
            {
                CopyFile(file, Path.Combine(destination, Path.GetFileName(file)));
            }

            if (!NoRecurse)
            {
                foreach (string subdirectory in Directory.GetDirectories(source, SubdirectoryMatches))
                {
                    CopyDirectory(subdirectory, Path.Combine(destination, Path.GetFileName(subdirectory)));
                }
            }
        }

        void CopyFile(string source, string destination)
        {
            if (ChangedOnly && File.Exists(destination))
            {
                var sourceInfo = new FileInfo(source);
                var destInfo = new FileInfo(destination);
                if (destInfo.LastWriteTimeUtc >= sourceInfo.LastWriteTimeUtc)
                {
                    return;
                }
            }

            if (Verbose) Console.WriteLine(string.Concat(source, " -> ", destination));
            if (DataLevelCopy)
            {
                if (File.Exists(destination)) File.Delete(destination);

                // the using block is used here becuase we need to close destStream
                // before overriding the last modification time
                using (var sourceStream = File.Open(source, FileMode.Open))
                {
                    using var destStream = File.Open(destination, FileMode.Create);
                    sourceStream.CopyTo(destStream);
                }

                var sourceInfo = new FileInfo(source);
                var destInfo = new FileInfo(destination);
                destInfo.LastWriteTimeUtc = sourceInfo.LastWriteTimeUtc;
            }
            else
            {
                File.Copy(source, destination, true);
            }
        }
    }
}
