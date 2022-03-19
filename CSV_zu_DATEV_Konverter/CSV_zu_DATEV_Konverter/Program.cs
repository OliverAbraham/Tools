using System;
using System.Collections.Generic;
using System.IO;

namespace CSV_zu_DATEV_Konverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CSV_zu_DATEV_Konverter - Version 1");
            Console.WriteLine("Usage:");
            Console.WriteLine("CSV_zu_DATEV_Konverter [eingabedatei] [ausgabedatei]");

            string InputFilename  = args[0];
            string OutputFilename = args[1];

            FormatConverter Converter = new FormatConverter();

            List<CsvRow> InputRows = Converter.Read_input_file(InputFilename);

            var OutputRows = Converter.Convert_all_rows(InputRows);

            Converter.Write_output_file(OutputFilename, OutputRows);

            Console.WriteLine("File successfully converted.");
        }
    }
}
