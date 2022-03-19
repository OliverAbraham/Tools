using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using JustAgile.Html.Linq;


namespace CannaPowerDirectoryDownloader
{
	public partial class MainWindow : Window
	{
		#region ------------- Fields --------------------------------------------------------------
		private string      _outputFilename      = "CDs.csv";
        private static int  _pageNumber          = 1;
        private static bool _gotDataFromSite     = false;
        private bool        _stopTheWorkerThread = false;
        private Thread      _workerThread;
		#endregion



		#region ------------- Init ----------------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();
            _workerThread = new Thread(DownloaderThread);
        }
		#endregion



		#region ------------- Implementation ------------------------------------------------------
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            _workerThread.Start();
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            _stopTheWorkerThread = true;
        }

        private void DownloaderThread()
        {
            if (File.Exists(_outputFilename))
                File.Delete(_outputFilename);

            do
            {
                LoadOnePage(_pageNumber++);
                SelectLastRowAndMakeItVisible();
                Thread.Sleep(10000);
            } 
            while (_gotDataFromSite && !_stopTheWorkerThread);
            
            InsertRowIntoListbox("Stopped");
        }



        private void InsertRowIntoListbox(string Zeile)
        {
            listBox1.Dispatcher.Invoke(new UpdateTextCallback(this.InsertRowIntoListbox_Delegate), new object[] { Zeile });
        }

        public delegate void UpdateTextCallback(string text);
        
        private void InsertRowIntoListbox_Delegate(string text)
        {
            listBox1.Items.Add(text);
        }

        private void SelectLastRowAndMakeItVisible()
        {
            listBox1.Dispatcher.Invoke(new UpdateTextCallback(this.SelectLastRowAndMakeItVisible_Delegate), new object[] { "" });
        }
        
        private void SelectLastRowAndMakeItVisible_Delegate(string text)
        {
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            object o = listBox1.SelectedItem;
            if (o != null)
                listBox1.ScrollIntoView(o);
        }

        

        private void LoadOnePage (int page)
        {
            HDocument document = LoadHtmlPage(page);

            List<CD> rows = ParsePage(document);

            if (rows.Count > 0)
            {
                InsertRowsIntoListbox(rows);
    			UpdateProgress();
                _gotDataFromSite = true;
            }
            else
                _gotDataFromSite = false;
        }

        private void InsertRowsIntoListbox(List<CD> rows)
		{
			foreach (CD row in rows)
			{
				string Temp = $"{Format(row.Artist)};{Format(row.Format)};{Format(row.Uploader)};{Format(row.Datum)}";
				File.AppendAllText(_outputFilename, Temp + "\n");
				InsertRowIntoListbox(Temp);
			}
		}

		private void UpdateProgress()
		{
			LabelProgress.Dispatcher.Invoke(new UpdateTextCallback(this.FillProgress), new object[] { "" });
		}

		private void FillProgress(string text)
		{
            LabelProgress.Content = $"Seite {_pageNumber}";
		}

		private string Format(string value)
		{
			return $"\"{value.Replace('"', '_')}\"";
        }

		private HDocument LoadHtmlPage(int page)
        {
            //string Seite = "http://uu.canna.to/cpuser/links.php?action=kategorie&kat_id=5&sort=datum asc&alpha=&page=" + seitennummer.ToString();
            string Seite = "http://uu.canna.to/links.php?action=kategorie&kat_id=5&sort=filename asc&alpha=&format=&page=" + page.ToString();
            return HDocument.Load(Seite);
        }

        private List<CD> ParsePage(HDocument document)
        {
            List<CD> Cds = new List<CD>();
            int tableNumber = 1;
            foreach (HElement table in document.Descendants("table"))
            {
                string value = table.Value;
                value = value.Trim('\r', '\n', '\t', ' ').Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
                if (tableNumber++==7)
                {
                    int rowNumber = 1;
                    foreach (HElement row in table.Descendants("tr"))
                    {
                        if (rowNumber>=2 && rowNumber<=26)
                        {
                            var cd = new CD();
                            int Part = 1;
                            foreach (HElement Teil in row.Descendants("td"))
                            {
                                string cell = Teil.Value.Trim('\r', '\n', '\t', ' ').Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
                                switch (Part++)
                                {
                                    case 1: cd.Artist     = cell; break;
                                    case 2: cd.Format     = cell; break;
                                    case 3: cd.Uploader   = cell; break;
                                    case 4: cd.Downloads  = cell; break;
                                    case 5: cd.Datum      = cell; break;
                                }
                            }
                            Cds.Add(cd);
                        }
                        rowNumber++;
                    }
                }
            }
            return Cds;
        }
		#endregion
    }
}
