using System;
using System.Text;

namespace CSV_zu_DATEV_Konverter
{
    class DatevRow
    {
        #region ------------- Properties ----------------------------------------------------------

        public string UmsatzOhneSollHabenKz       { get; set; }
        public string SollHabenKz                 { get; set; }
        public string WKZUmsatz                   { get; set; }
        public string Kurs                        { get; set; }
        public string BasisUmsatz                 { get; set; }
        public string WKZBasisUmsatz              { get; set; }
        public string Konto                       { get; set; }
        public string GegenkontoOhneBUSchlüssel   { get; set; }
        public string BUSchlüssel                 { get; set; }
        public string Belegdatum                  { get; set; }
        public string Belegfeld1                  { get; set; }
        public string Belegfeld2                  { get; set; }
        public string Skonto                      { get; set; }
        public string Buchungstext                { get; set; }
        public string Postensperre                { get; set; }
        public string DiverseAdressnummer         { get; set; }
        public string Geschäftspartnerbank        { get; set; }
        public string Sachverhalt                 { get; set; }
        public string Zinssperre                  { get; set; }
        public string Beleglink                   { get; set; }
        public string Beleginfo1Art               { get; set; }
        public string Beleginfo1Inhalt            { get; set; }
        public string Beleginfo2Art               { get; set; }
        public string Beleginfo2Inhalt            { get; set; }
        public string Beleginfo3Art               { get; set; }
        public string Beleginfo3Inhalt            { get; set; }
        public string Beleginfo4Art               { get; set; }
        public string Beleginfo4Inhalt            { get; set; }
        public string Beleginfo5Art               { get; set; }
        public string Beleginfo5Inhalt            { get; set; }
        public string Beleginfo6Art               { get; set; }
        public string Beleginfo6Inhalt            { get; set; }
        public string Beleginfo7Art               { get; set; }
        public string Beleginfo7Inhalt            { get; set; }
        public string Beleginfo8Art               { get; set; }
        public string Beleginfo8Inhalt            { get; set; }
        public string KOST1Kostenstelle           { get; set; }
        public string KOST2Kostenstelle           { get; set; }
        public string KostMenge                   { get; set; }
        public string EULanduUStID                { get; set; }
        public string EUSteuersatz                { get; set; }
        public string AbwVersteuerungsart         { get; set; }
        public string SachverhaltLuL              { get; set; }
        public string FunktionsergänzungLuL       { get; set; }
        public string BU49Hauptfunktionstyp       { get; set; }
        public string BU49Hauptfunktionsnummer    { get; set; }
        public string BU49Funktionsergänzung      { get; set; }
        public string Zusatzinformation01Art      { get; set; }
        public string Zusatzinformation01Inhalt   { get; set; }
        public string Zusatzinformation02Art      { get; set; }
        public string Zusatzinformation02Inhalt   { get; set; }
        public string Zusatzinformation03Art      { get; set; }
        public string Zusatzinformation03Inhalt   { get; set; }
        public string Zusatzinformation04Art      { get; set; }
        public string Zusatzinformation04Inhalt   { get; set; }
        public string Zusatzinformation05Art      { get; set; }
        public string Zusatzinformation05Inhalt   { get; set; }
        public string Zusatzinformation06Art      { get; set; }
        public string Zusatzinformation06Inhalt   { get; set; }
        public string Zusatzinformation07Art      { get; set; }
        public string Zusatzinformation07Inhalt   { get; set; }
        public string Zusatzinformation08Art      { get; set; }
        public string Zusatzinformation08Inhalt   { get; set; }
        public string Zusatzinformation09Art      { get; set; }
        public string Zusatzinformation09Inhalt   { get; set; }
        public string Zusatzinformation10Art      { get; set; }
        public string Zusatzinformation10Inhalt   { get; set; }
        public string Zusatzinformation11Art      { get; set; }
        public string Zusatzinformation11Inhalt   { get; set; }
        public string Zusatzinformation12Art      { get; set; }
        public string Zusatzinformation12Inhalt   { get; set; }
        public string Zusatzinformation13Art      { get; set; }
        public string Zusatzinformation13Inhalt   { get; set; }
        public string Zusatzinformation14Art      { get; set; }
        public string Zusatzinformation14Inhalt   { get; set; }
        public string Zusatzinformation15Art      { get; set; }
        public string Zusatzinformation15Inhalt   { get; set; }
        public string Zusatzinformation16Art      { get; set; }
        public string Zusatzinformation16Inhalt   { get; set; }
        public string Zusatzinformation17Art      { get; set; }
        public string Zusatzinformation17Inhalt   { get; set; }
        public string Zusatzinformation18Art      { get; set; }
        public string Zusatzinformation18Inhalt   { get; set; }
        public string Zusatzinformation19Art      { get; set; }
        public string Zusatzinformation19Inhalt   { get; set; }
        public string Zusatzinformation20Art      { get; set; }
        public string Zusatzinformation20Inhalt   { get; set; }
        public string Stück                       { get; set; }
        public string Gewicht                     { get; set; }
        public string Zahlweise                   { get; set; }
        public string Forderungsart               { get; set; }
        public string Veranlagungsjahr            { get; set; }
        public string ZugeordneteFälligkeit       { get; set; }

        #endregion



        #region ------------- Fields --------------------------------------------------------------

        private const string Separator = ";";

        #endregion



        #region ------------- Methods -------------------------------------------------------------

        public new string ToString()
        {
            return $"{Belegdatum} {Konto} {GegenkontoOhneBUSchlüssel} {Buchungstext}";
        }

