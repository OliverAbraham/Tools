//-------------------------------------------------------------------------------------------------
//
//                                  PRORIS 3
//                              DATEIFUNKTIONEN
//
//                      Oliver Abraham, Inveos GmbH, 7/2010
//                      oliver.abraham@inveos.com
//                      mail@oliver-abraham.de
//
//-------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;                    // Path


namespace Inveos.Helper
{
    /// <summary>
    /// Die Klasse liest und schreibt Strings mit einer Programmzeile in/von Dateien
    /// </summary>
    public class Dateifunktionen
    {

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Eine Datei einlesen und Inhalt als String zurückgeben
        /// </summary>
        /// <param name="filename">Name der Quelldatei</param>
        /// <returns>Kompletten Dateiinhalt</returns>
        //-----------------------------------------------------------------------------------------
        public string LadeDatei(string filename)
        {
            // Datei komplett in die Variable "Dateiinhalt" einlesen
            Byte[] DateiinhaltRoh = File.ReadAllBytes(filename);

            // Datei mit der "Default"-Umsetzungsmethode in einen String konvertieren
            // Dies ist bei uns die 1252-Codepage. Wenn wir StreamReader verwendet hätten, 
            // hätte er die UTF-8-Kodierung verwendet, was falsch ist.
            Encoding enc = Encoding.Default;
            string Dateiinhalt = enc.GetString(DateiinhaltRoh);


            // Wenn wir StreamReader verwendet hätten, hätte er die UTF-8-Kodierung verwendet, was falsch ist:
            //try 
            //{
            //    //FileInfo fi = File.OpenRead(filename);
            //    using (StreamReader sr = new StreamReader(filename, Encoding.UTF8))
            //        Dateiinhalt = sr.ReadToEnd();
            //}
            //catch (Exception e) 
            //{
            //    LastErrorMsg = e.Message.ToString();
            //    return 1;       // Fehler beim Einlesen aufgetreten
            //}
            //UmlauteKonvertieren_Proris_zu_Csharp(ref Dateiinhalt);

            return Dateiinhalt;
        }



        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Legt eine neue Datei an und schreibt den übergebenen String hinein.
        /// </summary>
        /// <param name="filename">Name der Zieldatei. Falls sie eistiert, wird sie ersetzt.</param>
        /// <param name="inhalt">Zu schreibende Daten.</param>
        //-----------------------------------------------------------------------------------------
        public void SpeichereDatei(string filename, string inhalt)
        {
            // String mit der "Default"-Umsetzungsmethode in eine Bytefolge konvertieren
            Byte[] DateiinhaltRoh;
            Encoding enc = Encoding.Default;
            DateiinhaltRoh = enc.GetBytes(inhalt.ToString());
            File.WriteAllBytes(filename, DateiinhaltRoh);
        }
    }
}
