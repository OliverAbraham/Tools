 //-----------------------------------------------------------------------------
//
//                          ARCHIVÜBERWACHUNGSPROGRAMM
//
//                                Oliver Abraham
//                               Abraham Beratung
//                            mail@oliver-abraham.de
//                             www.oliver-abraham.de
//
//
//  Das Programm überwacht Dateien eines Dateiarchivs, indem es für alle 
//  Dateien eine Prüfsumme berechnet und speichert. 
//  Beim nächsten Programmlauf prüft es, ob Dateien inhaltlich verändert, 
//  umbenannt, verschoben oder gelöscht wurden. 
//  
//
//  Das Programm benutzt eine MySQL-Datenbank mit zwei Tabellen.
//  In der einen Tabelle legt es die Dateiprüfsummen ab, in der zweiten
//  Tabelle speichert es die Änderungen.
//
//  Neuerstellung in 8/2012
//-----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Threading;
using Inveos.DateTimeExtensions;
using MySql.Data.MySqlClient;

namespace ChecksumFiles
{
    class Program
    {
        static string Version = "04.12.2013";

        #region ------------- Variablen -----------------------------------------------------------

        static bool Parallelverarbeitung = false;

        struct Parameter
        {
            public string ConnectionString;
            public string Path;
            public string Notification_title;
            public string Notification_from;
            public string Notification_to;
            public bool   IncludeFileListsInEmail;
            //public string Filename_Changedfiles;
            public bool   GenerateFileLists;
            public string Protokolldateiname;
            public DateTime Startzeit;

            public string SmtpServer;
            public string SmtpUser;
            public string SmtpPassword;
            public int    SmtpPort;
            public bool   SmtpEnableSSL;
            public int    SmtpTimeout;
        };
        static Parameter Par;

        static string[] AlleDateinamen;
        static int GesamtanzahlDateien;
        static int Dateinummer;

        struct Ergebnisse
        {
            public int               AnzahlAlleDateien;
            public int               AnzahlUnveränderte;
            public int               AnzahlMitArchivAttribut;
            public Int64             SummeDateigrößen;
            public Int64             SummeDateigrößenArchivAttribut;
            public List<Änderung>    Änderungen;
        };
        static Ergebnisse Erg;

        static Stoppuhr Gesamtlaufzeit;
        static Stoppuhr Verarbeitungszeit;
        static Fortschrittsanzeige Fortschritt;

        static bool LöschungDesArchivbitsIgnorieren = true;
        static bool KeineExistsWeilDatenbankLeer = false;
        #endregion



        #region ------------- Hauptprogramm -------------------------------------------------------

        static void Main(string[] args)
        {
            Gesamtlaufzeit = new Stoppuhr();
            Fortschritt = new Fortschrittsanzeige();
            Parameter_aus_Configdatei_holen();

            Par.Startzeit = DateTime.Now;
            Par.Protokolldateiname = Par.Startzeit.ToString("yyyy-MM-dd HH.mm.ss") + ".log";

            if (!Datenbank_connect_mit_Fehlerbehandlung())
                return;

            if (!PrüfenObDieTabellenExistieren_mit_Fehlerbehandlung())
                return;

            if (!PrüfenObDieTabellenLeerSind_mit_Fehlerbehandlung())
                return;

            if (!Alle_Dateien_zusammensuchen())
                return;

            Prüfsummenberechnung();     //   <--------- Hauptprogramm

            Db.Disconnect();

            Email_senden_mit_Fehlerbehandlung();

            System.Console          .WriteLine("Programmlaufzeit: {0}", Gesamtlaufzeit.Zeit);
            System.Diagnostics.Debug.WriteLine("Programmlaufzeit: {0}", Gesamtlaufzeit.Zeit);
        }

        static bool Datenbank_connect_mit_Fehlerbehandlung()
        {
            try
            {
                System.Console.WriteLine("Verbinden mit Datenbank");
                Datenbank_connect(Par.ConnectionString);
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Connect fehlgeschlagen. Programm wird beendet. Mehr Info: " + ex.ToString());
                return false;
            }
        }

        static bool PrüfenObDieTabellenExistieren_mit_Fehlerbehandlung()
        {
            try
            {
                PrüfenObDieTabellenExistieren();
                return true;
            }
            catch (Exception)
            {
                System.Console.WriteLine("Die benötigten Tabellen existieren nicht in der Datenbank. Ggf. auch den ConnectionString überprüfen");
                return false;
            }
        }

