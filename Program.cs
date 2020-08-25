using System;
using System.Collections.Generic;
using System.IO;

namespace copyOverSameFileNames
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Copy over same file names");
            if (args.Length != 2) {
                Console.WriteLine("Usage:\r\ncopyOverSameFileNames source-file-path dest-folder");
                return;
            }
            var srcFn = Path.GetFullPath(args[0]);
            // fix casing
            var srcFolder = Path.GetDirectoryName(srcFn);
            var srcFixCase = Directory.GetFiles(srcFolder, Path.GetFileName(srcFn))[0];
            srcFn = srcFixCase;
            if (!File.Exists(srcFn)) {
                Console.WriteLine("Missing file " + srcFn);
                return;
            }

            if (!Directory.Exists(args[1])) {
                Console.WriteLine("Missing folder " + args[1]);
                return;
            }

            
            Console.WriteLine($"searching for {Path.GetFileName(srcFn)}...");
            foreach (var destFileName in findFiles(Path.GetFileName(srcFn), args[1])) {
                if (srcFn.Equals(destFileName, StringComparison.InvariantCultureIgnoreCase)) continue;
                Console.Write("Writing over " + destFileName);
                var fi = new FileInfo(destFileName);
                if (fi.Attributes.HasFlag(FileAttributes.ReadOnly) || fi.Attributes.HasFlag(FileAttributes.System)) {
                    fi.Attributes &= ~(FileAttributes.ReadOnly | FileAttributes.System);
                }
                File.Move(destFileName, destFileName + ".backup");
                Console.Write(".");
                File.Copy(args[0], destFileName);
                Console.Write(".");
                File.Delete(destFileName + ".backup");
                Console.WriteLine(".\t\tOK");
            }

            Console.WriteLine("Complete");
        }

        static IEnumerable<string> findFiles(string fn, string folder)
        {
            foreach(var subFolder in Directory.EnumerateDirectories(folder))
            foreach (var foundFile in findFiles(fn, subFolder))
                yield return foundFile;
            foreach (var foundFile in Directory.EnumerateFiles(folder, fn))
                yield return foundFile;
        }
    }
}
