using System;
using System.Text;

namespace Laufanmeldungen_SQL_zu_CSV_Konverter
{
    class CsvRow
    {
        #region ------------- Properties ----------------------------------------------------------

        public string Startnummer         { get; set; }
        public string Timestamp           { get; set; }
        public string FormID              { get; set; }
        public string Vorname             { get; set; }
        public string Nachname            { get; set; }
        public string EMail               { get; set; }
        public string Strecke             { get; set; }
        public string Verein              { get; set; }
        public string Bemerkung           { get; set; }
        public string Bestätigung1        { get; set; }
        public string Bestätigung2        { get; set; }
        public string Bestätigung3        { get; set; }

        #endregion



        #region ------------- Fields --------------------------------------------------------------

        private const string Separator = ";";

        #endregion



        #region ------------- Methods -------------------------------------------------------------

        public new string ToString()
        {
            return $"{Startnummer} {Vorname} {Nachname}";
        }

        public string ToCsv()
        {
            return
                       Startnummer            + Separator + 
             Terminate(Timestamp            ) + Separator + 
                       FormID                 + Separator + 
             Terminate(Vorname              ) + Separator + 
             Terminate(Nachname             ) + Separator + 
             Terminate(EMail                ) + Separator + 
             Terminate(Strecke              ) + Separator + 
             Terminate(Verein               ) + Separator + 
             Terminate(Bemerkung            ) + Separator + 
                       Bestätigung1           + Separator + 
                       Bestätigung2           + Separator + 
                       Bestätigung3           + Separator ;
        }

        #endregion



        #region ------------- Implementation ------------------------------------------------------

        private string Terminate(string value)
        {
            return "\"" + value + "\"";
        }

        private string Escape(string input)
        {
            if (input.Contains("\""))
            {
                StringBuilder Sb = new StringBuilder();
                for(int i=0; i< input.Length; i++)
                {
                    if (input[i] == '"')
                        Sb.Append('"');
                    Sb.Append(input[i]);
                }
                return Sb.ToString();
            }
            else
            {
                return input;
            }
        }

        #endregion
    }
}