        static bool PrüfenObDieTabellenLeerSind_mit_Fehlerbehandlung()
        {
            try
            {
                PrüfenObDieTabellenLeerSind();
                return true;
            }
            catch (Exception)
            {
                System.Console.WriteLine("Die benötigten Tabellen existieren nicht in der Datenbank. Ggf. auch den ConnectionString überprüfen");
                return false;
            }
        }

        static void Parameter_aus_Configdatei_holen()
        {
            Par.ConnectionString        = Settings.Default.ConnectionString;
            Par.Path                    = Settings.Default.DirectoryPath.ToString();
            Par.SmtpServer              = Settings.Default.SmtpServer;
            Par.SmtpUser                = Settings.Default.SmtpUser;
            Par.SmtpPassword            = Settings.Default.SmtpPassword;
            Par.SmtpPort                = Settings.Default.SmtpPort;
            Par.SmtpEnableSSL           = Settings.Default.SmtpEnableSSL;
            Par.SmtpTimeout             = Settings.Default.SmtpTimeout;
            Par.Notification_title      = Settings.Default.NotificationTitle.ToString();
            Par.Notification_from       = Settings.Default.NotificationFrom.ToString();
            Par.Notification_to         = Settings.Default.NotificationTo.ToString();
            Par.GenerateFileLists       = Settings.Default.GenerateFileLists;
            Par.IncludeFileListsInEmail = Settings.Default.IncludeFileListsInEmail;
            //Par.Filename_Changedfiles   = Settings.Default.ChangedFiles.ToString();
        }

        static bool Alle_Dateien_zusammensuchen()
        {
            Stoppuhr Uhr = new Stoppuhr();
            try
            {
                System.Console.WriteLine("Alle Dateien suchen in Speicherort '{0}'...", Par.Path);
                AlleDateinamen = Directory.GetFiles(Par.Path, "*.*", SearchOption.AllDirectories);
                GesamtanzahlDateien = AlleDateinamen.Count();
                Dateinummer = 0;
                System.Console.WriteLine("Abgeschlossen, {0} Dateien", GesamtanzahlDateien);
                System.Console.WriteLine("Dauer {0}", Uhr.MinutenSekunden);
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Alle Dateien suchen fehlgeschlagen. Mehr Info: " + ex.ToString());
                return false;
            }
        }

        static void Prüfsummenberechnung()      //  <-------------- Hauptprogramm
        {
            System.Console.WriteLine("Prüfsummenberechnung beginnt...");
            if (KeineExistsWeilDatenbankLeer)
                System.Console.WriteLine("Optimierung eingeschaltet, weil der Datenbestand noch leer ist...");

            Verarbeitungszeit = new Stoppuhr();
            Fortschritt.Init(AlleDateinamen.Count());
            try
            {
                DateTime Timestamp = DateTime.Now;
                Erg.AnzahlUnveränderte              = 0;
                Erg.AnzahlMitArchivAttribut         = 0;
                Erg.SummeDateigrößen                = 0;
                Erg.SummeDateigrößenArchivAttribut  = 0;
                Erg.Änderungen                      = new List<Änderung>();


                System.Console.WriteLine("Setze_Gelöscht_Kennzeichen_bei_allen_Datensätzen...");
                Setze_Gelöscht_Kennzeichen_bei_allen_Datensätzen();


                if (Parallelverarbeitung)
                {
                    System.Console.WriteLine("Parallele Berechnung der Prüfsummen für alle Dateien...");
                    Parallel.ForEach(AlleDateinamen,
                        Dateiname =>
                        {
                            Prüfsummenberechnung_eine_Datei(Dateiname, Timestamp);
                            Fortschritt.Print();
                        });
                }
                else
                {
                    System.Console.WriteLine("Sequentielle Berechnung der Prüfsummen für alle Dateien...");
                    foreach(var Dateiname in AlleDateinamen)
                    {
                        Prüfsummenberechnung_eine_Datei(Dateiname, Timestamp);
                        Fortschritt.Print();
                    }
                }
                System.Console.WriteLine("abgeschlossen.");


                System.Console.WriteLine("AnzahlAlleDateien Dateien berechnen...");
                Erg.AnzahlAlleDateien = Prüfsummentabelle_GetCount();               
                System.Console.WriteLine("abgeschlossen.");


                System.Console.WriteLine("Gelöschte Dateien suchen...");
                Suche_Datensätze_mit_Gelöscht_Kennzeichen();               
                Setze_alle_neuen_Datensätze_auf_verarbeitet();
                System.Console.WriteLine("abgeschlossen.");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Prüfsummenberechnung fehlgeschlagen. Mehr Info: " + ex.ToString());
                throw;
            }
        }

