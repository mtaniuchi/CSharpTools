using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ShowFileVersionInfo
{
    /// <summary>
    /// Program
    /// </summary>
    /// <remarks>icon: http://www.icons-land.com/ </remarks>
    class Program
    {
        private const int ExitSuccess = 0;
        private const int ExitFailure = -1;

        static int Main(string[] args)
        {
            if (!args.Any() || args.Count() > 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("\tShowFileVersionInfo.exe [dirPath]");
                Console.WriteLine("\tShowFileVersionInfo.exe [dirPath] [searchPattern]");
                Console.WriteLine("Example:");
                Console.WriteLine("\tShowFileVersionInfo.exe C:\\path\\to\\dir");
                Console.WriteLine("\tShowFileVersionInfo.exe C:\\path\\to\\dir *.exe");
                return ExitSuccess;
            }

            var path = args[0];
            if (!Directory.Exists(path))
            {
                Console.WriteLine("path doesn't exist.");
                return ExitFailure;
            }

            // header
            var t = typeof (FileVersionInfo);
            var propInfos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            foreach (var pInfo in propInfos)
            {
                Console.Write("{0}\t", pInfo.Name);
            }
            Console.Write("CreationTime\t");
            Console.Write("LastWriteTime\t");
            Console.Write("LastAccessTime\t");
            Console.Write(Environment.NewLine);

            // info
            var filters = args.Count().Equals(2) ? args[1] : "*.*";
            foreach (var file in GetFiles(path, filters, SearchOption.AllDirectories))
            {
                var fileInfo = FileVersionInfo.GetVersionInfo(file);
                foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    PropertyInfo property = t.GetProperty(p.Name);
                    var value = property.GetValue(fileInfo, null);
                    Console.Write("{0}\t", value);
                }
                Console.Write("{0}\t", File.GetCreationTime(file));
                Console.Write("{0}\t", File.GetLastWriteTime(file));
                Console.Write("{0}\t", File.GetLastAccessTime(file));
                Console.Write(Environment.NewLine);
            }

            return ExitSuccess;
        }

        private static string[] GetFiles(string sourceFolder, string filters, SearchOption searchOption)
        {
            return filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        }
    }
}
