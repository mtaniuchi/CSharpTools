using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LineCount
{
    class Program
    {
        private const int ExitSuccess = 0;
        private const int ExitFailure = -1;

        static int Main(string[] args)
        {
            if (!args.Any() || args.Count() > 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("\tLineCount.exe [dirPath]");
                Console.WriteLine("\tLineCount.exe [dirPath] [searchPattern]");
                Console.WriteLine("Example:");
                Console.WriteLine("\tLineCount.exe C:\\path\\to\\dir");
                Console.WriteLine("\tLineCount.exe C:\\path\\to\\dir *.exe");
                return ExitSuccess;
            }

            var path = args[0];
            if (!Directory.Exists(path))
            {
                Console.WriteLine("path doesn't exist.");
                return ExitFailure;
            }

            var searchPattern = args.Count().Equals(2) ? args[1] : "*.*";
            foreach (var directory in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                foreach (var file in Directory.GetFiles(directory, searchPattern))
                {
                    long count = 0;
                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var fileSize = (int)fs.Length;
                        var buf = new byte[fileSize];

                        fs.Read(buf, 0, fileSize);

                        byte before = 0x0;
                        foreach (var b in buf)
                        {
                            // CR+LF?
                            if (before == 0x0D && b == 0x0A)
                            {
                                count++;
                            }
                            before = b;
                        }
                    }
                    Console.WriteLine("{0}:{1}", file, count);
                }
            }

            return ExitSuccess;
        }
    }
}
