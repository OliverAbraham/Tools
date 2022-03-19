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
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SHDocVw;
using mshtml;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace RapidShareDownloader
{
    public partial class Form1 : Form
    {
        private Thread Arbeiter = null;
        private List<DownloadAufgabe> AlleDownloads = null;
        private bool ArbeiterSollStoppen = false;
        private static bool InternetExplorer_sichtbar_starten = true;
            
        public Form1()
        {
            InitializeComponent();
            button_Start.Enabled = true;
            button_Stop.Enabled = false;
            button_Abbruch.Enabled = false;
            Arbeiter = new Thread(Arbeiter_Thread);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();

            if (Arbeiter.IsAlive)
                if (MessageBox.Show("Der aktuelle Download läuft noch. " +
                    "Wollen Sie wirklich den Download abbrechen ?", 
                    "Rapidshare Downloader",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    SchreibInsProtokoll("Stop gedrückt.");
                    ArbeiterSollStoppen = true;
            Arbeiter.Abort();
                }
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            InternetExplorer_sichtbar_starten = checkBox1.Checked;
            AlleDownloads = EingabeInAufgabenZerlegen(textbox_Urls.Text);
            ArbeiterSollStoppen = false;
            button_Start.Enabled = false;
            button_Stop.Enabled = true;
            button_Abbruch.Enabled = true;
            Arbeiter.Start();
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Der aktuelle Download wird noch vollendet, dann wird abgebrochen. "+
                "Wollen Sie wirklich den Download abbrechen ?", "Rapidshare Downloader", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                SchreibInsProtokoll("Stop gedrückt.");
                ArbeiterSollStoppen = true;
                button_Stop.Enabled = false;
            }
        }

        private void button_Abbruch_Click(object sender, EventArgs e)
        {
            SchreibInsProtokoll("Abbruch gedrückt");
            ArbeiterSollStoppen = true;
            Arbeiter.Abort();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            InternetExplorer_sichtbar_starten = checkBox1.Checked;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            if (fb.ShowDialog() == DialogResult.OK)
                Zielverzeichnis_TextBox.Text = fb.SelectedPath;
        }


        // Weitergabe eines Textes an die Funktion "SchreibInsProtokoll_UI"
        // Die läuft im Thread der Benutzeroberfläche und hat daher Zugriff auf die Controls im Dialog
        public delegate void SchreibInsProtokoll_UI_Callback(string text);
        private void SchreibInsProtokoll(string text)
        {
            Debug.WriteLine(text);
            textBox_Protokoll.Invoke(new SchreibInsProtokoll_UI_Callback(this.SchreibInsProtokoll_UI), new object[] { text });
        }
        private void SchreibInsProtokoll_UI(string text)
        {
            textBox_Protokoll.Text += text + "\r\n";
            textBox_Protokoll.SelectionStart = textBox_Protokoll.Text.Length - 1;
            textBox_Protokoll.SelectionLength = 1;
            textBox_Protokoll.ScrollToCaret();
        }


        // Weitergabe des Auftrags an die Benutzeroberfläche, 
        // Die gerade fertig heruntergeladene Datei aus der oberen Textbox zu entfernen
        public delegate void EntferneDateinameAusListe_UI_Callback(string dateiname);
        private void EntferneDateinameAusListe(string dateiname)
        {
            Debug.WriteLine("Entferne Dateiname aus Liste: " + dateiname);
            textBox_Protokoll.Invoke(new EntferneDateinameAusListe_UI_Callback(this.EntferneDateinameAusListe_UI), new object[] { dateiname });
        }
        private void EntferneDateinameAusListe_UI(string dateiname)
        {
            string Text = textbox_Urls.Text;
            int StartPosition = Text.IndexOf(dateiname);
            if (StartPosition < 0)
                return;
            int ZeilenendePosition = Text.IndexOf("\r\n", StartPosition);
            if (ZeilenendePosition <= StartPosition)
                return;

            textbox_Urls.Text = Text.Substring(StartPosition, ZeilenendePosition - StartPosition);
            textbox_Urls.Update();
        }

        private List<DownloadAufgabe> EingabeInAufgabenZerlegen(string eingabe)
        {
            // Eingabe in Zeilen zerlegen
            char[] separators = { '\n', '\r', ' ' };
            string[] urls = eingabe.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            List<DownloadAufgabe> AlleDownloads = new List<DownloadAufgabe>();
            foreach (var url in urls)
                AlleDownloads.Add(new DownloadAufgabe() { Url = url, Erfolg = false });
            return AlleDownloads;
        }

    }

}
