using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_zu_DATEV_Konverter
{
    class FormatConverter
    {
        public List<CsvRow> Read_input_file(string filename)
        {
            var Rows = new List<CsvRow>();
            using (StreamReader reader = new StreamReader(filename, Encoding.GetEncoding(1252) ))
            {
                int LineNumber = 1;
                string Line;
                while ((Line = reader.ReadLine()) != null)
                {
                    if (LineNumber++ == 1)
                        continue;

                    var Fields = Line.Split(new char[] { ';' });
                    var Row = new CsvRow();
                    Row.Belegdatum         = TrimField(Fields[ 0]);
                    Row.Buchungsdatum      = TrimField(Fields[ 1]);
                    Row.Belegnummernkreis  = TrimField(Fields[ 2]);
                    Row.Belegnummer        = TrimField(Fields[ 3]);
                    Row.Buchungstext       = TrimField(Fields[ 4]);
                    Row.Buchungsbetrag     = TrimField(Fields[ 5]);
                    Row.Sollkonto          = TrimField(Fields[ 6]);
                    Row.Habenkonto         = TrimField(Fields[ 7]);
                    Row.Steuerschlüssel    = TrimField(Fields[ 8]);
                    Row.Kostenstelle1      = TrimField(Fields[ 9]);
                    Row.Kostenstelle2      = TrimField(Fields[10]);
                    Row.BuchungsbetragEuro = TrimField(Fields[11]);
                    Row.Währung            = TrimField(Fields[12]);
                    Row.Zusatzangaben      = TrimField(Fields[13]);
                    Rows.Add(Row);
                }
            }

            return Rows;
        }

        private string TrimField(string input)
        {
            if (input.Length > 0)
            {
                if (input[0] == '"')
                    input = input.Remove(0, 1);
                if (input[input.Length-1] == '"')
                    input = input.Remove(input.Length - 1, 1);
            }
            return input;
        }

        public void Write_output_file(string filename, List<DatevRow> data)
        {
            int i=0;
            string[] Rows = new string[data.Count+2];
            Rows[i++] = "\"EXTF\";300;21;\"Buchungsstapel\";2;20170629113731114;;\"HL\";\"\";\"\";1336;91104;20160101;4;20160101;20161231;\"\";\"\";1;0;;\"\";;;;";
            Rows[i++] = "Umsatz (ohne Soll/Haben-Kz);Soll/Haben-Kennzeichen;WKZ Umsatz;Kurs;Basis-Umsatz;WKZ Basis-Umsatz;Konto;Gegenkonto (ohne BU-Schlüssel);BU-Schlüssel;Belegdatum;Belegfeld 1;Belegfeld 2;Skonto;Buchungstext;Postensperre;Diverse Adressnummer;Geschäftspartnerbank;Sachverhalt;Zinssperre;Beleglink;Beleginfo - Art 1;Beleginfo - Inhalt 1;Beleginfo - Art 2;Beleginfo - Inhalt 2;Beleginfo - Art 3;Beleginfo - Inhalt 3;Beleginfo - Art 4;Beleginfo - Inhalt 4;Beleginfo - Art 5;Beleginfo - Inhalt 5;Beleginfo - Art 6;Beleginfo - Inhalt 6;Beleginfo - Art 7;Beleginfo - Inhalt 7;Beleginfo - Art 8;Beleginfo - Inhalt 8;KOST1 - Kostenstelle;KOST2 - Kostenstelle;Kost-Menge;EU-Land u. UStID;EU-Steuersatz;Abw. Versteuerungsart;Sachverhalt L+L;Funktionsergänzung L+L;BU 49 Hauptfunktionstyp;BU 49 Hauptfunktionsnummer;BU 49 Funktionsergänzung;Zusatzinformation - Art 1;Zusatzinformation- Inhalt 1;Zusatzinformation - Art 2;Zusatzinformation- Inhalt 2;Zusatzinformation - Art 3;Zusatzinformation- Inhalt 3;Zusatzinformation - Art 4;Zusatzinformation- Inhalt 4;Zusatzinformation - Art 5;Zusatzinformation- Inhalt 5;Zusatzinformation - Art 6;Zusatzinformation- Inhalt 6;Zusatzinformation - Art 7;Zusatzinformation- Inhalt 7;Zusatzinformation - Art 8;Zusatzinformation- Inhalt 8;Zusatzinformation - Art 9;Zusatzinformation- Inhalt 9;Zusatzinformation - Art 10;Zusatzinformation- Inhalt 10;Zusatzinformation - Art 11;Zusatzinformation- Inhalt 11;Zusatzinformation - Art 12;Zusatzinformation- Inhalt 12;Zusatzinformation - Art 13;Zusatzinformation- Inhalt 13;Zusatzinformation - Art 14;Zusatzinformation- Inhalt 14;Zusatzinformation - Art 15;Zusatzinformation- Inhalt 15;Zusatzinformation - Art 16;Zusatzinformation- Inhalt 16;Zusatzinformation - Art 17;Zusatzinformation- Inhalt 17;Zusatzinformation - Art 18;Zusatzinformation- Inhalt 18;Zusatzinformation - Art 19;Zusatzinformation- Inhalt 19;Zusatzinformation - Art 20;Zusatzinformation- Inhalt 20;Stück;Gewicht;Zahlweise;Forderungsart;Veranlagungsjahr;Zugeordnete Fälligkeit";

            foreach (var row in data)
                Rows[i++] = Convert_codepage(row.ToCsv());

            File.WriteAllLines(filename, Rows, Encoding.UTF8);
        }

        private string Convert_codepage(string input)
        {
            string Output = input;//.Replace("ü", Convert.ToString(0xFC));
            return Output;
        }

        public List<DatevRow> Convert_all_rows(List<CsvRow> inputRows)
        {
            var TempRows = Convert_negative_bookings(inputRows);
            
            TempRows = Convert_split_bookings(TempRows);

            return Convert_fields(TempRows);
        }

        private List<CsvRow> Convert_split_bookings(List<CsvRow> inputRows)
        {
            List<CsvRow> OutputRows = new List<CsvRow>();
            CsvRow LastRow = null;
            CsvRow RowToWrite = null;

            foreach (var row in inputRows)
            {
                if (!row.HatBelegdatum) // Split row
                {
                    var Temp = row.Clone();
                    Temp.Belegdatum = LastRow.Belegdatum;
                    Temp.Buchungsdatum = LastRow.Buchungsdatum;
                    Temp.Belegnummernkreis = LastRow.Belegnummernkreis;
                    Temp.Belegnummer = LastRow.Belegnummer;

                    if (string.IsNullOrWhiteSpace(Temp.Habenkonto))
                        Temp.Habenkonto = LastRow.Habenkonto;
                    else
                        Temp.Sollkonto = LastRow.Sollkonto;

                    OutputRows.Add(Temp);
                    RowToWrite = null;
                    continue;
                }

                if (RowToWrite != null)
                    OutputRows.Add(RowToWrite);

                if (row.HatBelegdatum) // normal row
                {
                    LastRow = row;
                    RowToWrite = row;
                }
            }

            if (RowToWrite != null)
                OutputRows.Add(RowToWrite);

            return OutputRows;
        }

        private List<CsvRow> Convert_negative_bookings(List<CsvRow> inputRows)
        {
            List<CsvRow> OutputRows = new List<CsvRow>();

            foreach (var row in inputRows)
            {
                if (Amount_is_negative(row))
                {
                    Swap_account_ids(row);
                    Remove_sign(row);
                }
                OutputRows.Add(row);
            }

            return OutputRows;
        }

        private static bool Amount_is_negative(CsvRow row)
        {
            return row.BuchungsbetragEuro.StartsWith("-");
        }

        private static void Swap_account_ids(CsvRow row)
        {
            string Temp = row.Habenkonto;
            row.Habenkonto = row.Sollkonto;
            row.Sollkonto = Temp;
        }

        private static void Remove_sign(CsvRow row)
        {
            row.Buchungsbetrag = row.Buchungsbetrag.Remove(0, 1);
        }

        private List<DatevRow> Convert_fields(List<CsvRow> inputRows)
        {
            var OutputRows = new List<DatevRow>();

            foreach (var row in inputRows)
            {
                if (row.HatBelegdatum)
                    OutputRows.Add(Convert_row(row));
            }

            return OutputRows;
        }

        private DatevRow Convert_row(CsvRow input)
        {
            var Output = new DatevRow();

            string Belegdatum = "";
            if (!string.IsNullOrWhiteSpace(input.Belegdatum))
                Belegdatum = input.Belegdatum.Substring(0,2) + input.Belegdatum.Substring(3,2);

            string Buchungsschlüssel = "";
            switch (input.Steuerschlüssel)
            {
                case "8": 
                case "9": 
                case "20": // Generalumkehr
                    Buchungsschlüssel = input.Steuerschlüssel;
                    break;
            }

            //// Anpassung für eine bestimmte Buchung mit Generalumkehr
            //if (input.Habenkonto == "1525")
            //{
            //    if (!string.IsNullOrWhiteSpace(input.Belegdatum) &&
            //        !string.IsNullOrWhiteSpace(input.Buchungsdatum))
            //    {
            //        int MonthBelegdatum = Convert.ToInt32(input.Belegdatum.Substring(3, 2));
            //        int MonthBuchungsdatum = Convert.ToInt32(input.Buchungsdatum.Substring(3, 2));
            //        if (MonthBelegdatum >= 7 && MonthBuchungsdatum < 7)
            //        {
            //            if (string.IsNullOrWhiteSpace(Buchungsschlüssel))
            //                Buchungsschlüssel = "20";
            //        }
            //    }
            //}

            Output.UmsatzOhneSollHabenKz        = input.Buchungsbetrag;
            Output.SollHabenKz                  = "S";  
            Output.WKZUmsatz                    = input.Währung;
            Output.Kurs                         = "";
            Output.BasisUmsatz                  = "";
            Output.WKZBasisUmsatz               = "";
            Output.Konto                        = input.Sollkonto;
            Output.GegenkontoOhneBUSchlüssel    = input.Habenkonto;
            Output.BUSchlüssel                  = Buchungsschlüssel;
            Output.Belegdatum                   = Belegdatum;     
            Output.Belegfeld1                   = input.Belegnummer;
            Output.Belegfeld2                   = "";
            Output.Skonto                       = "";
            Output.Buchungstext                 = input.Buchungstext;
            Output.Postensperre                 = ""; 
            Output.DiverseAdressnummer          = ""; 
            Output.Geschäftspartnerbank         = ""; 
            Output.Sachverhalt                  = ""; 
            Output.Zinssperre                   = ""; 
            Output.Beleglink                    = ""; 
            Output.Beleginfo1Art                = ""; 
            Output.Beleginfo1Inhalt             = ""; 
            Output.Beleginfo2Art                = ""; 
            Output.Beleginfo2Inhalt             = ""; 
            Output.Beleginfo3Art                = ""; 
            Output.Beleginfo3Inhalt             = ""; 
            Output.Beleginfo4Art                = ""; 
            Output.Beleginfo4Inhalt             = ""; 
            Output.Beleginfo5Art                = ""; 
            Output.Beleginfo5Inhalt             = ""; 
            Output.Beleginfo6Art                = ""; 
            Output.Beleginfo6Inhalt             = ""; 
            Output.Beleginfo7Art                = ""; 
            Output.Beleginfo7Inhalt             = "";
            Output.Beleginfo8Art                = "";
            Output.Beleginfo8Inhalt             = "";
            Output.KOST1Kostenstelle            = "";
            Output.KOST2Kostenstelle            = "";
            Output.KostMenge                    = "";
            Output.EULanduUStID                 = "";
            Output.EUSteuersatz                 = "";
            Output.AbwVersteuerungsart          = "";
            Output.SachverhaltLuL               = "";
            Output.FunktionsergänzungLuL        = "";
            Output.BU49Hauptfunktionstyp        = "";
            Output.BU49Hauptfunktionsnummer     = "";
            Output.BU49Funktionsergänzung       = "";
            Output.Zusatzinformation01Art       = "";
            Output.Zusatzinformation01Inhalt    = "";
            Output.Zusatzinformation02Art       = "";
            Output.Zusatzinformation02Inhalt    = "";
            Output.Zusatzinformation03Art       = "";
            Output.Zusatzinformation03Inhalt    = "";
            Output.Zusatzinformation04Art       = "";
            Output.Zusatzinformation04Inhalt    = "";
            Output.Zusatzinformation05Art       = "";
            Output.Zusatzinformation05Inhalt    = "";
            Output.Zusatzinformation06Art       = "";
            Output.Zusatzinformation06Inhalt    = "";
            Output.Zusatzinformation07Art       = "";
            Output.Zusatzinformation07Inhalt    = "";
            Output.Zusatzinformation08Art       = "";
            Output.Zusatzinformation08Inhalt    = "";
            Output.Zusatzinformation09Art       = "";
            Output.Zusatzinformation09Inhalt    = "";
            Output.Zusatzinformation10Art       = "";
            Output.Zusatzinformation10Inhalt    = "";
            Output.Zusatzinformation11Art       = "";
            Output.Zusatzinformation11Inhalt    = "";
            Output.Zusatzinformation12Art       = "";
            Output.Zusatzinformation12Inhalt    = "";
            Output.Zusatzinformation13Art       = "";
            Output.Zusatzinformation13Inhalt    = "";
            Output.Zusatzinformation14Art       = "";
            Output.Zusatzinformation14Inhalt    = "";
            Output.Zusatzinformation15Art       = "";
            Output.Zusatzinformation15Inhalt    = "";
            Output.Zusatzinformation16Art       = "";
            Output.Zusatzinformation16Inhalt    = "";
            Output.Zusatzinformation17Art       = "";
            Output.Zusatzinformation17Inhalt    = "";
            Output.Zusatzinformation18Art       = "";
            Output.Zusatzinformation18Inhalt    = "";
            Output.Zusatzinformation19Art       = "";
            Output.Zusatzinformation19Inhalt    = "";
            Output.Zusatzinformation20Art       = "";
            Output.Zusatzinformation20Inhalt    = "";
            Output.Stück                        = "";
            Output.Gewicht                      = "";
            Output.Zahlweise                    = "";
            Output.Forderungsart                = "";
            Output.Veranlagungsjahr             = "";
            Output.ZugeordneteFälligkeit        = "";
 
            return Output;
        }
    }
}
