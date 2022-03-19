//------------------------------------------------------------------------------------------------
//
//                      STRING - ERWEITERUNGSFUNKTIONEN
//                      mail@oliver-abraham.de
//
//------------------------------------------------------------------------------------------------


using System;
using System.IO;
using System.Collections.Generic;


namespace Abraham.String
{
    /// <summary>
    /// Regelt, wie die String-Funktionen Text aus Strings ausschneiden und wie sie mit Zeilenenden umgehen.
    /// </summary>
    public enum TextholModus
    {
        /// <summary>Das Stück, das den Anfang des Strings markiert, wird nicht beachtet. </summary>
        OhneAnfang,

        /// <summary>Der Anfang wird beachtet.</summary>
        MitAnfang,
        
        /// <summary>Das Stück, das das Ende des Strings markiert, wird nicht beachtet.</summary>
        OhneEnde,
        
        /// <summary>Das Ende wird beachtet.</summary>
        MitEnde,
        
        /// <summary>Sollte eine neue Zeile in dem Stück beginnen, wird sie nicht beachtet.</summary>
        OhneZeilenvorschub,
        
        /// <summary>Sollte eine neue Zeile in dem Stück beginnen, wird sie beachtet.</summary>
        MitZeilenvorschub
    }


    /// <summary>
    /// Ausnahmen, die bei der Stringverarbeitung entstehen können.
    /// </summary>
    public class StringVerarbeitungsException : Exception
    {
        /// <summary>Eine Ausnahme bei der Stringverarbeitung</summary>
        public StringVerarbeitungsException() 
        { 
        }
        
        /// <summary>Eine Ausnahme bei der Stringverarbeitung</summary>
        public StringVerarbeitungsException(string fehlermeldung) 
        { 
        }
    }


    
    
