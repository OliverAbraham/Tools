using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace ResetArchiveAttributes
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ResetArchiveAttributes    -   Oliver Abraham 2023");
            if (args.GetLength(0) < 1)
            {
                Console.WriteLine("Verwendung: ResetArchiveAttributes {verzeichnisname}");
                return;
            }

            Console.WriteLine($"Suche Dateien in Verzeichnis {args[0]} ...");
            List<string> alleDateien = Directory.GetFiles(args[0], "*", SearchOption.AllDirectories).ToList();
            Console.WriteLine($"Dateien gesamt: {alleDateien.Count}");
            Console.WriteLine($"Suche nach Dateien mit Archivbit...");

            var dateienMitArchivbit = new List<string>();
            Parallel.ForEach (alleDateien, delegate(string datei)
                {
                    try
                    {
                        FileAttributes Fa = File.GetAttributes(datei);
                        if ((Fa & FileAttributes.Archive) != 0)
                            dateienMitArchivbit.Add(datei);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0,50}':{1}\n", datei, ex.ToString().Replace("\n", "").Replace("\r", ""));
                    }
                });
            Console.WriteLine($"Dateien mit Archivbit: {dateienMitArchivbit.Count}");
            

            Console.WriteLine($"Loesche Archiv-Informationen...");
            UInt64 zähler = 0;
            foreach (string datei in dateienMitArchivbit)
            {
                try
                {
                    FileAttributes Fa = File.GetAttributes(datei);
                    Fa = Fa & (~FileAttributes.Archive);  // FileAttributes.Archive löschen
                    File.SetAttributes(datei, Fa);
                    zähler++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0,50}':{1}", datei, ex.Message.Replace("\n", "").Replace("\r", ""));
                }
            }
            
            Console.WriteLine($"Fertig.  {alleDateien.Count} Dateien verarbeitet, bei {zähler} Dateien das Archivbit zurückgesetzt.");
        }
    }
}
