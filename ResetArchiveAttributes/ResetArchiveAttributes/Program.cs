using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ResetArchiveAttributes
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ResetArchiveAttributes    -   Oliver Abraham 2014");
            if (args.GetLength(0) < 1)
            {
                Console.WriteLine("Verwendung: ResetArchiveAttributes {verzeichnisname}");
                return;
            }

            Console.WriteLine("Suche Dateien in Verzeichnis {0} ...", args[0]);
            string[] Dateien = Directory.GetFiles(args[0], "*", SearchOption.AllDirectories);

            UInt64 Zurückgesetzt = 0;
            Console.WriteLine("Setze Archiv-Informationen zurück...");
            foreach (string Datei in Dateien)
            {
                try
                {
                    FileAttributes Fa = File.GetAttributes(Datei);
                    if ((Fa & FileAttributes.Archive) != 0)
                    {
                        Fa = Fa & (~FileAttributes.Archive);  // FileAttributes.Archive löschen
                        File.SetAttributes(Datei, Fa);
                        Zurückgesetzt++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler bei Verarbeitung von Datei '{0}':\n{1}\n\n", 
                        Datei, ex.ToString());
                }
            }
            
            Console.WriteLine("Fertig.  {0} Dateien verarbeitet, bei {1} Dateien das Archivbit zurückgesetzt.", 
                Dateien.GetLength(0), Zurückgesetzt);
        }
    }
}
