//-------------------------------------------------------------------------------------------------
// Klasse zum Downloaden einer Internetseite
// Oliver Abraham, Inveos GmbH, 4/2010
// Siehe auch: http://www.codeproject.com/KB/cs/SeansDownloader.aspx
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace InternetHelper
{
    public class Downloader
    {
        private class MyCookie
		{
           public string CookieDomain = "";
           public string CookieID     = "";
           public string CookieValue  = "";
		}

        private List<MyCookie> _cookies = new List<MyCookie>();

        public void SetCookie(string cookieDomain, string cookieID, string cookieValue)
        {
            var cookie = new MyCookie()
			{
                CookieDomain = cookieDomain,
                CookieID = cookieID,
                CookieValue = cookieValue
			};
            _cookies.Add(cookie);
        }

        public string DownloadFromUrl(string url, int timeout_in_seconds = 30)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.CookieContainer = new CookieContainer();
            foreach(var cookie in _cookies)
                httpRequest.CookieContainer.Add(new Cookie(cookie.CookieID, cookie.CookieValue, "", cookie.CookieDomain));

            httpRequest.AllowAutoRedirect = true;
            httpRequest.Timeout = timeout_in_seconds * 1000;
            string Seiteninhalt = "";

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                byte[] httpHeaderData = httpResponse.Headers.ToByteArray();
                Stream httpContentData = httpResponse.GetResponseStream();
                using (httpContentData)
                {
                    Encoding enc = Encoding.UTF8;
                    int AnzahlGelesen;
                    byte[] seiteninhalt = new byte[1000000];
                    do
                    {
                        AnzahlGelesen = httpContentData.Read(seiteninhalt, 0, seiteninhalt.Length);
                        Seiteninhalt += enc.GetString(seiteninhalt, 0, AnzahlGelesen);
                    }
                    while (AnzahlGelesen > 0);
                }
            }
            httpResponse.Close();
            return Seiteninhalt;
        }

        internal byte[] DownloadBinary(string url, int timeout_in_seconds = 30)
        {
            byte[] Content = new byte[100_000_000];
            int Index = 0;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.CookieContainer = new CookieContainer();
            foreach(var cookie in _cookies)
                httpRequest.CookieContainer.Add(new Cookie(cookie.CookieID, cookie.CookieValue, "", cookie.CookieDomain));

            httpRequest.AllowAutoRedirect = true;
            httpRequest.Timeout = timeout_in_seconds * 1000;

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                byte[] httpHeaderData = httpResponse.Headers.ToByteArray();
                Stream httpContentData = httpResponse.GetResponseStream();
                using (httpContentData)
                {
                    int AnzahlGelesen;
                    byte[] seiteninhalt = new byte[100000];
                    do
                    {
                        AnzahlGelesen = httpContentData.Read(seiteninhalt, 0, seiteninhalt.Length);
                        for(int i=0; i< AnzahlGelesen; i++)
                            Content[Index++] = seiteninhalt[i];
                    }
                    while (AnzahlGelesen > 0);
                }
            }
            httpResponse.Close();

            byte[] Result = new byte[Index];
            for (int i = 0; i < Index; i++)
                Result[i] = Content[i];

            return Result;
        }
    }
}

