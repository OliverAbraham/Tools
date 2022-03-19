using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Abraham.ExternalPrograms;
using Inveos.String;
using System.Text.RegularExpressions;


namespace WordPressMover
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string FileName = "";
        string Data     = "";
        string NewData  = "";
        string RegularExpression = "";
        MatchCollection Matches;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_1_Click(object sender, RoutedEventArgs e)
        {
            if (Browse_for_file() == false)
                return;
            Load_File();
            Enable_UI();
            Try_to_find_old_domain_name();
        }

        private bool Browse_for_file()
        {
            ExternalPrograms Ext = new ExternalPrograms();
            bool Success = Ext.SelectFileDialog("", "Select database backup (sql) file");
            if (!Success)
                return false;

            if (!File.Exists(Ext.SelectedFile))
            {
                MessageBox.Show("Sorry, the file selected cannot be opened! Try another location or file.");
                return false;
            }
            FileName = Ext.SelectedFile;
            return true;
        }

        private void Load_File()
        {
            Data = File.ReadAllText(FileName);
            MessageBox.Show("Read " + Data.Length + " Bytes");
        }

        private void Enable_UI()
        {
            TextboxOldDomain.IsEnabled = true;
            TextboxNewDomain.IsEnabled = true;
        }

        private void Try_to_find_old_domain_name()
        {
            string UrlString = Data.HoleTextZwischenZweiStellen("'siteurl'", ")");
            if (UrlString.Length <= 0)
                return;
            if (!UrlString.Contains("http"))
                return;

            UrlString = UrlString.TrimStart(new char[]{ ',', ' ', '\''});
        
            UrlString = UrlString.HoleTextZwischenZweiStellen("", "',");

            TextboxOldDomain.Text = UrlString;
            MessageBox.Show ("Please check the old domain name");
        }

        private void TextboxOldDomain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Data.Length > 0)
            { 
                string Old = TextboxOldDomain.Text;
                if (Old.Length <= 0)
                    return;
                Find_all_matching_strings(Old);
                ListboxRegex.ItemsSource = Matches;
            }
        }

        private void Find_all_matching_strings(string domainName)
        {
            RegularExpression = "s:[0-9]+:\\\\\"" + domainName + "[^\\\\]+\\\\\"";
            Matches = Regex.Matches(Data, RegularExpression,
                                    RegexOptions.Singleline | 
                                    RegexOptions.CultureInvariant);
            Button_2.IsEnabled = (Matches.Count > 0);
        }

        private void TextboxNewDomain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Data.Length > 0)
            { 
                string New = TextboxNewDomain.Text;
                if (New.Length <= 0)
                    return;
                Change_the_file();
            }
        }

        private void Change_the_file()
        {
            string Old = TextboxOldDomain.Text;
            if (Old.Length <= 0)
                return;

            string New = TextboxNewDomain.Text;
            if (New.Length <= 0)
                return;

            int Offset = New.Length - Old.Length;

            NewData = Data;
            bool DoneSomething;
            do
            {
                DoneSomething = false;
                MatchCollection MatchesNew = Regex.Matches(NewData, RegularExpression,
                                                           RegexOptions.Singleline | 
                                                           RegexOptions.CultureInvariant);
                if (MatchesNew.Count == 0)
                    break;
                
                foreach (Match Match in MatchesNew)
                {
                    string OldExpr = Match.Value;
                    string NewExpr = Match.Value.Replace(Old, New);
                    string OldLengthText = OldExpr.HoleTextZwischenZweiStellen("s:", ":");
                    int    OldLength = Convert.ToInt32(OldLengthText);
                    string OldLengthValue = string.Format("s:{0}:", OldLength);
                    string NewLengthValue = string.Format("s:{0}:", OldLength + Offset);
                    NewExpr = NewExpr.Replace(OldLengthValue, NewLengthValue);
                    NewData = NewData.Replace(OldExpr, NewExpr);
                    DoneSomething = true;
                }
            }
            while (DoneSomething);

            // Check
            string NewRegularExpression = "s:[0-9]+:\\\\\"" + New + "[^\\\\]+\\\\\"";
            MatchCollection Output = Regex.Matches(NewData, NewRegularExpression,
                                                   RegexOptions.Singleline | 
                                                   RegexOptions.CultureInvariant);
            ListboxRegex2.ItemsSource = Output;
        }

        private void Button_2_Click(object sender, RoutedEventArgs e)
        {
            Change_the_file();

            if (Browse_for_file2() == false)
                return;

            Save_the_file();
        }

        private bool Browse_for_file2()
        {
            ExternalPrograms Ext = new ExternalPrograms();
            bool Success = Ext.SaveFileDialog("", "Enter new filename to save");
            if (!Success)
                return false;
            FileName = Ext.SelectedFile;
            return true;
        }

        private void Save_the_file()
        {
            if (File.Exists(FileName))
            {
                if (MessageBox.Show("Overwrite file?", "Question", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
            }
            File.WriteAllText(FileName, NewData);
            MessageBox.Show("File saved");
        }
    }
}
