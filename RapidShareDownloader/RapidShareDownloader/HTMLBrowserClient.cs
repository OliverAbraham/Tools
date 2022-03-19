using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using mshtml;
using SHDocVw;

namespace RapidShareDownloader
{
    class LadefehlerException : Exception { }
    class FreeUserButtonNotFoundException : Exception { }
    class KeineFreigabeException : Exception { }
    class TageslimitErreichtException : Exception { }
    class DurchsuchenFehlgeschlagenException : Exception { }
    class UnbekannterAnbieterException : Exception { }


    public class HTMLBrowserClient
    {
        public delegate void SchreibInsProtokollDelegate(string text);
        public SchreibInsProtokollDelegate SchreibInsProtokoll = null;
        public bool InternetExplorer_sichtbar_starten;

        private InternetExplorer sitzung = null;



        public void Starte_InternetExplorer(string url, bool internetExplorer_sichtbar_starten)
        {
            InternetExplorer_sichtbar_starten = internetExplorer_sichtbar_starten;

            // Für diesen Typ die DLL "SHDocVw.dll" als Referenz hinzufügen
            // Herunterzuladen unter http://www.dll-files.com/dllindex/dll-files.shtml?shdocvw.
            // Sie erscheint unter dem Namen SHDocVw im Baum
            sitzung = new InternetExplorer();
            sitzung.Visible = InternetExplorer_sichtbar_starten;
            object mVal = System.Reflection.Missing.Value;
            sitzung.Navigate(url, ref mVal, ref mVal, ref mVal, ref mVal);

            // Warten, bis die Seite geladen ist
            Warte_solange_IE_noch_aktiv();
        }

        public void Navigiere_zu_Link(string url)
        {
            object mVal = System.Reflection.Missing.Value;
            sitzung.Navigate(url, ref mVal, ref mVal, ref mVal, ref mVal);

            // Warten, bis die Seite geladen ist
            Warte_solange_IE_noch_aktiv();
        }

        public void Warte_solange_IE_noch_aktiv()
        {
            int timeout_sekunden = 60;
            while (sitzung.Busy && --timeout_sekunden > 0)
            {
                Thread.Sleep(1000);
            }
            if (timeout_sekunden <= 0)
                throw new TimeoutException();
        }

        public HTMLDocument Warte_bis_ein_Formular_erscheint()
        {
            // Warten, bis der Zähler heruntergezählt hat und der Download-Button sichtbar geworden ist
            // Hierzu holen wir das Document der neu geladenen Seite immer wieder neu, 
            // solange bis ein "form" auf der Seite ist
            int timeout_sekunden = 5 * 60;    //maximal 5 Minuten warten
            HTMLDocument doc = (HTMLDocument)sitzung.Document;
            while (doc.forms.length <= 0 && timeout_sekunden >= 0)
            {
                Debug.WriteLine("Warte auf Download-Freigabe... " + timeout_sekunden.ToString());
                System.Threading.Thread.Sleep(2000);
                doc = (HTMLDocument)sitzung.Document;
                timeout_sekunden -= 2;
            }

            // Wir haben 5 Minuten gewartet und es ist kein Download-Link erschienen
            if (timeout_sekunden <= 0)
                throw new KeineFreigabeException();
            return doc;
        }


        public long Downloade_Datei_mit_Protokollierung(string downloadlink, 
                                                            string zielverzeichnis, 
                                                            string zieldatei)
        {
            SchreibInsProtokoll("Download von '" + zieldatei + "'.");

            // Bevor der Download loslegen kann, müssen wir uns aus der URL noch einen Dateinamen schnitzen
            string[] stringteile = zieldatei.Split('/', '\\', '?', '*', ':');
            // Wir nehmen den letzten URL-Bestandteil, also das hinter dem letzten Schrägstrich
            string name_downloaddatei = stringteile[stringteile.GetLength(0) - 1];

            // Verzeichnisnamen davorsetzen
            if (zielverzeichnis.Length > 0)
                name_downloaddatei = zielverzeichnis + "\\" + name_downloaddatei;

            SchreibInsProtokoll("Datei wird gespeichert unter: " + name_downloaddatei);
            Downloade_eine_Datei_mit_dem_Browser(downloadlink, name_downloaddatei,
                                                 (HTMLDocument)sitzung.Document);

            // Dateigröße bestimmen
            FileInfo Info = new FileInfo(name_downloaddatei);
            if (Info != null)
            {
                SchreibInsProtokoll("Ergebnis: Dateigröße " + DateigrößeInStringUmwandeln(Info.Length));
                return Info.Length;
            }
            else
            {
                SchreibInsProtokoll("Ergebnis: Die Dateigröße kann nicht ermittelt werden!");
                return 0;
            }
        }

