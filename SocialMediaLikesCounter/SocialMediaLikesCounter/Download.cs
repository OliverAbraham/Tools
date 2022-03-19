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
using System.Net.Http;

namespace Abraham.Internet
{
    public class Downloader
    {
        public string Get(string url, int timeout_in_milliseconds = -1)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.CookieContainer = new CookieContainer();
            httpRequest.AllowAutoRedirect = true;
            if (timeout_in_milliseconds != -1)
                httpRequest.Timeout = timeout_in_milliseconds;
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
                    byte[] seiteninhalt = new byte[10000];
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

        async public void Post(string url, string req, int timeout = -1)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            var request = new HttpRequestMessage(HttpMethod.Post, req);

            var requestContent = "/me?fields=id,name&amp;access_token={EAACEdEose0cBACEk0AfHkRQaExqN2SxIy9gWSivX8nGI2nZBSDqQZB9TsGEM4b3rVQn5Hv4UunMw3Lm9WZCzW9E5JM5uA020ST5auQ22g5OCkCMtBopwjZCLh3hwV6KSGDZCXKJHSE4i1JzZB66Rp87eQL7TRC142BlqnBPsbr7DhMjbh2QAWgm8w26Tj9mdEZD}";

            request.Content = new StringContent(requestContent, Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await client.SendAsync(request);
        }
    }
}
