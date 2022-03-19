using System;

namespace CSV_zu_DATEV_Konverter
{
    class CsvRow
    {
        public string Belegdatum         { get; set; }
        public string Buchungsdatum      { get; set; }
        public string Belegnummernkreis  { get; set; }
        public string Belegnummer        { get; set; }
        public string Buchungstext       { get; set; }
        public string Buchungsbetrag     { get; set; }
        public string Sollkonto          { get; set; }
        public string Habenkonto         { get; set; }
        public string Steuerschlüssel    { get; set; }
        public string Kostenstelle1      { get; set; }
        public string Kostenstelle2      { get; set; }
        public string BuchungsbetragEuro { get; set; }
        public string Währung            { get; set; }
        public string Zusatzangaben      { get; set; }

        public bool HatBelegdatum => !string.IsNullOrWhiteSpace(Belegdatum);

        public new string ToString()
        {
            return $"BelegDt:{Belegdatum} BelegNr:{Belegnummer} Soll:{Sollkonto} Haben:{Habenkonto} {Buchungstext}";
        }

        public CsvRow Clone()
        {
            CsvRow New = new CsvRow();
            New.Belegdatum         = this.Belegdatum        ;
            New.Buchungsdatum      = this.Buchungsdatum     ;
            New.Belegnummernkreis  = this.Belegnummernkreis ;
            New.Belegnummer        = this.Belegnummer       ;
            New.Buchungstext       = this.Buchungstext      ;
            New.Buchungsbetrag     = this.Buchungsbetrag    ;
            New.Sollkonto          = this.Sollkonto         ;
            New.Habenkonto         = this.Habenkonto        ;
            New.Steuerschlüssel    = this.Steuerschlüssel   ;
            New.Kostenstelle1      = this.Kostenstelle1     ;
            New.Kostenstelle2      = this.Kostenstelle2     ;
            New.BuchungsbetragEuro = this.BuchungsbetragEuro;
            New.Währung            = this.Währung           ;
            New.Zusatzangaben      = this.Zusatzangaben     ;
            return New;
        }
    }
}