        public static void Downloade_eine_Datei_mit_dem_Browser(string downloadlink, string name_downloaddatei, HTMLDocument doc)
        {
            // Wir könnten jetzt den Download-Button drücken, aber hier macht ein Sicherheitsmerkmal des
            // InternetExplorers nicht mehr mit. Er möchte nämlich jetzt eine Bestätigung vom Anwender,
            // das er den Download will. Dies geschiet, um die Einschleusung von Viren zu erschweren.
            // Dies hier geht also nicht:
            // formDownload.submit();

            // Stattdessen benutzen wir eine alternative Download-Methode: Den .NET WebClient
            // Wichtig hierbei ist nur, dass wir das Sitzungs-Cookie mit übergeben, 
            // da der Rapidshare-Webserver sonst möglicherweise den Download ablehnt.
            // Wir müssen uns ja schließlich ausweisen, dass wir von der gleichen Sitzung kommen.
            WebClient client = new WebClient();
            client.Headers.Add("Cookie", doc.cookie);
            client.DownloadFile(downloadlink, name_downloaddatei);  // wirft (mindestens) die WebException
        }

        public void Schliesse_Explorerfenster()
        {
            try
            {
                sitzung.Stop();
                sitzung.Quit();
            }
            catch (Exception) { }
        }




        public HTMLFormElement Suche_Formular(string formularname, string action)
        {
            // Für diesen Typ unter "References" "COM" die Referenz "Microsoft HTML Object Library" 
            // hinzufügen. Sie erscheint dann unter dem Namen "MSHTML" im Baum
            HTMLDocument doc = (HTMLDocument)sitzung.Document;
            System.Threading.Thread.Sleep(500);

            // Das Formular mit einer bestimmten ID auf der Seite suchen
            IHTMLElementCollection iforms = (IHTMLElementCollection)doc.forms;
            foreach (var form in iforms)
            {
                HTMLFormElement element = TryCast<HTMLFormElement>(form);
                if (formularname.Length == 0 && action.Length == 0)
                    return element;
                if (element.id == formularname)
                    return element;
                if (element.action.StartsWith(action))
                    return element;
            }

            throw new FreeUserButtonNotFoundException();
        }

        public HTMLAnchorElement Suche_Hyperlink(string anchor_id, string anchor_text)
        {
            // Für diesen Typ unter "References" "COM" die Referenz "Microsoft HTML Object Library" 
            // hinzufügen. Sie erscheint dann unter dem Namen "MSHTML" im Baum
            HTMLDocument Doc = (HTMLDocument)sitzung.Document;
            System.Threading.Thread.Sleep(500);

            // Alle Hyperlinks im HTML Dokument durchsuchen
            foreach (var Anchor in Doc.anchors)
            {
                HTMLAnchorElement EinAnchor = SafeCast<HTMLAnchorElement>(Anchor);
                if (EinAnchor != null)
                {
                    if (anchor_id.Length > 0 && EinAnchor.id == anchor_id)
                        return EinAnchor;
                    if (anchor_text.Length > 0 && EinAnchor.name == anchor_text)
                        return EinAnchor;
                }
            }

            return null;
        }

        public HTMLAnchorElement SucheAnchorMitBestimmtemText(HTMLDivElement div, string anchor_text)
        {
            IHTMLElementCollection Anchors = (IHTMLElementCollection)div.all;
            foreach (var Anchor in Anchors)
            {
                HTMLAnchorElement EinAnchor = TryCast<HTMLAnchorElement>(Anchor);
                if (EinAnchor.id == anchor_text)
                    return EinAnchor;
            }
            return null;
        }

        public HTMLDivElement Suche_Div_auf_der_Seite(string div_id_name, string div_class)
        {
            HTMLDocument Doc = (HTMLDocument)sitzung.Document;
            System.Threading.Thread.Sleep(500);

            IHTMLElementCollection Divs = (IHTMLElementCollection)Doc.all;
            foreach (var Div in Divs)
            {
                if (Div is HTMLDivElement)
                {
                    HTMLDivElement RealDiv = TryCast<HTMLDivElement>(Div);
                    if (div_id_name.Length > 0)
                    {
                        if (RealDiv.id == div_id_name)
                            return RealDiv;
                    }
                    if (div_class.Length > 0)
                    {
                        if (RealDiv.className != null && RealDiv.className == div_class)
                            return RealDiv;
                    }
                }
            }
            return null;
        }

        private T TryCast<T>(object form)
        {
            T el;
            try
            {
                el = (T)form;
                Debug.WriteLine("Gefundenes Formular: " + el.ToString());
            }
            catch (Exception ex)
            {
                SchreibInsProtokoll("TryCast fehlgeschlagen: " + ex.ToString());
                throw new DurchsuchenFehlgeschlagenException();
            }
            return el;
        }

        private T SafeCast<T>(object form)
        {
            try
            {
                T el = (T)form;
                Debug.WriteLine("Gefundenes Formular: " + el.ToString());
                return el;
            }
            catch (Exception ex)
            {
                SchreibInsProtokoll("SafeCast fehlgeschlagen: " + ex.ToString());
                return default(T);
            }
        }

        public bool LinkGefunden(HTMLAnchorElement anchor)
        {
            return (anchor != null && anchor.href.Length > 0);
        }

        public string DateigrößeInStringUmwandeln(long bytes)
        {
            return (bytes / (1024 * 1024)).ToString() + " MB";
        }

    }
}
