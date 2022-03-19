using System.IO;

namespace DotNetProInhaltsverzeichnis
{
	public class HtmlGenerator
	{
		public string Filename { get; set; }
		public string Style { get; set; }
		public string Title { get; set; }

		public HtmlGenerator()
		{
			Filename = "output.html";
			Style = $" style=\"font-family: sans-serif;\"";
			Title = "MyTitle";
		}

		public void WriteHeader()
		{
			File.WriteAllText(Filename, $"<html>\r\n<head></head>");
		}

		public void StartBody()
		{
			File.AppendAllText(Filename, $"<body {Style}>\r\n<h1>{Title}</h1>");
		}

		public void EndBody()
		{
			File.AppendAllText(Filename, "</body>\r\n");
		}

		public void WriteFooter()
		{
			File.AppendAllText(Filename, "</html>\r\n");
		}

		public void StartTable()
		{
			File.AppendAllText(Filename, "<table>\r\n");
		}

		public void EndTable()
		{
			File.AppendAllText(Filename, "</table>\r\n");
		}

		public void WriteListRowStart()
		{
			File.AppendAllText(Filename, $"<tr>\r\n");
		}

		public void WriteListRowEnd()
		{
			File.AppendAllText(Filename, $"</tr>\r\n");
		}

		internal void WriteTableHeadingRowStart()
		{
			File.AppendAllText(Filename, $"<tr>\r\n");
		}

		internal void WriteTableHeadingColumn(string text)
		{
			File.AppendAllText(Filename, $"<th>{text}</th>\r\n");
		}

		internal void WriteTableHeadingRowEnd()
		{
			File.AppendAllText(Filename, $"</tr>\r\n");
		}

		public void WriteListColumn(string value)
		{
			File.AppendAllText(Filename, $"<td>{value}</td>\r\n");
		}

		public void SaveAndClose()
		{
		}
	}
}
