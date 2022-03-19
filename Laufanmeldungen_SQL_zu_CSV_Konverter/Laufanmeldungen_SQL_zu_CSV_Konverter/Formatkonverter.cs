using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Laufanmeldungen_SQL_zu_CSV_Konverter
{
    class FormatConverter
    {
        #region Felder
        private char[] TrimChars = new char[] { '\'', '"' };
        #endregion

        #region Einlesen der Eingabedatei

        public List<SqlRow> Read_input_file(string filename)
        {
            var Rows = new List<SqlRow>();
            using (StreamReader reader = new StreamReader(filename, Encoding.GetEncoding(1252) ))
            {
                int LineNumber = 1;
                string Line;
                bool Convert = false;

                while ((Line = reader.ReadLine()) != null)
                {
                    if (LineNumber++ == 1)
                        continue;

                    if (Line_contains_Stoptoken(Line))
                        Convert = false;

                    if (Convert)
                        Process_line(Rows, Line);

                    if (Line_contains_Starttoken(Line))
                        Convert = true;
                }
            }

            return Rows;
        }

        private bool Line_contains_Starttoken(string line)
        {
            return line.Contains("INSERT INTO `wp_laufanmeldungen`");
        }

        private bool Line_contains_Stoptoken(string line)
        {
            return line.Contains("ENABLE KEYS");
        }

        private void Process_line(List<SqlRow> Rows, string line)
        {
            var Fields       = Split_line(line, ',');
            var Row          = new SqlRow();
            Row.Startnummer  = TrimField(Fields[0]);
            Row.Timestamp        = TrimField(Fields[1]);
            Row.FormID       = TrimField(Fields[2]);
            Row.Vorname      = TrimField(Fields[3]);
            Row.Nachname     = TrimField(Fields[4]);
            Row.EMail        = TrimField(Fields[5]);
            Row.Strecke      = TrimField(Fields[6]);
            Row.Verein       = TrimField(Fields[7]);
            Row.Bemerkung    = TrimField(Fields[8]);
            Row.Bestätigung1 = TrimField(Fields[9]);
            Row.Bestätigung2 = TrimField(Fields[10]);
            Row.Bestätigung3 = TrimField(Fields[11]);
            Rows.Add(Row);
        }

        private string[] Split_line(string line, char separator)
        {
            string[] Output = new string[100];
            int Index = 0;
            bool InString = false;

            line = line.TrimStart('(').TrimEnd(',').TrimEnd(')');

            foreach (char c in line)
            {
                if (!InString)
                {
                    if (c == ',')
                    {
                        Output[Index] = Output[Index].Trim();
                        Index++;
                        continue;
                    }
                    if (c == '\'')
                        InString = true;
                }
                else
                {
                    if (c == '\'')
                        InString = false;
                }

                Output[Index] += c;
            }

            Output[Index] = Output[Index].Trim();

            string[] Temp = (from o in Output where o != null select o).ToArray();
            return Temp;
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

        private string Convert_codepage(string input)
        {
            string Output = input;//.Replace("ü", Convert.ToString(0xFC));
            return Output;
        }

        #endregion

        #region Konvertierung

        public List<CsvRow> Convert_all_rows(List<SqlRow> inputRows)
        {
            var OutputRows = new List<CsvRow>();

            foreach (var row in inputRows)
                OutputRows.Add(Convert_row(row));

            return OutputRows;
        }

        private CsvRow Convert_row(SqlRow input)
        {
            var Output = new CsvRow();

            Output.Startnummer  = ConvertHtmlSpecialChars(input.Startnummer .Trim(TrimChars));
            Output.FormID       = ConvertHtmlSpecialChars(input.FormID      .Trim(TrimChars));
            Output.Vorname      = ConvertHtmlSpecialChars(input.Vorname     .Trim(TrimChars));
            Output.Nachname     = ConvertHtmlSpecialChars(input.Nachname    .Trim(TrimChars));
            Output.EMail        = ConvertHtmlSpecialChars(input.EMail       .Trim(TrimChars));
            Output.Strecke      = ConvertHtmlSpecialChars(input.Strecke     .Trim(TrimChars));
            Output.Verein       = ConvertHtmlSpecialChars(input.Verein      .Trim(TrimChars));
            Output.Bemerkung    = ConvertHtmlSpecialChars(input.Bemerkung   .Trim(TrimChars));
            Output.Bestätigung1 = ConvertHtmlSpecialChars(input.Bestätigung1.Trim(TrimChars));
            Output.Bestätigung2 = ConvertHtmlSpecialChars(input.Bestätigung2.Trim(TrimChars));
            Output.Bestätigung3 = ConvertHtmlSpecialChars(input.Bestätigung3.Trim(TrimChars));

            if (Field_matches_timestamp_pattern(input))
            {
                Output.Timestamp = Convert_timestamp(input.Timestamp);
            }
            else
            {
                Output.Timestamp = input.Timestamp;
            }
            return Output;
        }

        private string ConvertHtmlSpecialChars(string v)
        {
            return v
                .Replace("&quot;", "'")
                .Replace("&amp;", "&")
                .Replace(@"\r\n", " ")
                .Replace(@"\r", " ")
                .Replace(@"\n", " ");
        }

        private static bool Field_matches_timestamp_pattern(SqlRow input)
        {
            return input.Timestamp.Length == "0x323031382d30372d32322031373a35373a3538".Length &&
                   Regex.IsMatch(input.Timestamp, "0x[0-9A-Fa-f]+");
        }

        private string Convert_timestamp(string timestamp)
        {
            timestamp = timestamp.Remove(0,2);
            
            string Result = "";
            while (!string.IsNullOrWhiteSpace(timestamp))
            {
                string Part = timestamp.Substring(0, 2);
                timestamp = timestamp.Remove(0,2);
                int result = Convert_hex_to_dec(Part);
                Result += (char)result;
            }
            return Result;
        }

        private int Convert_hex_to_dec(string part)
        {
            string Upper = part.Substring(0,1);
            string Lower = part.Substring(1,1);
            int upper = Convert_nibble(Upper[0]);
            int lower = Convert_nibble(Lower[0]);
            return upper * 16 + lower;
        }

        private int Convert_nibble(char digit)
        {
            switch (char.ToUpper(digit))
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'A': return 10;
                case 'B': return 11;
                case 'C': return 12;
                case 'D': return 13;
                case 'E': return 14;
                case 'F': return 15;
                default: throw new Exception("illegal hex char");
            }
        }

        #endregion

        #region Speicherung in die Ausgabedatei

        public void Write_output_file(string filename, List<CsvRow> data)
        {
            int i=0;
            string[] Rows = new string[data.Count+2];
            Rows[i++] = "\"Startnummer\"; \"Datum/Uhrzeit\"; \"FormID\"; \"Vorname\"; \"Nachname\"; \"EMail\"; \"Strecke\"; \"Verein\"; \"Bemerkung\"; \"Bestätigung1\"; \"Bestätigung2\"; \"Bestätigung3\"";

            foreach (var row in data)
                Rows[i++] = Convert_codepage(row.ToCsv());

            File.WriteAllLines(filename, Rows, Encoding.UTF8);
        }

        #endregion
    }
}
