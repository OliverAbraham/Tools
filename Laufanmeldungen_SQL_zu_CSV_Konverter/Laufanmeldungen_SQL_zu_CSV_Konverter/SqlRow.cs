using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laufanmeldungen_SQL_zu_CSV_Konverter
{
    class SqlRow
    {
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

        public new string ToString()
        {
            return $"Startnummer:{Startnummer} Name:{Vorname} {Nachname} Strecke:{Strecke} Verein:{Verein}";
        }

        public SqlRow Clone()
        {
            SqlRow New       = new SqlRow();
            New.Startnummer  = this.Startnummer;
            New.Timestamp    = this.Timestamp;
            New.FormID       = this.FormID;
            New.Vorname      = this.Vorname;
            New.Nachname     = this.Nachname;
            New.EMail        = this.EMail;
            New.Strecke      = this.Strecke;
            New.Verein       = this.Verein;
            New.Bemerkung    = this.Bemerkung;
            New.Bestätigung1 = this.Bestätigung1;
            New.Bestätigung2 = this.Bestätigung2;
            New.Bestätigung3 = this.Bestätigung3;
            return New;
        }
    }
}
