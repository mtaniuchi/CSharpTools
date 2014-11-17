using System;
using System.IO;

namespace FiDiCleaner
{
    class Program
    {
        // ReSharper disable InconsistentNaming
        private const int EXIT_SUCCESS = 0;
        private const int EXIT_FAILURE = -1;
        // ReSharper restore InconsistentNaming

        static int Main(string[] args)
        {
            // parse args
            if (args.Length < 2)
            {
                Console.WriteLine("*************************************************************");
                Console.WriteLine(" This tool will remove specified all object without confirm.");
                Console.WriteLine(" Please pay close attention!!!");
                Console.WriteLine("*************************************************************");
                Console.WriteLine("[Usage] targetDir days [option]");
                Console.WriteLine("\ttargetDir:\ttarget directory");
                Console.WriteLine("\tdays:\terase file it's older than days");
                Console.WriteLine("\tdays:\t(0 day means delete all!)");
                return EXIT_SUCCESS;
            }

            var path = args[0];
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Path is not exists: {0}", path);
                return EXIT_FAILURE;
            }

            int days;
            if (!int.TryParse(args[1], out days) || days < 0)
            {
                Console.WriteLine("Invalid days specified: {0}", args[3]);
                return EXIT_FAILURE;
            }

            // delete files
            var d = DateTime.Now.AddDays(-days);
            foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                if (File.GetLastWriteTime(file) < d)
                {
                    File.Delete(file);
                    Console.WriteLine("deleted:{0}", file);
                }
                else
                {
                    Console.WriteLine("not deleted:{0}", file);
                }
            }

            // delete empty directories
            foreach (var directory in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    // deleting failed if files exist in directory...
                    Directory.Delete(directory);
                    Console.WriteLine("deleted:{0}", directory);
                }
                catch
                {
                    Console.WriteLine("not deleted:{0}", directory);
                }
            }

            return EXIT_SUCCESS;
        }
    }
}
