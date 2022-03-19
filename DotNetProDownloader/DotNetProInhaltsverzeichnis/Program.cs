using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetProInhaltsverzeichnis
{
	class Program
	{
		static HtmlGenerator _output;

		static void Main(string[] args)
		{
			PrintGreeting();
			InitOutputFile();
			File.WriteAllText("nicht-aufgeloest.txt", "");

			ReadAndProcessAllInputFiles();
			CloseOutputFile();
		}

		private static void PrintGreeting()
		{
			Console.WriteLine("DotNetPro Inhaltsverzeichnis aufbereiten");
		}

		private static void ReadAndProcessAllInputFiles()
		{
			var inputPath = @"N:\Zeitschriften\DotNetPro";
			var filesArray = Directory.GetFiles(inputPath, "*.html");// DotnetPro Inhalt 2021-12-2021.html");
			var files = new List<string>(filesArray);
			files.Sort();
			foreach (var file in files)
			{
				ProcessFile(file);
			}
		}

		private static void ProcessFile(string filename)
		{
			try
			{
				var filenameBody = Path.GetFileName(filename);
				var monat = filenameBody.Substring("DotnetPro Inhalt 2003-".Length, 2);

				var regex = new Regex("[0-9]+");
				var jahr = regex.Match(filename)?.Value;

				var contents = File.ReadAllText(filename, Encoding.UTF8);
				
				var Doc = new HtmlDocument();
				Doc.LoadHtml(contents);

				var body = Doc.DocumentNode.SelectSingleNode("//body");
				if (body == null)
					return;

				var articleLists = body.SelectNodes($".//div[contains(@class, 'artikelliste')]");
				if (articleLists == null || articleLists.Count == 0)
					return;
				var articleList = articleLists[0];

				var allArticles = articleList.SelectNodes($".//div[contains(@class, 'list_item')]");
				if (allArticles == null || allArticles.Count == 0)
					return;

				foreach(var article in allArticles)
				{
					ProcessArticle(article, jahr, monat);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Problem mit Datei '{filename}: {ex.ToString()}'");
			}
		}

		private static void ProcessArticle(HtmlNode article, string jahr, string monat)
		{
			// XPATH syntax: READ THIS!!!
			// https://www.w3schools.com/xml/xpath_syntax.asp
			//
			// some details:
			//    nodename 	Selects all nodes with the name "nodename"
			//    / 	    Selects from the root node
			//    // 	    Selects nodes in the document from the current node that match the selection no matter where they are
			//    . 	    Selects the current node
			//    .. 	    Selects the parent of the current node
			//    @ 	    Selects attributes

			var rowDiv         = article.SelectSingleNode($".//div[contains(@class, 'row')]");


			var dachzeileNode  = rowDiv.SelectSingleNode($".//div[contains(@class, 'dachzeile')]");
			string dachzeile   = FetchInnerText(dachzeileNode);

			var datumNode      = rowDiv.SelectSingleNode($".//div[contains(@class, 'datum')]");
			var datum          = FetchInnerText(datumNode);


			var headlineNode   = rowDiv.SelectSingleNode($".//h2");
			var headline       = FetchInnerText(headlineNode);
			var headlineAnchor = headlineNode.SelectSingleNode($".//a");
			var headlineLink   = headlineAnchor.GetAttributeValue("href", "");


			var vorspannNode   = rowDiv.SelectSingleNode($".//div[contains(@class, 'vorspann')]");
			var vorspann       = FetchInnerText(vorspannNode);
			vorspann           = RemoveReadMore(vorspann);

			GenerateNewListItem(dachzeile, datum, headline, headlineLink, vorspann, jahr, monat);
		}

		private static string FetchInnerText(HtmlNode node)
		{
			if (node == null)
				return "";
			var text = node.InnerText;
			if (text == null)
				return "";
			return text.Trim(new char[] { '\n', '\r', '\t', ' ' });
		}

		private static string RemoveReadMore(string text)
		{
			while (true)
			{
				var newText = text.Replace("weiterlesen", "");
				newText = newText.Replace("&nbsp;", "");
				newText = newText.Trim(new char[] { '\n', '\r', ' ' });
				if (newText.Equals(text) || newText == null)
					return newText;
				text = newText;
			}
		}

		private static void InitOutputFile()
		{
			_output = new HtmlGenerator();
			_output.Title = "DotNetPro Inhaltsverzeichnis";
			_output.WriteHeader();
			_output.StartBody();
			_output.StartTable();

			_output.WriteTableHeadingRowStart();
			_output.WriteTableHeadingColumn("Jahr");
			_output.WriteTableHeadingColumn("Datum");
			_output.WriteTableHeadingColumn("Headline");
			_output.WriteTableHeadingColumn("PDF");
			_output.WriteTableHeadingColumn("Inhalt");
			//_output.WriteTableHeadingColumn("Dachzeile");
			_output.WriteTableHeadingRowEnd();
		}

		private static void CloseOutputFile()
		{
			_output.EndTable();
			_output.EndBody();
			_output.WriteFooter();
			_output.SaveAndClose();
		}

		private static void GenerateNewListItem(string dachzeile, string datum, string headline, string headlineLink, string vorspann, string jahr, string monat)
		{
			var filenamePart = FindPdfFileInDirectory(headlineLink, jahr, monat);

			var completeLink = $"<a href=\"{filenamePart}\">PDF</a>";

			_output.WriteListRowStart();
			_output.WriteListColumn(jahr);
			_output.WriteListColumn(datum);
			_output.WriteListColumn(headline);
			_output.WriteListColumn(completeLink);
			_output.WriteListColumn(vorspann);
			//_output.WriteListColumn(dachzeile);
			_output.WriteListRowEnd();
		}

		private static string FindPdfFileInDirectory(string headlineLink, string jahr, string monat)
		{
			var prefix = $"{jahr}-{monat}";

			var folderName = @"N:\\Zeitschriften\\DotNetPro\\" + jahr;
			var allFiles = Directory.GetFiles(folderName, "*");

			var filename          = FetchFilenameFromPath(headlineLink);
			var nameWithoutNumber = FetchNameWithoutNumber(filename);
			var firstWord         = FetchFirstWordFromFilename(filename);

			var realFilename ="";
			//if (FileExists(filename, allFiles, out realFilename))
			//	return realFilename;

			if (FileExists(nameWithoutNumber, prefix, allFiles, out realFilename))
				return realFilename;

			if (FileExists(firstWord, prefix, allFiles, out realFilename))
				return realFilename;

			var part = nameWithoutNumber;
			while(true)
			{
				part = part.Substring(0, part.Length-1);
				if (part.Length == 0)
					break;
				if (FileExists(part, prefix, allFiles, out realFilename))
				    return realFilename;
			}

			File.AppendAllText("nicht-aufgeloest.txt", $"{jahr}-{monat}      {headlineLink}\r\n");
			return "???";
		}

		private static bool FileExists(string filename, string prefix, string[] allFiles, out string realFilename)
		{
			var compareName = ReplaceSpecialCharacters(filename.Replace(" ", "").Replace(".pdf", ""));

			realFilename = "";
			int counter = 0;
			foreach(var file in allFiles)
			{
				var filenameOnly = Path.GetFileName(file);
				filenameOnly = filenameOnly
					.Replace(" ", "")
					.Replace(".pdf", "");
				var strippedFilename = ReplaceSpecialCharacters(filenameOnly);
				strippedFilename = strippedFilename.Substring(8);

				if (filenameOnly.StartsWith(prefix))
					if (strippedFilename.Contains(compareName))
					{
						realFilename = file;
						counter++;
					}
			}
			if (counter == 1)
				return true;
			else
				return false;
		}

		private static string ReplaceSpecialCharacters(string text)
		{
			return text
				.ToLower()
				.Replace("ß", "ss")
				.Replace("ä", "ae")
				.Replace("ö", "oe")
				.Replace("ü", "ue")
				.Replace("Ä", "AE")
				.Replace("Ö", "OE")
				.Replace("Ü", "UE")
				.Replace("-", "")
				.Replace("_", "")
				.Replace(",", "")
				.Replace(";", "")
				.Replace("–", "")
				.Replace("'", "")
				.Replace("!", "")
				.Replace("´", "")
				.Replace("`", "")
				.Replace("(", "")
				.Replace(")", "")
				.Replace("für", "")
				.Replace("fuer", "")
				.Replace("mit", "")
				.Replace("und", "");
		}

		private static string FetchFilenameFromPath(string filename)
		{
			int pos = filename.LastIndexOf('/');
			if (pos > 0)
				return filename.Substring(pos+1)?.Replace(".html", "");
			else
				return filename;
		}

		private static string FetchNameWithoutNumber(string filename)
		{
			int pos = filename.LastIndexOf('-');
			if (pos > 0)
				return filename.Substring(0,pos)?.Trim();
			else
				return filename;
		}

		private static string FetchFirstWordFromFilename(string filename)
		{
			var parts = filename.Split(new char[] { ' ', '\t', '_', '-', '&' });
			if (parts.GetLength(0) > 0)
				return parts[0];
			else
				return filename;
		}
	}
}
