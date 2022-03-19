using Abraham.Text;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CodeFormatterGUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CodeFormatter Formatter = new CodeFormatter();
        long LetzteEingabeOben  = 0;
        long LetzteEingabeUnten = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Textbox_Oben_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Textbox_Oben == null || Textbox_Unten == null)
                return;
            if (VerstricheneZeit(LetzteEingabeUnten) < 1000)
                return;
            LetzteEingabeOben = DateTime.Now.Ticks;

            Textbox_Unten.Text = Formatter.Format(Textbox_Oben.Text);
        }

        long VerstricheneZeit(long ticks)
        {
            return (DateTime.Now.Ticks - ticks) / 10000;
        }
    }
}
