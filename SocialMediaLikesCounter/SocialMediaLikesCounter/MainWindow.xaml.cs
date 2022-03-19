using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using InternetHelper;
using Facebook;
using System.Windows.Threading;
using Abraham.Internet;
using System.Diagnostics;

namespace SocialMediaLikesCounter
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
		private string Facebook_App_ID = "1453774798017495";

        private string UserId = "100000304119066";
        private string AccessTokenForManagePages = "EAACEdEose0cBAGC14WN0IeyYDaeKU9ZCxzZAIm0KSGASRf0vDrZAWgse3D0MVEDk4SZCgRex3RRR9YHoUGCSF1qlFQZAwkhqhDoHWlZAI0VNne2hPcXZC12s0VKVf858E02mvZBIEzlxAIPZAF2i5kBm8Ji32EXA8NuadsCDKlUcvg59lTGEVek4vu1KjZBLZChvnwZD";

        private string PageId = "692932557462943";
		// private string PageAccessToken = EAACEdEose0cBACEk0AfHkRQaExqN2SxIy9gWSivX8nGI2nZBSDqQZB9TsGEM4b3rVQn5Hv4UunMw3Lm9WZCzW9E5JM5uA020ST5auQ22g5OCkCMtBopwjZCLh3hwV6KSGDZCXKJHSE4i1JzZB66Rp87eQL7TRC142BlqnBPsbr7DhMjbh2QAWgm8w26Tj9mdEZD";
        private string PageAccessToken = "EAACEdEose0cBACeWeiZCPA8loZAbcdEEzpxQjfwwZBdYcpbu5iCKqYflQa4C9JzueCZB7QethKQozsDrZCZBY2sFNvlwYfUc1V8Jy8eOw6POb0oXS1ZB5ZBwMUwUkSeEDZBVug9FLdRX6ZCVKvvfcOBEnvMsoZAfMdQ5bKl5MQu2dbqJnIlZArkseIJHhekNzPoo96QZD"; 

		private int PollingIntervallInSeconds = 5;

		private DispatcherTimer _MyTimer;

		public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Open_facebook_login_screen();
			//StartTimer();
			//Login_to_facebook();
			//RefreshView();
		}

		private void Get_facebook_page_as_anonymous_user()
		{
			Downloader Dl = new Downloader();
			int timeout_in_seconds = 60;
			string Response = Dl.Get("http://www.facebook.com/lentfoehrdenopenair", timeout_in_seconds * 1000);
		}

		private void Open_facebook_login_screen()
		{
			LoginWindow Win = new LoginWindow();
			Win.ShowDialog();

			////string Url = "http://www.facebook.com/v2.9/dialog/oauth?client_id=1453774798017495&redirect_uri=\"http://www.facebook.com/lentfoehrdenopenair\"";
			//string Url = "http://www.facebook.com";
			//Process.Start("iexplore.exe", Url);
		}

		private void Get_facebook_account()
		{
			string Accounts = GetAccount(UserId, AccessTokenForManagePages);
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			StopTimer();
		}

		private void StartTimer()
		{
			_MyTimer = new DispatcherTimer();
			_MyTimer.Interval = new TimeSpan(0,0,PollingIntervallInSeconds);
			_MyTimer.Tick += _MyTimer_Tick;
			_MyTimer.Start();
		}

		private void StopTimer()
		{
			_MyTimer.Stop();
		}

		private void _MyTimer_Tick(object sender, EventArgs e)
		{
			Get_facebook_account();
			//RefreshView();
		}

		private void RefreshView()
		{
			string Info = GetPageInformation(PageId, PageAccessToken);
			if (!string.IsNullOrWhiteSpace(Info))
			{
				string Likes = GetJson(Info, "fan_count");
				Output.Content = Likes;
			}
		}

		private string GetAccount(string facebookUserId, string accessToken)
        {
			try
			{
				Facebook.FacebookClient c = new FacebookClient();
				c.AccessToken = accessToken;
				string json = c.Get(facebookUserId + "?me/accounts").ToString();
				return json;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				return "";
			}
        }

        private string GetPageInformation(string facebookPageId, string pageAccessToken)
        {
			try
			{
				Facebook.FacebookClient c = new FacebookClient();
				c.AccessToken = pageAccessToken;
				string PageInfo = c.Get("v2.9/" + facebookPageId + "?fields=fan_count").ToString();
				return PageInfo;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				return "";
			}
        }

        private string GetJson(string info, string token)
        {
            int Pos = info.IndexOf(token);
            if (Pos == -1)
                return "";
            string Start = info.Substring(Pos + token.Length);
            Start = Start.Trim(':');
            Start = Start.Trim('\"');
            Start = Start.Trim(':');

            int End = Start.IndexOf('\"');
            if (End== -1)
                return "";
            string Content = Start.Substring(0, End);
            Content = Content.Trim(':');
            Content = Content.Trim('\"');
            Content = Content.Trim(',');
            return Content;
        }

        private void image_Loaded(object sender, RoutedEventArgs e)
        {
            string Directory = Environment.CurrentDirectory;
            string FullPath = Directory + "\\" + "facebooklogo.jpg";
            ImageSource imageSource = new BitmapImage(new Uri(FullPath));
            image.Source = imageSource;
        }
	}
}
