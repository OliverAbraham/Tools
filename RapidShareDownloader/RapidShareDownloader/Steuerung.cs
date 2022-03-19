//-------------------------------------------------------------------------------------------------
//
//                                   RAPIDSHARE DOWNLOADER
//
//                                      Oliver Abraham 
//                                          4/2010
//                                   mail@oliver-abraham.de
//                                   www.oliver-abraham.de
//
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using mshtml;

namespace RapidShareDownloader
{
    public partial class Form1
    {
        private HTMLBrowserClient Browser = null;

        private void Arbeiter_Thread()
        {
            Browser = new HTMLBrowserClient();
            Browser.SchreibInsProtokoll = SchreibInsProtokoll;

            try
            {
                // Alle eingegebenen URLs nacheinander herunterladen, immer wieder die fehlerhaften versuchen
                AlleDownloadAufträgeAbarbeitenBisAlleErfolgreich();
                SchreibInsProtokoll("Arbeit erfolgreich beendet.");
            }
            catch (ThreadAbortException)
            {
                SchreibInsProtokoll("Abbruch während der Verarbeitung!");
            }
        }

        private void AlleDownloadAufträgeAbarbeitenBisAlleErfolgreich()
        {
            int Durchlaufnummer = 1;
            bool EsIstNochEinDurchlaufErforderlich = false;
            do
            {
                SchreibInsProtokoll("Durchlauf Nr." + Durchlaufnummer);

                bool AlleErfolgreich = EinmalAlleNochOffenenDownloadsVersuchen();
                EsIstNochEinDurchlaufErforderlich = !AlleErfolgreich;

                // Nach dem Durchlauf bewerten wir das Ergebnis
                if (Bewertung())
                    EsIstNochEinDurchlaufErforderlich = true;

                Durchlaufnummer++;
            } while (EsIstNochEinDurchlaufErforderlich && !ArbeiterSollStoppen);
            SchreibInsProtokoll("Fertig");
        }

        private bool EinmalAlleNochOffenenDownloadsVersuchen()
        {
            bool EsIstNochEinDurchlaufErforderlich = false;
            for (int i = 0; i < AlleDownloads.Count; i++)
            {
                DownloadAufgabe Download = AlleDownloads[i];
                if (Download.Erfolg == false)
                {
                    long Dateigröße;
                    Download.Erfolg = Herunterlade_eine_Datei_mit_Fehlerprotokollierung(
                                        Download.Url, Zielverzeichnis_TextBox.Text, out Dateigröße);
                    Download.Dateigröße = Dateigröße;
                    if (Download.Erfolg == false)
                        EsIstNochEinDurchlaufErforderlich = true;
                }
                if (ArbeiterSollStoppen)
                    break;
            }

            return EsIstNochEinDurchlaufErforderlich;
        }

        private bool Bewertung()
        {
            if (AlleDownloads.Count < 2)
                return false;

            SchreibInsProtokoll("Bewertung der Download-Ergebnisse:");


            // Wir suchen erstmal die größte Datei aller Heruntergeladenen
            long GrößteDatei = 0;
            for (int i = 0; i < AlleDownloads.Count; i++)
            {
                GrößteDatei = Math.Max(GrößteDatei, AlleDownloads[i].Dateigröße);
            }


            // Dann prüfen wir alle Dateien bis auf die letzte (denn die kann beliebig klein sein)
            // und markieren alle als "fehlerhaft", die weniger als 90% der Maximalgröße haben.
            // Dies hat dann zur Folge, dass unser Aufrufer noch einen Durchlauf macht.
            bool EsIstNochEinDurchlaufErforderlich = false;
            long NeunzigProzentDerGrößtenDatei = (long)(((double)GrößteDatei) * 0.9);
            for (int i = 0; i < AlleDownloads.Count - 1; i++)
            {
                if (AlleDownloads[i].Dateigröße < NeunzigProzentDerGrößtenDatei)
                {
                    AlleDownloads[i].Erfolg = false;
                    EsIstNochEinDurchlaufErforderlich = true;
                    SchreibInsProtokoll("Die Datei '" + AlleDownloads[i].Url +
                                        "' ist kleiner als 90% des Maximums, nämlich " +
                                        Browser.DateigrößeInStringUmwandeln(AlleDownloads[i].Dateigröße) +
                                        ". Sie wird nochmal heruntergeladen.");
                }
            }

            if (!EsIstNochEinDurchlaufErforderlich)
                SchreibInsProtokoll("Alle Dateien OK");

            return EsIstNochEinDurchlaufErforderlich;
        }

        private bool Herunterlade_eine_Datei_mit_Fehlerprotokollierung(string url, string zielverzeichnis, out long dateigröße)
        {
            dateigröße = 0;
            bool Erfolg = false;
            try
            {
                SchreibInsProtokoll(DateTime.Now.ToString());
                SchreibInsProtokoll(url + ": ");
                long Dateigröße = Herunterlade_eine_Datei_eines_Filesharing_Anbieters(url, zielverzeichnis);
                dateigröße = Dateigröße;
                SchreibInsProtokoll("OK");
                Erfolg = true;
            }
            catch (COMException)
            {
                SchreibInsProtokoll("Seite kann nicht geladen werden! Allgemeiner Fehler (COMException)");
            }
            catch (LadefehlerException)
            {
                SchreibInsProtokoll("Seite kann nicht geladen werden! (möglicherweise URL nicht erreichbar)");
            }
            catch (TimeoutException)
            {
                SchreibInsProtokoll("Seite kann nicht geladen werden! (kine Antwort innerhalb von 60 Sekunden)");
            }
            catch (FreeUserButtonNotFoundException)
            {
                SchreibInsProtokoll("Der 'Free-User'-Button konnte auf der Webseite nicht gefunden werden!");
            }
            catch (TageslimitErreichtException)
            {
                SchreibInsProtokoll("Das Tageslimit wurde erreicht.");
            }
            catch (KeineFreigabeException)
            {
                SchreibInsProtokoll("Der Download-Button ist innerhalb von 5 Minuten nicht sichtbar geworden!");
            }
            catch (DurchsuchenFehlgeschlagenException ex)
            {
                SchreibInsProtokoll("Durchsuchen der Webseite fehlgeschlagen: " + ex.ToString());
            }
            catch (WebException ex)
            {
                SchreibInsProtokoll("Der Download ist gescheitert: " + ex.ToString());
            }
            catch (UnbekannterAnbieterException)
            {
                SchreibInsProtokoll("Unbekannter Download-Anbieter! Ich kann nur Rapidshare-Links!");
            }
            finally
            {
                Browser.Schliesse_Explorerfenster();
                SchreibInsProtokoll("\r\n");
            }
            return Erfolg;
        }

        private long Herunterlade_eine_Datei_eines_Filesharing_Anbieters(string url, string zielverzeichnis)
        {
            FilesharingProvider Provider = null;
 
            if (url.Contains("rapidshare.com"))
                Provider = new Rapidshare(Browser);

            else if (url.Contains("hotfile.com"))
                Provider = new Hotfile(Browser);

            else
                throw new UnbekannterAnbieterException();

            return Provider.Download(url, zielverzeichnis);               
        }

    }
}
