using System;
using System.Diagnostics;
using System.Threading;
using mshtml;

namespace RapidShareDownloader
{
    public class Rapidshare : FilesharingProvider
    {
        public Rapidshare(HTMLBrowserClient browser)
            : base(browser)
        {
        }

        public override long Download(string url, string zielverzeichnis)
        {
            Debug.WriteLine("Herunterlade_eine_Datei: " + url);
            Browser.SchreibInsProtokoll("Starte den Internet Explorer und rufe den Rapidshare-Link...");
            Browser.Starte_InternetExplorer(url, Browser.InternetExplorer_sichtbar_starten);

            // Wenn die Overlay-Box (MessageBox) erscheint, dort auf "Nein" drücken
            HTMLDivElement OverlayBox = Browser.Suche_Div_auf_der_Seite("overlay-box", "");
            if (OverlayBox != null)
            {
                Browser.SchreibInsProtokoll("Drücke die Overlay-Box weg.");
                HTMLAnchorElement NeinButtonLink = Browser.Suche_Hyperlink("", "Nein");
                if (NeinButtonLink != null)
                    Browser.Navigiere_zu_Link(NeinButtonLink.href);
            }

            // Suche den "Free Download" Button
            Browser.SchreibInsProtokoll("Suche den \"Free download\" button...");
            HTMLAnchorElement gefundenes_Div = Browser.Suche_Hyperlink("js_free-download_btn", "");
            if (gefundenes_Div == null)
            {
                Browser.SchreibInsProtokoll("Habe den \"Free download\" Button NICHT gefunden, Download ist gescheitert!");
                Browser.SchreibInsProtokoll("HTML:");
                Browser.SchreibInsProtokoll(gefundenes_Div.innerHTML);
                throw new DurchsuchenFehlgeschlagenException();
            }
            Browser.SchreibInsProtokoll("Den \"Free download\" Button gefunden, warte auf Freigabe...");


            // Drücke drauf und warte, bis die Seite ganz geladen ist
            Browser.Navigiere_zu_Link(gefundenes_Div.href);

            // Warte, bis der Warte-Timer heruntergezählt hat
            int TimeoutSekunden = 120;
            HTMLAnchorElement JetztHerunterladenButton = null;
            do
            {
                int Sekunden = -1;
                HTMLDivElement Timerbox = Browser.Suche_Div_auf_der_Seite("", "timerbox");
                if (SekundenZaehlerIstDaUndEsSindNochSekundenÜbrig(Timerbox, out Sekunden))
                {
                    if ((TimeoutSekunden % 10) == 0)
                        Browser.SchreibInsProtokoll("Zähler gefunden, warte noch " + Sekunden + " Sekunden");
                    Thread.Sleep(1000);
                }
                else
                    break;
            } while (--TimeoutSekunden > 0);

            if (TimeoutSekunden <= 0)
                throw new TimeoutException("Der Download-Button ist innerhalb von 120 Sekunden nicht erschienen");

            //Prüfe_auf_erreichtes_Tageslimit();


            JetztHerunterladenButton = Browser.Suche_Hyperlink("js_downloadnowlink", "");
            if (!Browser.LinkGefunden(JetztHerunterladenButton))
                throw new TimeoutException("Der Download-Link ist nicht zu finden");
            Browser.SchreibInsProtokoll("Download-Link gefunden, beginne mit dem Download...");


            // URL, unter der nun wirklich die gewünschte Datei heruntergeladen werden kann
            string downloadlink = JetztHerunterladenButton.href;
            long Dateigröße = Browser.Downloade_Datei_mit_Protokollierung(downloadlink, zielverzeichnis, url);
            Browser.SchreibInsProtokoll("Download erfolgreich beendet.");
            return Dateigröße;
        }

        private bool SekundenZaehlerIstDaUndEsSindNochSekundenÜbrig(HTMLDivElement Timerbox, out int sekunden)
        {
            sekunden = -1;
            if (Timerbox != null && Timerbox.innerText != null && Timerbox.innerText.Contains("sec"))
            {
                int Sekunden = -1;
                string SekundenText = Timerbox.innerText;
                try
                {
                    Sekunden = Convert.ToInt16(SekundenText.Substring(0, 2));
                    if (Sekunden > 0)
                    {
                        sekunden = Sekunden;
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        //private void Prüfe_auf_erreichtes_Tageslimit()
        //{
        //    // Es könnte sein, dass wir das tägliche Download-Limit erreicht haben
        //    // Wir schauen, ob wir das Wort "Limit" auf der Seite entdecken.
        //    // Wenn ja, ist dem wohl so und wir brechen NICHT ab, 
        //    // sondern versuchen es einfach nochmal ganz von vorn.
        //    // if (downloadlimit_erreicht)
        //    HTMLDocument doc = (HTMLDocument)sitzung.Document;
        //    string bodytext = doc.body.innerHTML;
        //    if (bodytext.Contains("Tageslimit") && bodytext.Contains("erreicht"))
        //        throw new TageslimitErreichtException();
        //}


    }
}
