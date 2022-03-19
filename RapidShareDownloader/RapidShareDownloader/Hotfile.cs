using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using mshtml;

namespace RapidShareDownloader
{
    public class Hotfile : FilesharingProvider
    {
        public Hotfile(HTMLBrowserClient browser)
            : base(browser)
        {
        }

        public override long Download(string url, string zielverzeichnis)
        {
            Debug.WriteLine("Herunterlade_eine_Datei: " + url);
            Browser.Starte_InternetExplorer(url, Browser.InternetExplorer_sichtbar_starten);

            // Das Form-Element mit der passenden ID bzw. anderen Merkmalen auf der Seite suchen
            HTMLFormElement gefundenes_Formular = Browser.Suche_Formular("", "/dl/");

            // Bei hotfile löst der Button eine JavaScript-Funktion aus, die wiederum startet den Timer.
            // Wir müssen den Button suchen und die click-Methode aufrufen
            bool gefunden = false;
            foreach (var button in gefundenes_Formular)
            {
                HTMLInputElementClass htmlButton = (HTMLInputElementClass)button;
                string html = htmlButton.outerHTML;
                if (html.Contains("DOWNLOAD"))
                {
                    htmlButton.click();
                    gefunden = true;
                }
            }

            if (!gefunden)
                throw new FreeUserButtonNotFoundException();

            // Warten, bis die (neue) Seite ganz geladen ist
            Browser.Warte_solange_IE_noch_aktiv();

            //Prüfe_auf_erreichtes_Tageslimit();

            // Warten, bis der Seiteninhalt sich ändert 
            // Die Seite zählt jetzt die Sekunden runter und dann wird eine neue Seite geladen
            // die dann einen anchor(link) mit der class "click_download" enthält
            string downloadlink = "";
            int timeout_sekunden = 5 * 60;
            gefunden = false;
            do
            {
                //JetztHerunterladenButton = Browser.Suche_Hyperlink("click_download", "");

                //doc = (HTMLDocument)sitzung.Document;

                //// Alle "elemente" im HTML Dokument durchlaufen und anchors suchen
                //IHTMLElementCollection iall = (IHTMLElementCollection)doc.all;
                //foreach (var element in iall)
                //{
                //    string text = element.GetType().ToString();
                //    Debug.WriteLine(text);
                //    if (text.Contains("HTMLAnchorElementClass"))
                //    {
                //        HTMLAnchorElementClass anchor = (HTMLAnchorElementClass)element;
                //        if (anchor.className != null && anchor.className == "click_download")
                //        {
                //            downloadlink = anchor.href;
                //            gefunden = true;
                //            break;
                //        }
                //    }
                //}
                if (!gefunden)
                {
                    Debug.WriteLine("Warte auf Download-Freigabe... " + timeout_sekunden.ToString());
                    System.Threading.Thread.Sleep(2000);
                    timeout_sekunden -= 2;
                }
            }
            while (!gefunden && timeout_sekunden > 0);

            // URL, unter der nun wirklich die gewünschte Datei heruntergeladen werden kann
            return Browser.Downloade_Datei_mit_Protokollierung(downloadlink, zielverzeichnis, url);
        }


    }
}