    /// <summary>
    /// Klasse mit Erweiterungsmethoden für die zentrale string-Klasse.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Konvertiert einen double in einen String, mit Vor- und Nachkommastellen sowie wählbaren Trennzeichen
        /// </summary>
        public static string ToStringExt(this    double wert,
                                            int     vorkomma                = -1, 
                                            int     nachkomma               = -1, 
                                            bool    tausenderpunkte         = true, 
                                            char    tausendertrennzeichen   = '.' , 
                                            char    dezimaltrennzeichen     = ',' )
        {
            string Formatangabe1 = ":";
            string Formatangabe2 = ":";

            if (tausenderpunkte)
                Formatangabe1 += "n";
            else
                Formatangabe1 += "d";

            if (nachkomma != -1)
            {
                Formatangabe1 += nachkomma.ToString();
                Formatangabe2 += nachkomma.ToString();
            }

            string Ausgabe = System.String.Format("{0" + Formatangabe1 + "}", wert);
            string Temp    = System.String.Format("{0" + Formatangabe2 + "}", wert);

            if (vorkomma != -1)
            {
                string ZiffernBisKomma ;
                int Pos = Temp.IndexOf(',');
                if (Pos > 0)
                    ZiffernBisKomma = Temp.HoleTextZwZeilenanfangUndPosition(Pos);
                else
                    ZiffernBisKomma = Temp;
                int AnzahlVorkommastellen = ZiffernBisKomma.Length;

                if (vorkomma > AnzahlVorkommastellen)
                    Ausgabe = Ausgabe.GeneriereZeichen('0', vorkomma - AnzahlVorkommastellen) + Ausgabe;
            }

            return Ausgabe;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Löscht die ersten n Zeichen aus dem String
        /// </summary>
        /// <param name="eingabe">Der String</param>
        /// <param name="count">Anzahl zu löschender Zeichen</param>
        ///----------------------------------------------------------------------------------------
        public static void DeleteFirstNchars(ref string eingabe, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("Der count darf nicht kleiner als 0 sein bei Funktion DeleteFirstNchars!");
            if (eingabe.Length <= count)
                eingabe = "";
            else
                eingabe = eingabe.Substring(count, eingabe.Length - count);
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Holt die ersten n Zeichen aus dem String und löscht sie auch dort
        /// </summary>
        /// <param name="eingabe">Eingabe und Ausgabe! String wird verkürzt!</param>
        /// <param name="count">Anzahl zu holender Zeichen, oder soviel wie noch da ist</param>
        /// <returns>count Zeichen oder soviel wie noch da ist. </returns>
        ///----------------------------------------------------------------------------------------
        public static string GetAndRemoveFirstNchars(ref string eingabe, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("Der count darf nicht kleiner als 0 sein bei Funktion GetAndRemoveFirstNchars!");
            if (eingabe.Length <= count)
            {
                string Ausgabe = eingabe.Substring(0, eingabe.Length);
                eingabe = "";
                return Ausgabe;
            }
            else
            {
                string Ausgabe = eingabe.Substring(0, count);
                DeleteFirstNchars(ref eingabe, count);
                return Ausgabe;
            }   
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Ersetzt in einem String ein Wort nur einmal, nicht wie String.Replace so oft es vorkommt.
        /// Gibt das Resultat zurück.
        /// </summary>
        /// <param name="input">String</param>
        /// <param name="suchbegriff">Zu suchende Stelle</param>
        /// <param name="ersetzeMit">Reinzupackender String</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static int IndexOf_CaseInsensitive(this string input, string searchterm, int startindex = 0)
        {
            if (searchterm.Length == 0)
                return -1;

            bool found_state = false;
            int index = 0;
            int index_in_input = -1;
            int index_in_searchterm = 0;

            foreach (char c in input)
            {
                if (index < startindex)
                    continue;

                if (char.ToUpper(c) == char.ToUpper(searchterm[index_in_searchterm]))
                {
                    if (!found_state)
                    {
                        found_state = true;
                        index_in_searchterm = 1;
                        index_in_input = index;
                    }
                    else
                    {
                        if (index_in_searchterm == searchterm.Length-1)
                            return index_in_input;
                        else
                            index_in_searchterm++;
                    }
                }
                else
                {
                    found_state = false;
                    index_in_searchterm = 0;
                    index_in_input = 0;
                }
                index++;
            }
            return -1;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Ersetzt in einem String ein Wort nur einmal, nicht wie String.Replace so oft es vorkommt.
        /// Gibt das Resultat zurück.
        /// </summary>
        /// <param name="eingabe">String</param>
        /// <param name="suchbegriff">Zu suchende Stelle</param>
        /// <param name="ersetzeMit">Reinzupackender String</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string ReplaceOneTime(this string eingabe, string suchbegriff, string ersetzeMit)
        {
            int StartPosition = eingabe.IndexOf(suchbegriff);
            if (StartPosition < 0)
                return eingabe;

            string Vorher = "";
            if (StartPosition > 0)
                Vorher = eingabe.Substring(0, StartPosition);
                
            string Nachher = eingabe.Substring(StartPosition + suchbegriff.Length);

            return Vorher + ersetzeMit + Nachher;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Ersetzt in einem String ein Wort nur einmal, nicht wie String.Replace so oft es vorkommt.
        /// Gibt das Resultat zurück.
        /// </summary>
        /// <param name="eingabe">String</param>
        /// <param name="suchbegriff">Zu suchende Stelle</param>
        /// <param name="ersetzeMit">Reinzupackender String</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string ReplaceOneTimeWholeWordOnly(this string eingabe, 
                                                         string suchbegriff, 
                                                         string ersetzeMit,
                                                         string charset)
        {
            int position = 0;
            return ReplaceOneTimeWholeWordOnly(eingabe, suchbegriff, ersetzeMit, charset, ref position);
        }

        public static string ReplaceOneTimeWholeWordOnly(this string eingabe, 
                                                         string suchbegriff, 
                                                         string ersetzeMit,
                                                         string charset,
                                                         ref int position)
        {
            int StartPosition = eingabe.IndexOf_CaseInsensitive(suchbegriff, position);
            if (StartPosition < 0)
            {
                position = eingabe.Length;
                return eingabe;
            }

            string Vorher = "";
            if (StartPosition > 0)
                Vorher = eingabe.Substring(0, StartPosition);
                
            string Nachher = eingabe.Substring(StartPosition + suchbegriff.Length);

            bool SeparatedBefore = (Vorher.Length  == 0 || !charset.ContainsChar(Vorher[Vorher.Length-1]));
            bool SeparatedAfter  = (Nachher.Length == 0 || !charset.ContainsChar(Nachher[0]));
            if (SeparatedBefore && SeparatedAfter)
            {
                position = Vorher.Length + ersetzeMit.Length;
                return Vorher + ersetzeMit + Nachher;
            }
            else
            {
                position = eingabe.Length;
                return eingabe;
            }
        }



        public static bool ContainsChar(this string eingabe, char c)
        {
            return (eingabe.IndexOf(c) >= 0);
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Entfernt allen Weissraum aus einem String (Leerzeichen und Tabs)
        /// </summary>
        /// <param name="eingabe">String</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string TrimWhitespaces(this string eingabe)
        {
            return eingabe.Trim('\t', ' ');
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Entfernt Leerzeichen, Tabs und alle Zeilenvorschübe aus einem String.
        /// </summary>
        /// <param name="eingabe">String</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string TrimWhitespacesAndNewlines(this string eingabe)
        {
            return eingabe.Trim('\r', '\n', '\t', ' ');
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Entfernt alle leeren Zeilen aus einem String. 
        /// Die Funktion arbeitet noch nicht in allen Situationen richtig.
        /// Zur Zeit entfernt sie nur eine Zeile, wenn davor und danach ein Zeilenvorschub steht.
        /// </summary>
        /// <param name="eingabe">String</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string RemoveEmptyLines(this string eingabe)
        {
            while (eingabe.Contains("\r\n\r\n"))
                eingabe = eingabe.Replace("\r\n\r\n", "\r\n");
            return eingabe;
        }


        
        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht den Zeilenanfang vor der übergebenen Position und gibt alles zwischen 
        /// dem Zeilenanfang und der übergebenen Position zurück.
        /// </summary>
        /// <param name="eingabe">Mehrzeiliger String</param>
        /// <param name="pos">Position innerhalb der Zeile (nullbasierter Index)</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string HoleTextZwZeilenanfangUndPosition(this string eingabe, int pos)
        {
            int Ende = pos;
            int Anfang = pos;

            if (Anfang > 0)
            {
                Anfang--;
                // Rückwärts gehen, bis wir den Zeilenanfang gefunden haben
                while (Anfang > 0 && eingabe[Anfang] != '\r' && eingabe[Anfang] != '\n')
                    Anfang--;
            }

            return eingabe.Substring(Anfang, Ende - Anfang);
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht ein bestimmtes Wort in einem String und gibt die gesamte Zeile zurück, 
        /// in der das Wort zum ersten Mal vorkommt.
        /// </summary>
        /// <param name="eingabe">Mehrzeiliger String</param>
        /// <param name="suchwort">Stelle</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string HoleTextderGesamtenZeile(this string eingabe, string suchwort)
        {
            int PosSuchwort = eingabe.IndexOf(suchwort);
            if (PosSuchwort >= 0)
                return HoleTextderGesamtenZeile(eingabe, PosSuchwort);
            else
                return "";
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Gibt eine gesamte Zeile eines Strings zurück, in der der übergebene Index liegt.
        /// </summary>
        /// <param name="eingabe">Mehrzeiliger String</param>
        /// <param name="pos">Nullbasierte Position innerhalb des Strings</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string HoleTextderGesamtenZeile(this string eingabe, int pos)
        {
            char[] Zeilentrennzeichen = new char[] { '\n', '\r' };

            // Rückwärts gehen und Zeilenanfang suchen
            int Zeilenanfang = pos;
            while (Zeilenanfang > 0 && 
                   eingabe[Zeilenanfang - 1] != Zeilentrennzeichen[0] && 
                   eingabe[Zeilenanfang - 1] != Zeilentrennzeichen[1] )
            {
                Zeilenanfang--;
            }

            // Jetzt Zeilenende suchen
            int Zeilenende = eingabe.IndexOfAny(Zeilentrennzeichen, Zeilenanfang);
            if (Zeilenende > Zeilenanfang)
                return eingabe.Substring(Zeilenanfang, Zeilenende - Zeilenanfang);
            else
                return "";
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht die beiden übergebenen Stellen "anfang" und "ende" im übergebenen String
        /// und gibt den Text zwischen den Stellen zurück. (Ohne Anfang und Ende)
        /// </summary>
        /// <param name="eingabe">Mehrzeiliger oder einzeiliger String</param>
        /// <param name="anfang">Zu suchender String, der den Anfang markiert</param>
        /// <param name="ende">Zu suchender String, der das Ende markiert</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string HoleTextZwischenZweiStellen( this string eingabe,
                                                          string anfang,
                                                          string ende)
        {
            return HoleTextZwischenZweiStellen(eingabe, anfang, ende, 
                                               TextholModus.OhneAnfang,
                                               TextholModus.OhneEnde);
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht die beiden übergebenen Stellen "anfang" und "ende" im übergebenen String
        /// und gibt den Text zwischen den Stellen zurück.
        /// </summary>
        /// <param name="eingabe">Mehrzeiliger oder einzeiliger String</param>
        /// <param name="anfang">Zu suchender String, der den Anfang markiert</param>
        /// <param name="ende">Zu suchender String, der das Ende markiert</param>
        /// <param name="anfangModus">Flag das angibt, ob der Anfangstext selbst auch zurückgegeben werden soll.</param>
        /// <param name="endeModus">Flag das angibt, ob der Endetext selbst auch zurückgegeben werden soll.</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string HoleTextZwischenZweiStellen(this string eingabe, 
                                                          string anfang, 
                                                          string ende, 
                                                          TextholModus anfangModus, 
                                                          TextholModus endeModus)
        {
            int PosAnfang = eingabe.IndexOf(anfang);
            if (PosAnfang == -1)
                return "";

            int PosEnde = eingabe.IndexOf(ende, PosAnfang + anfang.Length + 1);
            if (PosEnde == -1)
                PosEnde = eingabe.Length - 1;

            if (anfangModus == TextholModus.OhneAnfang)
                PosAnfang += anfang.Length;

            if (endeModus == TextholModus.MitEnde)
                PosEnde += ende.Length;

            return eingabe.Substring(PosAnfang, PosEnde - PosAnfang);
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht die beiden übergebenen Stellen "anfang" und "ende" im übergebenen String
        /// und löscht alles dazwischen. (ohne nachfolgenden Zeilenvorschub)
        /// </summary>
        /// <param name="inhalt">Mehrzeiliger oder einzeiliger String</param>
        /// <param name="anfang">Zu suchender String, der den Anfang markiert</param>
        /// <param name="ende">Zu suchender String, der das Ende markiert</param>
        /// <returns>True, wenn was entfernt wurde (um eine while-Schleife zu basteln)</returns>
        ///----------------------------------------------------------------------------------------
        public static string EntferneTextZwischenZweiStellen( this string inhalt,
                                                              string anfang,
                                                              string ende)
        {
            return EntferneTextZwischenZweiStellen(inhalt, anfang, ende, TextholModus.OhneZeilenvorschub);
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht die beiden übergebenen Stellen "anfang" und "ende" im übergebenen String
        /// und löscht alles dazwischen.
        /// </summary>
        /// <param name="inhalt">Mehrzeiliger oder einzeiliger String</param>
        /// <param name="anfang">Zu suchender String, der den Anfang markiert</param>
        /// <param name="ende">Zu suchender String, der das Ende markiert</param>
        /// <param name="zeilenendeModus">Flag das angibt, ob der nachfolgende Zeilenvorschub auch entfernt werden soll.</param>
        /// <returns>True, wenn was entfernt wurde (um eine while-Schleife zu basteln)</returns>
        ///----------------------------------------------------------------------------------------
        public static string EntferneTextZwischenZweiStellen( this string inhalt,
                                                              string anfang,
                                                              string ende,
                                                              TextholModus zeilenendeModus)   // MitZeilenvorschub, OhneZeilenvorschub
        {
            bool dummy;
            return EntferneTextZwischenZweiStellen(inhalt, anfang, ende, zeilenendeModus, out dummy);
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht die beiden übergebenen Stellen "anfang" und "ende" im übergebenen String
        /// und löscht alles dazwischen.
        /// </summary>
        /// <param name="inhalt">Mehrzeiliger oder einzeiliger String</param>
        /// <param name="anfang">Zu suchender String, der den Anfang markiert</param>
        /// <param name="ende">Zu suchender String, der das Ende markiert</param>
        /// <param name="zeilenendeModus">Flag das angibt, ob der nachfolgende Zeilenvorschub auch entfernt werden soll.</param>
        /// <param name="habWasErsetzt">True, wenn was entfernt wurde (um eine while-Schleife zu basteln)</param>
        /// <returns>Neuer String (Teil von "inhalt")</returns>
        ///----------------------------------------------------------------------------------------
        public static string EntferneTextZwischenZweiStellen( this string inhalt,
                                                              string anfang,
                                                              string ende,
                                                              TextholModus zeilenendeModus,   // MitZeilenvorschub, OhneZeilenvorschub
                                                              out bool habWasErsetzt)
        {
            return inhalt.ErsetzeTextZwischenZweiStellen(anfang, ende, "", 
                                                         TextholModus.OhneAnfang, TextholModus.OhneEnde, 
                                                         out habWasErsetzt);
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht die beiden übergebenen Stellen "anfang" und "ende" im übergebenen String
        /// und ersetzt den Text zwischen den Stellen.
        /// </summary>
        /// <param name="eingabe">Mehrzeiliger oder einzeiliger String</param>
        /// <param name="anfang">Zu suchender String, der den Anfang markiert</param>
        /// <param name="ende">Zu suchender String, der das Ende markiert</param>
        /// <param name="neuerInhalt">Text, der dort eingesetzt wird.</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string ErsetzeTextZwischenZweiStellen( this string eingabe,
                                                             string anfang,
                                                             string ende,
                                                             string neuerInhalt)
        {
            return ErsetzeTextZwischenZweiStellen (eingabe, anfang, ende, neuerInhalt,
                                                   TextholModus.OhneAnfang,
                                                   TextholModus.OhneEnde);
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht die beiden übergebenen Stellen "anfang" und "ende" im übergebenen String
        /// und ersetzt den Text zwischen den Stellen.
        /// </summary>
        /// <param name="eingabe">Mehrzeiliger oder einzeiliger String</param>
        /// <param name="anfang">Zu suchender String, der den Anfang markiert</param>
        /// <param name="ende">Zu suchender String, der das Ende markiert</param>
        /// <param name="neuerInhalt">Text, der dort eingesetzt wird.</param>
        /// <param name="anfangModus">Flag das angibt, ob der Anfangstext selbst auch zurückgegeben werden soll.</param>
        /// <param name="endeModus">Flag das angibt, ob der Endetext selbst auch zurückgegeben werden soll.</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string ErsetzeTextZwischenZweiStellen(this string eingabe,
                                                             string anfang,
                                                             string ende,
                                                             string neuerInhalt,
                                                             TextholModus anfangModus,
                                                             TextholModus endeModus)
        {
            bool HabWasErsetzt;
            return eingabe.ErsetzeTextZwischenZweiStellen (anfang, ende, neuerInhalt, anfangModus, endeModus,
                                                            out HabWasErsetzt);
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht die beiden übergebenen Stellen "anfang" und "ende" im übergebenen String
        /// und ersetzt den Text zwischen den Stellen.
        /// </summary>
        /// <param name="eingabe">Mehrzeiliger oder einzeiliger String</param>
        /// <param name="anfang">Zu suchender String, der den Anfang markiert</param>
        /// <param name="ende">Zu suchender String, der das Ende markiert</param>
        /// <param name="neuerInhalt">Text, der dort eingesetzt wird.</param>
        /// <param name="anfangModus">Flag das angibt, ob der Anfangstext selbst auch zurückgegeben werden soll.</param>
        /// <param name="endeModus">Flag das angibt, ob der Endetext selbst auch zurückgegeben werden soll.</param>
        /// <param name="habWasErsetzt">Rückgabe true, wenn eine Ersetzung erfolgte</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string ErsetzeTextZwischenZweiStellen(this string eingabe,
                                                             string anfang,
                                                             string ende,
                                                             string neuerInhalt,
                                                             TextholModus anfangModus,
                                                             TextholModus endeModus,
                                                             out bool habWasErsetzt)
        {
            int DummyStartposition = 0;
            return eingabe.ErsetzeTextZwischenZweiStellen(anfang,
                                                          ende,
                                                          neuerInhalt,
                                                          anfangModus,
                                                          endeModus,
                                                          out habWasErsetzt,
                                                          ref DummyStartposition);
        }


        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht die beiden übergebenen Stellen "anfang" und "ende" im übergebenen String
        /// und ersetzt den Text zwischen den Stellen.
        /// </summary>
        /// <param name="eingabe">Mehrzeiliger oder einzeiliger String</param>
        /// <param name="anfang">Zu suchender String, der den Anfang markiert</param>
        /// <param name="ende">Zu suchender String, der das Ende markiert</param>
        /// <param name="neuerInhalt">Text, der dort eingesetzt wird.</param>
        /// <param name="anfangModus">Flag das angibt, ob der Anfangstext selbst auch zurückgegeben werden soll.</param>
        /// <param name="endeModus">Flag das angibt, ob der Endetext selbst auch zurückgegeben werden soll.</param>
        /// <param name="habWasErsetzt">Rückgabe true, wenn eine Ersetzung erfolgte</param>
        /// <param name="startposition">Gibt an, wo mit der Suche begonnen werden soll, falls anfang oder ende mehrfach vorkommen.</param>
        /// <returns>Teilstring aus "eingabe"</returns>
        ///----------------------------------------------------------------------------------------
        public static string ErsetzeTextZwischenZweiStellen( this string eingabe,
                                                             string anfang,
                                                             string ende,
                                                             string neuerInhalt,
                                                             TextholModus anfangModus,
                                                             TextholModus endeModus,
                                                             out bool habWasErsetzt,
                                                             ref int startposition)
        {
            string Neu = eingabe;

            int PosAnfang = Neu.IndexOf(anfang, startposition);
            if (PosAnfang == -1)
            {
                habWasErsetzt = false;
                return eingabe;
            }

            int PosEnde = Neu.IndexOf(ende, PosAnfang + anfang.Length + 1);
            if (PosEnde == -1)
            {
                habWasErsetzt = false;
                return eingabe;
            }

            if (anfangModus == TextholModus.OhneAnfang)
                PosAnfang += anfang.Length;

            if (endeModus == TextholModus.MitEnde)
                PosEnde += ende.Length;

            if (PosEnde > PosAnfang)
                Neu = Neu.Remove(PosAnfang, PosEnde - PosAnfang);
            Neu = Neu.Insert(PosAnfang, neuerInhalt);

            habWasErsetzt = (Neu != eingabe);

            // Startposition für den nächsten Schleifendurchlauf berechnen
            int AnzahlEntfernteZeichen = PosEnde - PosAnfang;
            int AnzahlEingefügteZeichen = neuerInhalt.Length;
            startposition = PosEnde + ende.Length - AnzahlEntfernteZeichen + AnzahlEingefügteZeichen;

            return Neu;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht die beiden übergebenen Stellen "anfang" und "ende" im übergebenen String
        /// und löscht alles dazwischen.
        /// </summary>
        /// <param name="inhalt">Mehrzeiliger oder einzeiliger String</param>
        /// <param name="anfang">Zu suchender String, der den Anfang markiert</param>
        /// <param name="ende">Zu suchender String, der das Ende markiert</param>
        /// <param name="zeilenendeModus">Flag das angibt, ob der nachfolgende Zeilenvorschub auch entfernt werden soll.</param>
        /// <returns>Neuer String (Teil von "inhalt)</returns>
        ///----------------------------------------------------------------------------------------
        public static string EntferneTextZwischenZweiStellenAlleStellen(this string inhalt,
                                                                        string anfang,
                                                                        string ende,
                                                                        TextholModus zeilenendeModus)   // MitZeilenvorschub, OhneZeilenvorschub
        {
            // Wiederholt allen Code aus der Datei entfernen
            string NeuerInhalt = "";
            int Startposition = 0;
            bool HabWasErsetzt = true;
            while (HabWasErsetzt)
            {
                inhalt = inhalt.ErsetzeTextZwischenZweiStellen(anfang, ende, NeuerInhalt,
                                                                TextholModus.OhneAnfang,
                                                                zeilenendeModus,
                                                                out HabWasErsetzt,
                                                                ref Startposition);
            }
            return inhalt;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Generiert einen String, der das übergebenen Zeichen n Mal enthält
        /// </summary>
        /// <param name="inhalt">Unbenutzt</param>
        /// <param name="Zeichen">Das Zeichen, das genommen werden soll</param>
        /// <param name="Anzahl">So lang wird der String</param>
        /// <returns>Veränderter String</returns>
        ///----------------------------------------------------------------------------------------
        public static string GeneriereZeichen(this string inhalt, char Zeichen, int Anzahl)
        {
            string Rückgabe = "";
            for (int i = 0; i < Anzahl; i++)
                Rückgabe += Zeichen;
            return Rückgabe;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Füllt den String am Anfang mit einem Zeichen auf.
        /// </summary>
        /// <param name="inhalt">Objekt</param>
        /// <param name="zeichen">Das einzufügende Zeichen</param>
        /// <param name="länge">Wenn das Objekt kürzer als diese Länge ist, wird aufgefüllt. 
        /// Ansonsten bleibt das Objekt unverändert.</param>
        /// <returns>verändertes Objekt (eventuell länger als vorher)</returns>
        ///----------------------------------------------------------------------------------------
        public static string FülleVorneAuf(this string inhalt, char zeichen, int länge)
        {
            if (inhalt.Length < länge)
                return inhalt.GeneriereZeichen(zeichen, länge - inhalt.Length) + inhalt;
            else
                return inhalt;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Füllt den String am Emde mit einem Zeichen auf.
        /// </summary>
        /// <param name="inhalt">Objekt</param>
        /// <param name="zeichen">Das einzufügende Zeichen</param>
        /// <param name="länge">Wenn das Objekt kürzer als diese Länge ist, wird aufgefüllt. 
        /// Ansonsten bleibt das Objekt unverändert.</param>
        /// <returns>verändertes Objekt (eventuell länger als vorher)</returns>
        ///----------------------------------------------------------------------------------------
        public static string FülleHintenAuf(this string inhalt, char zeichen, int länge)
        {
            if (inhalt.Length < länge)
                return inhalt + inhalt.GeneriereZeichen(zeichen, länge - inhalt.Length);
            else
                return inhalt;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Sucht in einem einzeiligen String ein Wort und rückt es auf die angegebene 
        /// Spaltenposition ein, falls es zu weit links steht.
        /// </summary>
        /// <param name="inhalt">Eingabe</param>
        /// <param name="suchbegriff">Hiernach wird im String gesucht</param>
        /// <param name="spaltenposition">Zu dieser Spaltenposition wird das Wort eingerückt</param>
        /// <returns>Gibt den erweiterten String zurück</returns>
        ///----------------------------------------------------------------------------------------
        public static string Einrücken(this string inhalt, string suchbegriff, int spaltenposition)
        {
            if (spaltenposition < 0)
                throw new StringVerarbeitungsException ("Die Spaltenposition darf nicht negativ sein!");

            string Rückgabe = inhalt;
            int Pos = inhalt.IndexOf(suchbegriff);
            if (Pos >= 0 && Pos < spaltenposition)
                Rückgabe = inhalt.Insert(Pos, inhalt.GeneriereZeichen(' ', spaltenposition - Pos));
            return Rückgabe;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Wandelt eine Folge von Zahlen in Hex-Zeichen (sowas wie "3B 2F AB ")
        /// </summary>
        /// <typeparam name="T">Ein Array eines beliebigen Typs</typeparam>
        /// <param name="eingabe">Eine Folge von Bytes</param>
        /// <returns>String mit Hex-Zeichen</returns>
        ///----------------------------------------------------------------------------------------
        public static string ArrayToHexString<T>(T[] eingabe)
        {
            string Ausgabe = "";
            foreach (var Wert in eingabe)
                Ausgabe += string.Format("{0:X2} ", Wert);
            return Ausgabe;
        }

        

        ///----------------------------------------------------------------------------------------
        /// <summary>Teilt einen String in Zeilen auf. Leere Zeilen werden unterdrückt.</summary>
        /// <param name="eingabe">Der String mit allen Zeilen.</param>
        /// <returns>Gibt ein Array von einzelnen Zeilen zurück.</returns>
        ///----------------------------------------------------------------------------------------
        public static string[] ZerlegeStringInZeilen(this string eingabe)
        {
            string[] Zeilen = eingabe.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return Zeilen;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Zerlegt einen String in eine Liste von Strings fester Länge. Das letzte Element kann kürzer sein.
        /// Falls der übergebene String länger als die übergebene Maximallänge ist, 
        /// gibt sie mehrere Elemente zurück, ansonsten nur ein Element.
        /// </summary>
        /// 
        /// <param name="eingabe">Quellstring</param>
        /// <param name="maximallänge">Falls der Quellstring länger als diese Zahl ist, 
        /// wird er in Teile mit dieser Länge geschnitten. Der Rest konnt ins letzte Listenelement.
        /// </param>
        /// 
        /// <returns>
        /// Gibt die Liste von Strings zurück. 
        /// Wenn der Übergebene String leer ist, wird eine leere Liste zurückgegeben.
        /// </returns>
        ///----------------------------------------------------------------------------------------
        public static List<string> StringZerlegenFallsLängerAls(this string eingabe, int maximallänge)
        {
            if (maximallänge <= 0)
                throw new ArgumentOutOfRangeException("Die Maximallänge muss mindestens 1 sein!");

            List<string> Teilstücke = new List<string>();

            while (eingabe.Length > maximallänge)
            {
                Teilstücke.Add(eingabe.Substring(0, maximallänge));
                eingabe = eingabe.Substring(maximallänge);
            }
            if (eingabe.Length > 0)
                Teilstücke.Add(eingabe);

            return Teilstücke;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Zerlegt einen String an den Tabulatoren in ein Array
        /// </summary>
        /// <param name="eingabe">Der zu zerlegende String</param>
        /// <returns>Array mit den Teilen</returns>
        ///----------------------------------------------------------------------------------------
        public static string[] ToStringArray(this string eingabe)
        {
            return ToStringArray(eingabe, new string[] { "\t" });
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Zerlegt einen String an bestimmten Trennzeichen in ein Array
        /// </summary>
        /// <param name="eingabe">Der zu zerlegende String</param>
        /// <param name="trennzeichenArray">Beliebig vielen Zeichen, die als Trenner interpretiert werden.</param>
        /// <returns>Array mit den Teilen</returns>
        ///----------------------------------------------------------------------------------------
        public static string[] ToStringArray(this string eingabe, List<string> trennzeichenArray)
        {
            return ToStringArray(eingabe, trennzeichenArray.ToArray());
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Zerlegt einen String an bestimmten Trennzeichen in ein Array
        /// </summary>
        /// <param name="eingabe">Der zu zerlegende String</param>
        /// <param name="trennzeichenArray">Beliebig vielen Zeichen, die als Trenner interpretiert werden.</param>
        /// <returns>Array mit den Teilen</returns>
        ///----------------------------------------------------------------------------------------
        public static string[] ToStringArray(this string eingabe, string[] trennzeichenArray)
        {
            string[] Teile = eingabe.Split(trennzeichenArray, StringSplitOptions.RemoveEmptyEntries);
            return Teile;
        }


        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Wandelt ein Array von Strings in ein Array von Ints
        /// </summary>
        /// <param name="ÜberschriftenStringArray">Menge von Strings. Jeder String muss eine Zahl enthalten.</param>
        /// <returns>Array mit Zahlen</returns>
        ///----------------------------------------------------------------------------------------
        public static int[] ToIntArray(this string[] ÜberschriftenStringArray)
        {
            int Anzahl = ÜberschriftenStringArray.GetLength(0);
            int[] Rückgabe = new int[Anzahl];
            int Index = 0;
            foreach (string Wert in ÜberschriftenStringArray)
                Rückgabe[Index++] = Convert.ToInt32(Wert);
            return Rückgabe;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Setzt einen String in Anführungsstriche (ohne Character Stuffing)
        /// </summary>
        /// <param name="eingabe">Beliebiger String. Das Apostroph darf vorkommen.</param>
        /// <returns>String eingeschlossen in Anführungsstriche. 
        /// Im String vorhandene Zeichen werden sie NICHT verdoppelt.</returns>
        ///----------------------------------------------------------------------------------------
        public static string InAnführungsstriche(this string eingabe)
        {
            return eingabe.InQuotes ("'");
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Setzt einen String in Doppelte Anführungsstriche (ohne Character Stuffing)
        /// </summary>
        /// <param name="eingabe">Beliebiger String. Das Apostroph darf vorkommen.</param>
        /// <returns>String eingeschlossen in Anführungsstriche. 
        /// Im String vorhandene Zeichen werden sie NICHT verdoppelt.</returns>
        ///----------------------------------------------------------------------------------------
        public static string InDoppelteAnführungsstriche(this string eingabe)
        {
            return eingabe.InQuotes("\"");
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Setzt einen String in eckige Klammern (ohne Character Stuffing)
        /// </summary>
        /// <param name="eingabe">Beliebiger String</param>
        /// <returns>Verlängerter String</returns>
        ///----------------------------------------------------------------------------------------
        public static string InEckigeKlammern(this string eingabe)
        {
            return eingabe.InQuotes("<", ">");
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Setzt einen String in eckige Array-Klammern (ohne Character Stuffing)
        /// </summary>
        /// <param name="eingabe">Beliebiger String</param>
        /// <returns>Verlängerter String</returns>
        ///----------------------------------------------------------------------------------------
        public static string InArrayKlammern(this string eingabe)
        {
            return eingabe.InQuotes("[", "]");
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Setzt einen String in geschweifte Klammern (ohne Character Stuffing)
        /// </summary>
        /// <param name="eingabe">Beliebiger String</param>
        /// <returns>Verlängerter String</returns>
        ///----------------------------------------------------------------------------------------
        public static string InGeschweifteKlammern(this string eingabe)
        {
            return eingabe.InQuotes("{", "}");
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Setzt einen String in beliebige Anführungsstriche, mit oder ohne Character Stuffing
        /// </summary>
        /// <param name="eingabe">Beliebiger String.</param>
        /// <param name="quoteStringBeg">Begrenzungszeichen am Anfang</param>
        /// <param name="quoteStringEnd">Begrenzungszeichen am Ende</param>
        /// <param name="withCharacterStuffing">
        /// true beduetet, dass vor alle Vorkommen 
        /// des Begrenzungszeichens im String der stuffingCharacter eingefügt wird</param>
        /// 
        /// <param name="stuffingCharacter">
        /// Zeichen, das vor alle Vorkommen des 
        /// Begrenzungszeichens eingefügt wird</param>
        /// 
        /// <returns>String eingeschlossen in Anführungsstriche. 
        /// Im String vorhandene Zeichen werden sie NICHT verdoppelt.</returns>
        ///----------------------------------------------------------------------------------------
        public static string InQuotes (this string eingabe,
                                       string quoteStringBeg = "'",
                                       string quoteStringEnd = "",
                                       bool withCharacterStuffing = false,
                                       string stuffingCharacter = "\\")
        {
            if (quoteStringEnd.Length == 0)
                quoteStringEnd = quoteStringBeg;

            string Temp;
            if (withCharacterStuffing == true)
            {
                Temp = eingabe.Replace(quoteStringBeg, stuffingCharacter + quoteStringBeg);
                if (!quoteStringBeg.Equals(quoteStringEnd))
                    Temp = Temp.Replace(quoteStringEnd, stuffingCharacter + quoteStringEnd);
            }
            else
                Temp = eingabe;

            return quoteStringBeg + Temp + quoteStringEnd;
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Fügt in einen String Tausendertrennzeichen ein
        /// </summary>
        //-----------------------------------------------------------------------------------------
        public static string FügeTausenderpunkteEin(this string eingabe, string Tausendertrennzeichen = ".")
        {
            if (eingabe.Length <= 3)
                return eingabe;
            int Startpunkt = eingabe.Length % 4;
            for (int i = Startpunkt; i < eingabe.Length; i += (3+1))
            {
                eingabe = eingabe.Insert(i, ".");
            }
            return eingabe;
        }



        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Wandelt eine Folge von Zahlen in Hex-Zeichen (sowas wie "3B 2F AB ")
        /// </summary>
        /// <typeparam name="T">Ein Array eines beliebigen Typs</typeparam>
        /// <param name="eingabe">Eine Folge von Bytes</param>
        /// <returns>String mit Hex-Zeichen</returns>
        ///----------------------------------------------------------------------------------------
        public static string WandleArrayInHexString<T>(T[] eingabe)
        {
            string Ausgabe = "";
            foreach (var Wert in eingabe)
                Ausgabe += string.Format("{0:X} ", Wert);
            return Ausgabe;
        }


        /// <summary>
        /// Die Funktion sucht ab "start" ein bestimmtes Zeichen, ignoriert aber eingebettete Strings
        /// </summary>
        /// 
        /// <example>
        /// Wir wollen das nächste @-Zeichen suchen:
        ///   string eingabe = "BCC: 'mail@example.com' @";
        ///   Der Aufruf von 
        ///      IndexOf_Char_outside_string (eingabe, 0, '@', '\'','\'') 
        ///   ergibt 24 und nicht 10!
        ///   Der Aufruf mit eingabe = "'mail@example.com'" ergibt -1!
        /// </example>
        /// 
        /// <returns>Index der gefundenen Stelle oder -1, wenn nicht gefunden</returns>
        public static int IndexOf_Char_outside_string(  this string eingabe, 
                                                        int  start, 
                                                        char ToFind, 
                                                        char StringAnfangsZeichen, 
                                                        char StringEndeZeichen)
        {
            for (int i = start; i < eingabe.Length; i++)
            {
                if (eingabe[i] == ToFind)
                    return i;
                // Immer wenn ein String anfängt, diesen komplett überlesen
                if (eingabe[i] == StringAnfangsZeichen)
                {
                    i++;
                    while (i < eingabe.Length && eingabe[i] != StringEndeZeichen)
                        i++;
                    i++;
                }
            }

            return -1; // Nicht gefunden !
        }

        ///----------------------------------------------------------------------------------------
        /// <summary>
        /// Prüft ob ein String leer ist sprich NULL oder Length = 0        
        /// </summary>
        /// <param name="eingabe">Mehrzeiliger oder einzeiliger String</param>        
        /// <returns>bool</returns>
        ///----------------------------------------------------------------------------------------
        public static bool IsEmpty(this string eingabe)
        {
            return (eingabe == null || eingabe.Length == 0);
        }
    }
}
