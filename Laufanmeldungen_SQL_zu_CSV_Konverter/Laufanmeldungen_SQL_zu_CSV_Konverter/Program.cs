using Abraham.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laufanmeldungen_SQL_zu_CSV_Konverter
{
    class Program
    {
        private const string File_in_zipfile_containing_the_data = @"./anmeldungen.sql";
        private const string TempFile = "temp.sql";

        static void Main(string[] args)
        {
            if (args.Count() == 0)
            {
                Usage();
                return;
            }

            string InputFolder = args[0];
            string OutputFilename = args[1];

            if (!Directory.Exists(InputFolder))
            {
                Console.WriteLine("Angegebenes Quellverzeichnis existiert nicht!");
                return;
            }

            string NewestFile = Find_newest_file_in_folder(InputFolder);
            if (string.IsNullOrWhiteSpace(NewestFile))
            {
                Console.WriteLine("Die neueste Datei konnte nicht ermittelt werden!");
                return;
            }

            string TempFile = Extract_file_from_zip_archive(NewestFile);
            if (string.IsNullOrWhiteSpace(TempFile))
                return;

            Convert_file_to_csv(TempFile, OutputFilename);
        }

        private static void Usage()
        {
            Console.WriteLine("Laufanmeldungen_SQL_zu_CSV_Konverter - Version 1");
            Console.WriteLine("Usage:");
            Console.WriteLine("Laufanmeldungen_SQL_zu_CSV_Konverter [Quellverzeichnis] [Ausgabedatei]");
        }

        private static string Find_newest_file_in_folder(string inputFolder)
        {
            var InputFiles = Directory.EnumerateFiles(inputFolder, "*", SearchOption.AllDirectories);
            if (InputFiles == null || InputFiles.Count() == 0)
            {
                Console.WriteLine("Keine Dateien im Quellverzeichnis vorhanden!");
                return null;
            }

            DateTime MaxWriteTime = default(DateTime);
            foreach (var file in InputFiles)
            {
                DateTime FileInfo = File.GetLastWriteTime(file);
                if (MaxWriteTime < FileInfo)
                    MaxWriteTime = FileInfo;
            }

            foreach (var file in InputFiles)
            {
                DateTime FileInfo = File.GetLastWriteTime(file);
                if (MaxWriteTime == FileInfo)
                    return file;
            }
            return null;
        }

        private static string Extract_file_from_zip_archive(string NewestFile)
        {
            string FileContents = "";
            using (var zip = ZipArchive.OpenOnFile(NewestFile))
            {
                var file = zip.GetFile(File_in_zipfile_containing_the_data);
                if (!file.FolderFlag)
                {
                    FileContents = new StreamReader(file.GetStream(), Encoding.UTF8).ReadToEnd();
                }
            }
            if (string.IsNullOrWhiteSpace(FileContents))
            {
                Console.WriteLine($"Das Archiv '{NewestFile}' enthält keine Datei Names '{File_in_zipfile_containing_the_data}'!");
                return null;
            }
            Console.WriteLine($"Backupfile '{File_in_zipfile_containing_the_data}' aus Archiv '{NewestFile}' gelesen");
            File.WriteAllText(TempFile, FileContents, Encoding.UTF8);
            return TempFile;
        }

        private static void Convert_file_to_csv(string inputFilename, string outputFilename)
        {
            FormatConverter Converter = new FormatConverter();

            List<SqlRow> InputRows = Converter.Read_input_file(inputFilename);

            var OutputRows = Converter.Convert_all_rows(InputRows);

            Converter.Write_output_file(outputFilename, OutputRows);

            Console.WriteLine($"File successfully converted and saved as '{outputFilename}'.");
        }
    }
}