        public string ToCsv()
        {
            return
                      UmsatzOhneSollHabenKz.Replace(".", "")       + Separator + 
            Terminate(SollHabenKz               ) + Separator + 
            Terminate(WKZUmsatz                 ) + Separator + 
                      Kurs                        + Separator + 
                      BasisUmsatz                 + Separator +
            Terminate(WKZBasisUmsatz            ) + Separator + 
                      Konto                       + Separator + 
                      GegenkontoOhneBUSchlüssel   + Separator + 
            Terminate(BUSchlüssel               ) + Separator + 
                      Belegdatum                  + Separator + 
            Terminate(Belegfeld1                ) + Separator + 
            Terminate(Belegfeld2                ) + Separator + 
                      Skonto                      + Separator + 
            Terminate(Escape(Buchungstext)      ) + Separator + 
                      Postensperre                + Separator + 
            Terminate(DiverseAdressnummer       ) + Separator + 
                      Geschäftspartnerbank        + Separator + 
                      Sachverhalt                 + Separator + 
                      Zinssperre                  + Separator + 
            Terminate(Beleglink                 ) + Separator + 
            Terminate(Beleginfo1Art             ) + Separator + 
            Terminate(Beleginfo1Inhalt          ) + Separator + 
            Terminate(Beleginfo2Art             ) + Separator + 
            Terminate(Beleginfo2Inhalt          ) + Separator + 
            Terminate(Beleginfo3Art             ) + Separator + 
            Terminate(Beleginfo3Inhalt          ) + Separator + 
            Terminate(Beleginfo4Art             ) + Separator + 
            Terminate(Beleginfo4Inhalt          ) + Separator + 
            Terminate(Beleginfo5Art             ) + Separator + 
            Terminate(Beleginfo5Inhalt          ) + Separator + 
            Terminate(Beleginfo6Art             ) + Separator + 
            Terminate(Beleginfo6Inhalt          ) + Separator + 
            Terminate(Beleginfo7Art             ) + Separator + 
            Terminate(Beleginfo7Inhalt          ) + Separator + 
            Terminate(Beleginfo8Art             ) + Separator + 
            Terminate(Beleginfo8Inhalt          ) + Separator + 
            Terminate(KOST1Kostenstelle         ) + Separator + 
            Terminate(KOST2Kostenstelle         ) + Separator + 
                      KostMenge                   + Separator + 
            Terminate(EULanduUStID              ) + Separator + 
                      EUSteuersatz                + Separator + 
            Terminate(AbwVersteuerungsart       ) + Separator + 
                      SachverhaltLuL              + Separator + 
                      FunktionsergänzungLuL       + Separator + 
                      BU49Hauptfunktionstyp       + Separator + 
                      BU49Hauptfunktionsnummer    + Separator + 
                      BU49Funktionsergänzung      + Separator + 
            Terminate(Zusatzinformation01Art    ) + Separator + 
            Terminate(Zusatzinformation01Inhalt ) + Separator + 
            Terminate(Zusatzinformation02Art    ) + Separator + 
            Terminate(Zusatzinformation02Inhalt ) + Separator + 
            Terminate(Zusatzinformation03Art    ) + Separator + 
            Terminate(Zusatzinformation03Inhalt ) + Separator + 
            Terminate(Zusatzinformation04Art    ) + Separator + 
            Terminate(Zusatzinformation04Inhalt ) + Separator + 
            Terminate(Zusatzinformation05Art    ) + Separator + 
            Terminate(Zusatzinformation05Inhalt ) + Separator + 
            Terminate(Zusatzinformation06Art    ) + Separator + 
            Terminate(Zusatzinformation06Inhalt ) + Separator + 
            Terminate(Zusatzinformation07Art    ) + Separator + 
            Terminate(Zusatzinformation07Inhalt ) + Separator + 
            Terminate(Zusatzinformation08Art    ) + Separator + 
            Terminate(Zusatzinformation08Inhalt ) + Separator + 
            Terminate(Zusatzinformation09Art    ) + Separator + 
            Terminate(Zusatzinformation09Inhalt ) + Separator + 
            Terminate(Zusatzinformation10Art    ) + Separator + 
            Terminate(Zusatzinformation10Inhalt ) + Separator + 
            Terminate(Zusatzinformation11Art    ) + Separator + 
            Terminate(Zusatzinformation11Inhalt ) + Separator + 
            Terminate(Zusatzinformation12Art    ) + Separator + 
            Terminate(Zusatzinformation12Inhalt ) + Separator + 
            Terminate(Zusatzinformation13Art    ) + Separator + 
            Terminate(Zusatzinformation13Inhalt ) + Separator + 
            Terminate(Zusatzinformation14Art    ) + Separator + 
            Terminate(Zusatzinformation14Inhalt ) + Separator + 
            Terminate(Zusatzinformation15Art    ) + Separator + 
            Terminate(Zusatzinformation15Inhalt ) + Separator + 
            Terminate(Zusatzinformation16Art    ) + Separator + 
            Terminate(Zusatzinformation16Inhalt ) + Separator + 
            Terminate(Zusatzinformation17Art    ) + Separator + 
            Terminate(Zusatzinformation17Inhalt ) + Separator + 
            Terminate(Zusatzinformation18Art    ) + Separator + 
            Terminate(Zusatzinformation18Inhalt ) + Separator + 
            Terminate(Zusatzinformation19Art    ) + Separator + 
            Terminate(Zusatzinformation19Inhalt ) + Separator + 
            Terminate(Zusatzinformation20Art    ) + Separator + 
            Terminate(Zusatzinformation20Inhalt ) + Separator + 
                      Stück                       + Separator + 
                      Gewicht                     + Separator + 
                      Zahlweise                   + Separator + 
            Terminate(Forderungsart             ) + Separator + 
                      Veranlagungsjahr            + Separator + 
            ZugeordneteFälligkeit;
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
