//-------------------------------------------------------------------------------------------------
//      ExternalPrograms
//
//      Klasse um externe Programm zu starten, wie z.B. eine Textdatei im Editor, 
//      eine HTML-Datei im Browser, eine Eingabeaufforderung usw.
//
//      Abraham Beratung 10/2013
//      Oliver Abraham
//      www.oliver-abraham.de
//      mail@oliver-abraham.de
//
//-------------------------------------------------------------------------------------------------

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Abraham.ExternalPrograms
{
    class ExternalPrograms
    {
        public string SelectedFile;
        public string SelectedFolder;

        public bool SelectFileDialog(string initialFolder, string wasserzeichen = "Datei auswählen")
        {
            OpenFileDialog Dlg   = new OpenFileDialog();
            Dlg.InitialDirectory = initialFolder;
            Dlg.ValidateNames    = true;
            Dlg.CheckFileExists  = true;
            Dlg.CheckPathExists  = true;
            Dlg.FileName         = wasserzeichen;
            if (Dlg.ShowDialog().Value == true)
            {
                SelectedFile = Dlg.FileName;
                return true;
            }
            return false;
        }

        public bool SaveFileDialog(string initialFolder, string wasserzeichen = "Datei auswählen")
        {
            SaveFileDialog Dlg   = new SaveFileDialog();
            Dlg.InitialDirectory = initialFolder;
            Dlg.ValidateNames    = true;
            //Dlg.CheckFileExists  = true;
            //Dlg.CheckPathExists  = true;
            Dlg.FileName         = wasserzeichen;
            if (Dlg.ShowDialog().Value == true)
            {
                SelectedFile = Dlg.FileName;
                return true;
            }
            return false;
        }

        public bool SelectFolderDialog(string initialFolder, string wasserzeichen = "Verzeichnis auswählen")
        {
            OpenFileDialog Dlg   = new OpenFileDialog();
            Dlg.InitialDirectory = initialFolder;
            Dlg.ValidateNames    = false;
            Dlg.CheckFileExists  = false;
            Dlg.CheckPathExists  = true;
            Dlg.FileName         = wasserzeichen;
            
            try
            {
                if (Dlg.ShowDialog().Value == true)
                {
                    string Dir = Path.GetDirectoryName(Dlg.FileName);
                    if (Directory.Exists(Dir))
                    {
                        SelectedFolder = Dir;
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Der Speicherort existiert nicht oder Probleme beim Zugriff!");
            }
            return false;
        }

        public void StartProcess(string programm, string argumente)
        {
            Process.Start(programm, argumente);
        }

        public void StartBatchfile(string path)
        {
            string AktuellesArbeitsverzeichnis = Directory.GetCurrentDirectory();
            try
            {
                string Arbeitsverzeichnis = Path.GetPathRoot(path);
                Directory.SetCurrentDirectory(Arbeitsverzeichnis);

                string Programm = "cmd.exe";
                Process.Start(Programm, " /k \"" + path + "\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Directory.SetCurrentDirectory(AktuellesArbeitsverzeichnis);
            }
        }

        public void OpenDirectoryInExplorer(string path)
        {
            try
            {
                string Programm = "explorer.exe";
                Process.Start(Programm, " \"" + path + "\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void OpenFileInEditor(string path)
        {
            try
            {
                string Programm = "notepad.exe";
                Process.Start(Programm, " \"" + path + "\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void OpenHelpPage(string filename)
        {
            OpenFileInBrowser(Environment.CurrentDirectory + @"\" + @"Hilfe" + @"\" + filename);
        }

        public void OpenFileInBrowser(string filename)
        {
            try
            {
                string Programm = "iexplore.exe";
                Process.Start(Programm, filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
