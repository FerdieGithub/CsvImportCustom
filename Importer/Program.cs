using System;
using System.IO;
using Importer.CustomImporters;
using Importer.Interfaces;

namespace Importer
{
    class Program
    {
        private const string Usage = "Importer.exe CsvFileAndPath OutputPath";

        static int Main(string[] args)
        {
            if (args == null ||
                args.Length == 0 ||
                args[0].Trim('/', '-', '\\') == "?" ||
                args.Length != 2)
            {
                Console.WriteLine(Usage);
                return 10;
            }

            return Run(args[0], args[1]);
        }

        private static int Run(string csvFile, string outputPath)
        {
            try
            {
                ICustomImporter customImport = new CustomImport(csvFile, outputPath);

                if (!Directory.Exists(outputPath))
                {
                    Console.WriteLine($"Output Path '{outputPath}' not found");
                    return 7;
                }

                customImport.ImportData();
                customImport.ProcessOutput();
                return 0; //success for command line app
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"CSV File '{csvFile}' not found");
                return 2;
            }
        }
    }
}
