﻿using System;
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
using System.Windows.Shapes;

namespace SocialMediaLikesCounter
{
	/// <summary>
	/// Interaktionslogik für LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		public LoginWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Browser.Navigated += Browser_Navigated;
			Browser.Navigate("http://www.facebook.com/login");
		}

		private void Browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
		}
	}
}
