using InternetHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace DotNetProDownloader
{
	class Program
    {
        static Downloader Dl = new Downloader();
        static string BaseUrl = "https://www.dotnetpro.de/";
        static List<Issue> _AllIssues = new List<Issue>();
        static List<Article> _AllArticles = new List<Article>();
        static Random _randomNumberGenerator = new Random((int)DateTime.Now.Ticks);

        static void Main(string[] args)
        {
            Dl.SetCookie(".dotnetpro.de", "plenigo_user", "93c39e2880a03f4f5f5ef8b89ce95e415d3b665a9f80b01e834d37982dfce757ab5177e43dd24cfd47c939ec554e20134a10|6cc1fd93194ed3f6785a8e2758fb95c57945c5081979ef66b30c267a2e9ae2e1baf8d4cc48b182224ba25608f61db3af9f5a73e579db1e065757be05a0ea75305084c1c07ef593eb236087bc088ff94a841c7c406ab21d2b2eba84f|0c8e5c07d258fb9954f81eb873d944fcda0b48b63fe9f561b97ebd581b52d6eef2e60ac7bc6edcb58cc4c23515df2bc7bbb0e2e5b6ee2987d2dc34b9def9630a87cd2be27e323cf3e046c25357b_1641128803162");
            Dl.SetCookie(".dotnetpro.de", "plenigo_check", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwdiI6MH0.eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9eyJwdiI6MH0");

            // We start with year 2021
            string archivUrl = BaseUrl + "hefte_data_2715721.html";
            Console.WriteLine($"Downloading archive years from URL {archivUrl}...");

            // this gives us all subsequent years until now
            List<YearDictionary> yearDicts = DownloadArchiveYears(archivUrl);
            foreach (var dict in yearDicts)
            {
                Console.WriteLine($"Found year {dict.Year} with URL {dict.Url}");
            }

            foreach (var dict in yearDicts)
            {
                Console.WriteLine($"Downloading year {dict.Year}:");
                if (dict.Year == "2003")
                {
                    var Issues = ProcessYear(dict);
                    _AllIssues.AddRange(Issues);
                }
            }

            foreach (var issue in _AllIssues)
            {
                Console.WriteLine($"Downloading issue {issue.Month}:");
                //if (issue.Month.StartsWith("1/") ||
                //    issue.Month.StartsWith("2/") ||
                //    issue.Month.StartsWith("3/") ||
                //    issue.Month.StartsWith("4/"))
                {
                    var Articles = ProcessIssue(issue);
                    _AllArticles.AddRange(Articles);
                }
            }

            foreach (var art in _AllArticles)
            {
                Console.Write($"Downloading Article {art.Number,2} - {art.Title,-50}: ");
                var downloadedAtLeastOneFile = ProcessArticle(art);
                Console.WriteLine();

                if (downloadedAtLeastOneFile)
                    WaitRandomTime();
            }

            Console.WriteLine("Finished! press any key to end");
            Console.ReadKey();
        }

		private static void WaitRandomTime()
		{
            //var seconds = _randomNumberGenerator.Next(15, 30); // wait a random time
			//Thread.Sleep(seconds * 1000);
		}

		private static List<YearDictionary> DownloadArchiveYears(string url)
        {
            var Result = new List<YearDictionary>();

            string Content = Dl.DownloadFromUrl(url);
            Regex Finder = new Regex("<li><a href=\"/hefte_data_[0-9]+.html\">[0-9]+</a></li>");
            foreach (var match in Finder.Matches(Content))
            {
                AddYear(Result, match.ToString());
            }

            // plus the first year
            AddYear(Result, $"{url}\">2001<");

            return Result;
        }

        private static void AddYear(List<YearDictionary> Result, string item)
        {
            var Dict = new YearDictionary();
            Regex Finder2 = new Regex("hefte_data_[0-9]+.html");
            Dict.Url = Finder2.Match(item).ToString();

            Regex Finder3 = new Regex(">[0-9]+<");
            Dict.Year = Finder3.Match(item).ToString().Trim(new char[] { '<', '>' });
            
            Result.Add(Dict);
        }

        private static List<Issue> ProcessYear(YearDictionary dict)
        {
            var Result = new List<Issue>();

            string Content = Dl.DownloadFromUrl(BaseUrl + dict.Url);

            Queue<string> Months = new Queue<string>();
            Regex Finder = new Regex("<div class=\"heftbox-ausgabe\">[0-9]+/[0-9]+</div>");
            foreach (var match in Finder.Matches(Content))
            {
                Months.Enqueue(ExtractMonth(match.ToString()));
            }

            Finder = new Regex("<a href=\"/hefte_data_[0-9]+.html\"><img");
            foreach (var match in Finder.Matches(Content))
            {
                Result.Add(CreateIssue(match.ToString(), 
                                       (Months.Count > 0) ? Months.Dequeue() : "???",
                                       dict.Year));
            }

            foreach (var issue in Result)
            {
                Console.WriteLine($"Found issue {issue.Month} with URL {issue.Url}");
            }
            return Result;
        }

        private static string ExtractMonth(string item)
        {
            Regex Finder2 = new Regex(">[0-9/]+<");
            return Finder2.Match(item).ToString().Trim(new char[] { '<', '>' });
        }

        private static Issue CreateIssue(string item, string month, string year)
        {
            var issue = new Issue();
            Regex Finder2 = new Regex("hefte_data_[0-9/]+.html");
            issue.Url = Finder2.Match(item).ToString().Trim(new char[] { '<', '>' });
            issue.Year = year;
            issue.Month = month;

            return issue;
        }

        private static List<Article> ProcessIssue(Issue issue)
        {
            issue.Content = Dl.DownloadFromUrl(BaseUrl + issue.Url);

            SaveIssueTableOfContents(issue);

            var Result = new List<Article>();

            int Counter = 1;
            Regex Finder = new Regex("<h2 class=\"headline \">");
            foreach (Match match in Finder.Matches(issue.Content))
            {
                Article art = TakeNextHref(issue, match);
                art.Number = Counter++;
                Result.Add(art);
                Console.WriteLine($"Found article {art.Number} '{art.Title}'");
            }

            return Result;
        }

		private static void SaveIssueTableOfContents(Issue issue)
		{
            if (issue.Month == "???")
                issue.Month = "00";
			try
			{
                var filename = $"DotnetPro Inhalt {issue.Year}-{issue.Month}.html";
                filename = filename.Replace('/', '-');
                File.WriteAllText(filename, issue.Content);
            }
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}
		}

		private static Article TakeNextHref(Issue issue, Match match)
        {
            // take next href
            int hrefStart = issue.Content.IndexOf("<a href=\"", match.Index);
            if (hrefStart > 0)
            {
                int hrefEnd = issue.Content.IndexOf("</a>", hrefStart);
                if (hrefEnd > hrefStart)
                {
                    string html = issue.Content.Substring(hrefStart, hrefEnd-hrefStart);
                    if (html.StartsWith("<a href=\""))
                        html = html.Substring("<a href=\"".Length);

                    int PosEnd = html.IndexOf('"');
                    if (PosEnd > 0)
                    {
                        Article a = new Article();
                        a.Year  = issue.Year;
                        a.Month = issue.Month;
                        int pos = a.Month.IndexOf("/");
                        if (pos > 0)
                            a.Month = a.Month.Substring(0, pos);
                        a.Month = a.Month.PadLeft(2, '0');
                        a.Url   = html.Substring(0, PosEnd);
                        a.Title = html.Substring(PosEnd+2);

                        int startOfArticleName = a.Title.LastIndexOf('>');
                        if (startOfArticleName > 0)
                            a.Title = a.Title.Substring(startOfArticleName+1);
                        return a;
                    }
                }
            }
            return null;
        }

        private static bool ProcessArticle(Article art)
        {
            var downloadedAndSavedAtLeastOne = false;
            art.Content = Dl.DownloadFromUrl(BaseUrl + art.Url);

            if (art.Content.Contains("Werden Sie jetzt dotnetpro-plus-Kunde"))
            {
                Console.Write($"\nFEHLER: Login als Abo-Kunde nicht erkannt!");
                return false;
            }

            Dictionary<string,string> Links = new Dictionary<string, string>();

            int posStart = art.Content.IndexOf("<div class=\"pdf-download\">");
            if (posStart > 0)
            {
                int Counter = 1;
                string Dokumente = art.Content.Substring(posStart);
                Regex PdfFinder = new Regex("href=\"[^\"]+\"");
                foreach (Match match in PdfFinder.Matches(Dokumente))
                {
                    string url = match.Value;
                    if (url.StartsWith("href="))
                        url = url.Substring("href=".Length);
                    url = url.Trim('"');

                    if (url.Contains("/autor") ||
                        url.Contains("mailto:"))
                    {
                        //Console.Write($"Found Link {url} ...stopping");
                        //break;
                    }

                    if (Links.ContainsKey(url))
                    {
                        //Console.Write($"Found duplicate Link {url} ...skipping");
                        continue;
                    }
                    if (url.ToLower().Contains(".pdf"))
					{
                        Links.Add(url, url);

                        Console.Write($"Found PDF file with URL '{url}'");
                        var downloadedAndSaved = Download_file_and_save(art, url);
                        Console.Write(" ok");

                        if (downloadedAndSaved)
                            downloadedAndSavedAtLeastOne = true;
					}
                    Counter++;
                }
            }
            return downloadedAndSavedAtLeastOne;
        }

        private static bool Download_file_and_save(Article art, string url)
        {
            string filename = "";
            try
			{
                string PdfName = "";
                int posPdfName = url.IndexOf("download=");
                if (posPdfName > 0)
                    PdfName = " - " + url.Substring(posPdfName + "download=".Length);
                else
                    PdfName = ".pdf";

                filename = $"{art.Year:4}-{art.Month:00} {art.Number:00} {art.Title}{PdfName}";
                filename = filename.Replace("  ", " ");
                filename = filename
                    .Replace('?', '_')
                    .Replace(':', '_')
                    .Replace('\\', '_')
                    .Replace('\n', '_')
                    .Replace('\r', '_')
                    .Replace('/', '_')
                    .Replace('<', '_')
                    .Replace('>', '_')
                    .Replace('?', '_');

                if (!File.Exists(filename))
				{
                    var fileContents = Dl.DownloadBinary(url);
                    File.WriteAllBytes(filename, fileContents);
                    return true;
				}
                return false;
			}
            catch (Exception ex)
			{
                Console.WriteLine($"\nERROR: Download gescheitert:\n{ex.ToString()}");
                Console.WriteLine($"\nERROR: filename was '{filename}'");
                return false;
			}
        }
    }

    class YearDictionary
    {
        public string Year;
        public string Url;
    }

    class Issue
    {
        public string Year;
        public string Month;
        public string Url;
        public string Content;
    }

    class Article
    {
        public string Year;
        public string Month;
        public string Url;
        public string Filename;
        public string Title;
        internal int Number;
        internal string Content;
    }
}