        static void Prüfsummenberechnung_eine_Datei(string Pfad, DateTime Timestamp)
        {
            #region --------- Vorbereitung --------------------------
            Fortschrittsanzeige(Pfad);

            FileInfo Info = new FileInfo(Pfad.Replace(@"\", @"\\"));
            if (Info.Exists == false)
            {
                System.Console.WriteLine(Kommentar_Datei_zwischenzeitlich_verschwunden(Pfad));
                return;
            }

            Pfad = UngültigeUnicodezeichenUmwandeln(Pfad);
            ChecksumRow Neu = new ChecksumRow(Pfad, Info);
            Ungültiges_Dateidatum_korrigieren(Neu);
            #endregion


            #region --------- Prüfsumme der Datei bilden ------------
            try
            {
                //Neu.SHA256 = "TEST";
                Neu.SHA256 = ChecksumGenerator.GetChecksum(Pfad).ToString();
                System.Diagnostics.Debug.WriteLine("{0,-2}   {1}", Thread.CurrentThread.ManagedThreadId, Pfad);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Fehler bei der Verarbeitung der Datei '{0}'", Neu.FILEPATH);
                System.Console.WriteLine("Prüfsummenbildung fehlgeschlagen");
                System.Console.WriteLine("Info: {0}.", ex.ToString());
                return;
            }
            #endregion


            #region --------- Datei in der Datenbank finden ---------
            ChecksumRow Alt = null;
            try
            {
                if (!KeineExistsWeilDatenbankLeer)
                {
                    Alt = LeseSatz(Neu.FILEPATH);
                    if (Alt == null)
                    {
                        //Alt = null;
                        Alt = LeseSatzAnhandPrüfsumme(Neu.SHA256);
                        // Haben wir evtl. eine Dublette ? Falls die ALT-Datei noch existiert, ist Neu eine Dublette.
                        // wir tun dann so, als hätten wir eben keine Datei gefunden
                        if (Alt != null && File.Exists(Alt.FILEPATH))
                            Alt = null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Fehler bei der Verarbeitung der Datei '{0}'", Neu.FILEPATH);
                System.Console.WriteLine("Info: {0}.", ex.ToString());
                System.Console.WriteLine("SQL:\n\r'{0}'", Db.LetzteSqlAnweisung);
                return;
            }
            #endregion


            #region --------- Auswertung ----------------------------
            try
            {
                if (Alt == null)
                {
                    Neu.ID = 0;
                    Prüfsummentabelle_Insert(Neu);
                    Änderungstabelle_Insert(Neu, Neu.MOD_DATE, Änderungsart.Neu, "Neu");
                }
                else
                {
                    string PfadNeu = Path.GetDirectoryName(Neu.FILEPATH);
                    string PfadAlt = Path.GetDirectoryName(Alt.FILEPATH);
                    Neu.ID = Alt.ID;
                    
                    bool Änderung = Änderungen_erkennen(Timestamp, Neu, Alt, PfadNeu, PfadAlt);
                    if (!Änderung)
                        Erg.AnzahlUnveränderte++;

                    Erg.SummeDateigrößen += Neu.FILESIZE;
                    if (Neu.ATTRIBUTES.Contains("A"))
                    {
                        Erg.AnzahlMitArchivAttribut++;
                        Erg.SummeDateigrößenArchivAttribut += Neu.FILESIZE;
                    }

                    Alt.DELETED = 'N';
                    Prüfsummentabelle_Update(Neu);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Fehler bei der Verarbeitung der Datei '{0}'", Neu.FILEPATH);
                System.Console.WriteLine("Info: {0}.", ex.ToString());
                System.Console.WriteLine("SQL:\n\r'{0}'", Db.LetzteSqlAnweisung);
            }
            #endregion
        }

        static string UngültigeUnicodezeichenUmwandeln(string eingabe)
        {
            string Ausgabe = "";
            for (int i=0; i<eingabe.Length; i++)
            {
                if (i<eingabe.Length-1 && (int)eingabe[i + 1] == 776)
                {
                    if      (eingabe[i] == 'a') { Ausgabe += 'ä'; i++; }
                    else if (eingabe[i] == 'o') { Ausgabe += 'ö'; i++; }
                    else if (eingabe[i] == 'u') { Ausgabe += 'ü'; i++; }
                    else if (eingabe[i] == 'A') { Ausgabe += 'Ä'; i++; }
                    else if (eingabe[i] == 'O') { Ausgabe += 'Ö'; i++; }
                    else if (eingabe[i] == 'U') { Ausgabe += 'Ü'; i++; }
                }
                else
                    Ausgabe += eingabe[i];
            }
            return Ausgabe;
        }

        static void Ungültiges_Dateidatum_korrigieren(ChecksumRow Neu)
        {
            if (Neu.MOD_DATE.Month == 3 && Neu.MOD_DATE.Day == 22 && Neu.MOD_DATE.Hour == 2)
            {
                System.Console.WriteLine("Fehler: Ungültiges Dateidatum wird korrigiert (plus 1 Stunde). Datum: '{0}' Datei: '{1}' ",
                                         Neu.MOD_DATE, Neu.FILEPATH);
                Neu.MOD_DATE.AddHours(1);
            }
        }

        static void Fortschrittsanzeige(string Pfad)
        {
            Dateinummer++;
            double DateienProSekunde = (Verarbeitungszeit.Sekunden > 0) ? ((double)Dateinummer / Verarbeitungszeit.Sekunden) : 0;
            double Prozent = ((double)Dateinummer / GesamtanzahlDateien) * 100;
            System.Console.WriteLine("{0,-6} von {1,-6}   {2:0.0} D/sec   {3:0.0} %   {4,-3}   {5}", 
                Dateinummer, GesamtanzahlDateien,  DateienProSekunde, Prozent,
                Thread.CurrentThread.ManagedThreadId, Pfad);
        }

        static bool Änderungen_erkennen(DateTime Timestamp, ChecksumRow Neu, ChecksumRow Alt, string PfadNeu, string PfadAlt)
        {
            if (Neu.SHA256 != Alt.SHA256)
            {
                Änderungstabelle_Insert(Neu, Timestamp, Änderungsart.Geändert, Kommentar_Inhaltsänderung(Neu, Alt));
                return true;
            }

            if (Neu.MOD_DATE.Year   != Alt.MOD_DATE.Year   ||
                Neu.MOD_DATE.Month  != Alt.MOD_DATE.Month  ||
                Neu.MOD_DATE.Day    != Alt.MOD_DATE.Day    ||
                Neu.MOD_DATE.Hour   != Alt.MOD_DATE.Hour   ||
                Neu.MOD_DATE.Minute != Alt.MOD_DATE.Minute ||
                Neu.MOD_DATE.Second != Alt.MOD_DATE.Second)
            {
                Änderungstabelle_Insert(Neu, Timestamp, Änderungsart.Datumsänderung, Kommentar_Datumsänderung(Neu, Alt));
                return true;
            }

            if (PfadNeu == PfadAlt && Neu.FILENAME != Alt.FILENAME)
            {
                Änderungstabelle_Insert(Neu, Timestamp, Änderungsart.Umbenannt, Kommentar_Umbenennung(Neu.FILENAME, Alt.FILENAME));
                return true;
            }

            if (PfadNeu != PfadAlt)
            {
                Änderungstabelle_Insert(Neu, Timestamp, Änderungsart.Verschoben, Kommentar_Verschiebung(PfadNeu, PfadAlt));
                return true;
            }

            if (Neu.FILESIZE != Alt.FILESIZE)
            {
                Änderungstabelle_Insert(Neu, Timestamp, Änderungsart.Größenänderung, Kommentar_Größenänderung(Neu, Alt));
                return true;
            }

            string AttrNeu = Neu.ATTRIBUTES;
            string AttrAlt = Alt.ATTRIBUTES;
            if (LöschungDesArchivbitsIgnorieren)
            {
                if (!AttrNeu.Contains("A") && AttrAlt.Contains("A"))
                    AttrAlt = AttrAlt.Replace("A", "");
            }
            if (AttrNeu != AttrAlt)
            {
                Änderungstabelle_Insert(Neu, Timestamp, Änderungsart.AttributeGeändert, Kommentar_Attributsänderung(Neu, Alt));
                return true;
            }

            return false;
        }

        static string Kommentar_Datei_zwischenzeitlich_verschwunden(string Pfad)
        {
            return String.Format(
                "Hinweis: Datei existierte zu Beginn der Verarbeitung noch, nun nicht mehr: '{0}'. " +
                " Die Datei wird nicht verarbeitet.", Pfad);
        }

        static string Kommentar_Inhaltsänderung(ChecksumRow Neu, ChecksumRow Alt)
        {
            return String.Format("Inhalt geändert (Größe jetzt {0}, vorher {1}, Prüfsumme jetzt {2}, vorher {3})",
                Neu.FILESIZE, Alt.FILESIZE, Neu.SHA256, Alt.SHA256);
        }

        static string Kommentar_Datumsänderung(ChecksumRow Neu, ChecksumRow Alt)
        {
            return String.Format("Dateidatum geändert, jetzt {0}, vorher {1}", 
                Neu.MOD_DATE, Alt.MOD_DATE);
        }

        static string Kommentar_Umbenennung(string nameNeu, string nameAlt)
        {
            return String.Format("Umbenannt, Name jetzt '{0}', vorher '{1}'", 
                nameNeu, nameAlt);
        }

        static string Kommentar_Verschiebung(string PfadNeu, string PfadAlt)
        {
            return String.Format("Verschoben, " + 
                "\n                        Jetzt  '{0}', " + 
                "\n                        Vorher '{1}'", 
                PfadNeu, PfadAlt);
        }

        static string Kommentar_Größenänderung(ChecksumRow Neu, ChecksumRow Alt)
        {
            return String.Format("Dateigröße geändert, Größe jetzt {0}, vorher {1}, Prüfsumme jetzt {2}, vorher {3}",
                Neu.FILESIZE, Alt.FILESIZE, Neu.SHA256, Alt.SHA256);
        }

        static string Kommentar_Attributsänderung(ChecksumRow Neu, ChecksumRow Alt)
        {
            return String.Format("Attribute geändert, jetzt '{0}', vorher '{1}'", 
                Neu.ATTRIBUTES, Alt.ATTRIBUTES);
        }

        static void Änderungstabelle_Insert(ChecksumRow neu, DateTime date, Änderungsart art, string action)
        {
            neu.Action = action;
            if (neu.ID == 0)
            {
                Erg.Änderungen.Add(new Änderung(neu.FILEPATH, action, art));
            }
            else
            {
                Erg.Änderungen.Add(new Änderung(neu.FILEPATH, action, art));
                try
                {
                    Änderungstabelle_Insert2(neu.ID, date, action);
                }
                catch (MySqlException ex)
                {
                    if (ex.Number != (int)MySqlErrorCode.DuplicateKeyEntry)
                        throw;
                }
            }
        }

        #endregion


        
        #region ------------- Datenzugriffsschicht ------------------------------------------------

        static MySqlDatenbankverbindung Db;

        static void Datenbank_connect(string connectionString)
        {
            Db = new MySqlDatenbankverbindung(connectionString);
            Reconnect();
        }

        static void Reconnect()
        {
            try
            {
                System.Console.WriteLine("Connect zur Datenbank...");
                Db.Connect();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Connect fehlgeschlagen. Mehr Info: " + ex.ToString());
                throw;
            }
        }

        static void PrüfenObDieTabellenExistieren()
        {
            Db.SkalarSelect("select count(*) from checksum");
            Db.SkalarSelect("select count(*) from changes");
        }

        static void PrüfenObDieTabellenLeerSind()
        {
            int Anzahl = Db.SkalarSelect("select count(*) from checksum");
            if (Anzahl == 0)
                KeineExistsWeilDatenbankLeer = true;
        }

        static ChecksumRow LeseSatz(string filepath)
        {
            string Sql = String.Format("select * from checksum where filepath = '{0}'", 
                                       Db.CodeValue(filepath));
            return Db.SingleSelect(Sql);
        }

        static ChecksumRow LeseSatzAnhandPrüfsumme(string sha256)
        {
            string Sql = String.Format("select * from checksum where SHA256 = '{0}'", 
                                        Db.CodeValue(sha256));
            return Db.SingleSelect(Sql);
        }

        static void Setze_Gelöscht_Kennzeichen_bei_allen_Datensätzen()
        {
            int Count = Prüfsummentabelle_GetCount();
            if (Count > 0)
                Db.NonQuery("update checksum set DELETED = 'J' where REPORTED = 'N'");
        }

        static void Suche_Datensätze_mit_Gelöscht_Kennzeichen()
        {
            // Vor der Dateiverarbeitung haben wir alle Dateien auf Gelöscht gesetzt.
            // Während der Verarbeitung haben wir bei jedem Update das Gelöscht-Kennzeichen wieder weggenommen
            // Die übriggebliebenen Sätze müssen also die sein, die nicht mehr verarbeitet wurden, also die Gelöschten!

            int Count = Prüfsummentabelle_GetCount();
            if (Count > 0)
            {
                string Sql = String.Format("select * from checksum where DELETED = 'J' and REPORTED = 'N' ");
                List<ChecksumRow> Datensätze = Db.MultiSelect(Sql);
                foreach(var Satz in Datensätze)
                    Erg.Änderungen.Add(new Änderung(Satz.FILEPATH, "gelöscht", Änderungsart.Gelöscht));
            }
        }

        static void Setze_alle_neuen_Datensätze_auf_verarbeitet()
        {
            int Count = Prüfsummentabelle_GetCount();
            if (Count > 0)
                Db.NonQuery("update checksum set REPORTED = 'J' where REPORTED = 'N'");
        }

        static int Prüfsummentabelle_GetCount()
        {
            return Db.SkalarSelect("select count(*) from checksum");
        }

        static void Prüfsummentabelle_Update(ChecksumRow row)
        {
            string Sql = String.Format("update checksum " + 
                                       "set FILEPATH   = '{0}', " + 
                                       "    FILENAME   = '{1}', " + 
                                       "    SHA256     = '{2}', " +
                                       "    MOD_DATE   = '{3}', " +
                                       "    FILESIZE   = '{4}', " +
                                       "    ATTRIBUTES = '{5}', " +
                                       "    DELETED    = 'N' " + 
                                       "    where ID = {6}", 
                                        Db.CodeValue(row.FILEPATH), 
                                        Db.CodeValue(row.FILENAME), 
                                        row.SHA256, 
                                        row.MOD_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                                        row.FILESIZE,
                                        row.ATTRIBUTES,
                                        row.ID);
            int RowsAffected = Db.NonQuery(Sql);
            if (RowsAffected == 0)
                throw new Exception("Datenbankfehler, Satz mit der ID " + row.ID + " nicht gefunden bei Update");
        }

        static void Prüfsummentabelle_Insert(ChecksumRow row)
        {
            string Sql = String.Format(
                "insert into checksum (FILEPATH, FILENAME, SHA256, MOD_DATE, FILESIZE, ATTRIBUTES) " +
                " values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}' )", 
                Db.CodeValue(row.FILEPATH), 
                Db.CodeValue(row.FILENAME), 
                row.SHA256, 
                row.MOD_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                row.FILESIZE,
                row.ATTRIBUTES);
            int RowsAffected = Db.NonQuery(Sql);
            if (RowsAffected == 0)
                throw new Exception("Datenbankfehler, Insert gescheitert");
        }

        static void Änderungstabelle_Insert2(int id, DateTime date, string action)
        {
            if (action.Length > 1000)
                action = action.Substring(0, 1000);

            string Sql = String.Format("insert into changes (ID, MOD_DATE, ACTION) " +
                                        " values ('{0}', '{1}', '{2}' )", 
                                        id,
                                        date.ToString("yyyy-MM-dd HH:mm:ss"),
                                        Db.CodeValue(action));
            int RowsAffected = Db.NonQuery(Sql);
        }

        #endregion



        #region ------------- Reporting -----------------------------------------------------------
        
        static void Email_senden_mit_Fehlerbehandlung()
        {
            string Body = "";
            try
            {
                System.Console.WriteLine("Ergebnisprotokoll erzeugen...");
                Body = Generiere_Protokoll();

                System.Console.WriteLine("Protokolldatei speichern...");
                Log(Body);

                Email_senden(Body);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(
                    "Senden der Email mit den Ergebnissen fehlgeschlagen. " + 
                    "Zweiter Versuch beginnt gleich.\n\nFehlerinfo:\n{0}\n\n", ex.ToString());
                try
                {
                    Email_senden(Body);
                }
                catch (Exception ex2)
                {
                    System.Console.WriteLine(
                        "Senden der Email mit den Ergebnissen erneut fehlgeschlagen. " + 
                        "Programm wird beendet.\n\nFehlerinfo:\n{0}\n\n", ex2.ToString());
                }
            }
        }

        static void Email_senden(string body)
        {
            System.Console.WriteLine("Email senden...");
            Email Emailsender = new Email(Par.SmtpServer, Par.SmtpUser, Par.SmtpPassword, 
                                          Par.SmtpPort, Par.SmtpEnableSSL, Par.SmtpTimeout);
            Emailsender.ErzeugeAnhang(Par.Protokolldateiname, "Geänderte Dateien.txt");

            Emailsender.Sende(Par.Notification_from,
                              Par.Notification_to,
                              Par.Notification_title + DateTime.Today.ToShortDateString(),
                              body);
                
            System.Console.WriteLine("Email gesendet!");
        }

        static string Generiere_Protokoll()
        {
            int ProzentZuArchivieren = 0;
            if (Erg.SummeDateigrößen != 0)
                ProzentZuArchivieren = (int)((Erg.SummeDateigrößenArchivAttribut * 100) / Erg.SummeDateigrößen);

            Int64 Verändert = Erg.AnzahlAlleDateien - Erg.AnzahlUnveränderte;

            string Body = "";
            Body += "Dies ist die Zusammenfassung der Integritätsprüfung für folgenden Speicherort: \n";
            Body += Par.Path + "\n\n";
            Body += "Programmversion vom        :  " + Version + "\n";
            Body += "Anzahl Dateien in Datenbank:  " + Erg.AnzahlAlleDateien + "\n";
            Body += "Gesamtgröße des Archivs    :  " + Erg.SummeDateigrößen / (1024 * 1024) + " MB\n";
            Body += "Untersucht und unverändert :  " + Erg.AnzahlUnveränderte + "\n";
            Body += "Verändert                  :  " + Verändert + "\n";
            Body += "Zu sichernde Dateien sind  :  " + Erg.AnzahlMitArchivAttribut +
                    " Dateien in insgesamt " + Erg.SummeDateigrößenArchivAttribut / (1024 * 1024) + " MB" +
                    " = " + ProzentZuArchivieren + " % des Archivs\n";

            if (LöschungDesArchivbitsIgnorieren)
                Body += "\n\nAchtung: NICHT gemeldet werden gelöschte Archivbits!\n\n";

            Body += "Neu:                          " + (from d in Erg.Änderungen where d.Art == Änderungsart.Neu                select d).Count() + "\n";
            Body += "Geändert:                     " + (from d in Erg.Änderungen where d.Art == Änderungsart.Geändert           select d).Count() + "\n";
            Body += "Gelöscht:                     " + (from d in Erg.Änderungen where d.Art == Änderungsart.Gelöscht           select d).Count() + "\n";
            Body += "Umbenannt:                    " + (from d in Erg.Änderungen where d.Art == Änderungsart.Umbenannt          select d).Count() + "\n";
            Body += "Verschoben:                   " + (from d in Erg.Änderungen where d.Art == Änderungsart.Verschoben         select d).Count() + "\n";
            Body += "Attributsänderung:            " + (from d in Erg.Änderungen where d.Art == Änderungsart.AttributeGeändert  select d).Count() + "\n";
            Body += "Reine Größenänderung:         " + (from d in Erg.Änderungen where d.Art == Änderungsart.Größenänderung     select d).Count() + "\n";
            Body += "Datumsänderung:               " + (from d in Erg.Änderungen where d.Art == Änderungsart.Datumsänderung     select d).Count() + "\n";
            Body += "\n\n\n";


            if (Par.IncludeFileListsInEmail)
            {
            if (Erg.Änderungen.Count() > 0)
            {
                string GeänderteDateienAufstellung = Änderung.ListToString(Erg.Änderungen);
                Body += "Geänderte Dateien: \n" + GeänderteDateienAufstellung + "\n";
            }
            }
            return Body;
        }

        static void Log(string text)
        {
            System.Console.WriteLine(text);
            File.AppendAllText(Par.Protokolldateiname, text);
        }

        #endregion


    
        #region ------------- Testcode ------------------------------------------------------------

            //ChecksumRow Row = new ChecksumRow(){ FILEPATH = "äöüÄÖÜß°^µ€@~#'!\"§$%&/()=?+*<>[]{}", 
            //                                     FILENAME = "test", 
            //                                     MOD_DATE = DateTime.Now,
            //                                     SHA256   = "test123"};
            //Prüfsummentabelle_Insert(Row);
            //ChecksumRow Ro2 = LeseSatzAnhandPrüfsumme("test123");

        #endregion
    }
}
