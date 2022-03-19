using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CommandLine;

namespace DropboxCommandLineDownloader
{
	class Program
	{
        #region ------------- Command line options ------------------------------------------------

        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('s', "source", Required = true, HelpText = "Sets the source folder for copying (a dropbox folder)")]
            public string SourceFolder { get; set; }

            [Option('d', "destination", Required = true, HelpText = "Sets the destination folder for copying (a folder on a local drive)")]
            public string DestinationFolder { get; set; }

            [Option('m', "move", Required = false, HelpText = "move files instead of copying")]
            public bool MoveFiles { get; set; }
        }

        #endregion



		#region ------------- Fields --------------------------------------------------------------

		private static Options _CommandLineOptions;
		private static string _DropboxClient     = "dbxcli-windows-amd64.exe";
		private static int _WaitTimeoutInSeconds = 300;

		#endregion



		#region ------------- Init ----------------------------------------------------------------

		static void Main(string[] args)
		{
			ParseCommandLine(args);
			DisplayUsage();
			if (!ParametersAreAllCorrect())
				return;
			DisplayParameters();
			
			List<string> files = EnumerateAllFiles();

			DownloadFiles(files);
		}

		#endregion



		#region ------------- Implementation ------------------------------------------------------

		private static bool ParametersAreAllCorrect()
		{
			if (_CommandLineOptions == null)
			{
				Console.WriteLine("Error: Some options are missing!");
				return false;
			}

			if (string.IsNullOrWhiteSpace(_CommandLineOptions.SourceFolder))
			{
				Console.WriteLine("Error: The source folder is missing!");
				return false;
			}

			if (string.IsNullOrWhiteSpace(_CommandLineOptions.DestinationFolder))
			{
				Console.WriteLine("Error: The destination folder is missing!");
				return false;
			}

			if (!Directory.Exists(_CommandLineOptions.DestinationFolder))
			{
				Console.WriteLine("Error: The destination folder doesn't exist on your hard drive");
				return false;
			}
			return true;
		}

		private static void ParseCommandLine(string[] args)
		{
			Parser.Default.ParseArguments<Options>(args)
				.WithParsed<Options>(o =>
				{
					if (!o.Verbose)
						Console.WriteLine($"Current Arguments: -v {o.Verbose}");
					_CommandLineOptions = o;
				});
		}

		private static void DisplayUsage()
		{
			Console.WriteLine($"-----------------------------------------------------------------------------");
			Console.WriteLine($"DropboxCommandLineDownloader - Version 2");
			Console.WriteLine($"-----------------------------------------------------------------------------");
			Console.WriteLine($"");
			Console.WriteLine($"Oliver Abraham 2020, www.oliver-abraham.de, mail@oliver-abraham.de");
			Console.WriteLine($"This program is free to use.");
			Console.WriteLine($"");
			Console.WriteLine($"This program uses the dbxcli command line tool to move files ");
			Console.WriteLine($"from a dropbox folder to a local folder.");
			Console.WriteLine($"Check that this program (the dropbox command line interface) exists:");
			Console.WriteLine($"dbxcli-windows-amd64.exe");
			Console.WriteLine($"");
			Console.WriteLine($"Usage:");
			Console.WriteLine($"DropboxCommandLineDownloader   --help");
			Console.WriteLine($"DropboxCommandLineDownloader   [DropboxFolder]   [LocalFolder]  [Options]");
			Console.WriteLine($"");
			Console.WriteLine($"Example:");
			Console.WriteLine(@$"DropboxCommandLineDownloader   -source=Apps/InpsydeBackWPup   -destination=C:\Test");
			Console.WriteLine($"");
			Console.WriteLine($"");
		}

		private static void DisplayParameters()
		{
			Console.WriteLine($"Dropbox client    : {_DropboxClient}");
			Console.WriteLine($"Dropbox timeout   : {_WaitTimeoutInSeconds} seconds");
			Console.WriteLine($"Source folder     : {_CommandLineOptions.SourceFolder}");
			Console.WriteLine($"Destination folder: {_CommandLineOptions.DestinationFolder}");
			Console.WriteLine($"Move files option : {_CommandLineOptions.MoveFiles}");
		}

		private static List<string> EnumerateAllFiles()
		{
			Console.WriteLine("");
			Console.WriteLine("");
			Console.WriteLine("Reading all files from Dropbox:");
			List<string> files = DownloadDirectoryListingOfDropboxFolders(_CommandLineOptions.SourceFolder);
			return files;
		}

		private static void DownloadFiles(List<string> files)
		{
			Console.WriteLine("");
			Console.WriteLine("");
			Console.WriteLine("Copying all files from Dropbox:");
			DownloadAllFilesFromDropboxToLocalFolder(files, _CommandLineOptions.DestinationFolder);
		}

		private static void DisplayAllFiles(List<string> files)
		{
			Console.WriteLine($"contents:");
			foreach (var file in files)
				Console.WriteLine(GetPartOfStringAfter(file, _CommandLineOptions.SourceFolder));
		}

		private static List<string> DownloadDirectoryListingOfDropboxFolders(string sourceFolder)
		{
			var files = new List<string>();
			DownloadDirectoryListingOfDropboxFolders(sourceFolder, files);
			return files;
		}

		private static void DownloadDirectoryListingOfDropboxFolders(string folder, List<string> results)
		{
			var command = $"ls -l \"{folder}\"";
			var filesAndFolders = DropboxApiCommand(command);
			var lines = filesAndFolders.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var line in lines)
			{
				var fileOrFolder = GetPartOfStringAfter(line, "/");
				if (string.IsNullOrWhiteSpace(fileOrFolder) ||
					fileOrFolder.StartsWith("Revision"))
					continue;
				if (line.StartsWith("-"))
				{
					DownloadDirectoryListingOfDropboxFolders(fileOrFolder, results);
				}
				else
				{
					results.Add(fileOrFolder);
					Console.WriteLine(fileOrFolder);
				}
			}
		}

		private static void DownloadAllFilesFromDropboxToLocalFolder(List<string> files, string destinationFolder)
		{
			if (!Directory.Exists(_CommandLineOptions.DestinationFolder))
				Directory.CreateDirectory(_CommandLineOptions.DestinationFolder);

			foreach(var file in files)
			{
				DownloadFileFromDropboxToLocalFolder(file, destinationFolder);
			}
		}

		private static bool DownloadFileFromDropboxToLocalFolder(string PathAndFilename, string destinationFolder)
		{
			string DestinationFilename = BuildDestinationFilename(PathAndFilename, destinationFolder);

			if (FileAlreadyExistsOnDestination(DestinationFilename))
				return true;

			string DestinationFolder = Path.GetDirectoryName(DestinationFilename);

			CreateSubfoldersIfNecessary(DestinationFolder);

			if (!DownloadFileFromDropboxToFolder(PathAndFilename, DestinationFilename, DestinationFolder))
				return false;

			if (_CommandLineOptions.MoveFiles)
				RemoveFileFromSource(PathAndFilename);

			return true;
		}

		private static bool FileAlreadyExistsOnDestination(string DestinationFilename)
		{
			if (File.Exists(DestinationFilename))
			{
				Console.WriteLine($"File already exists:  {DestinationFilename}");
				return true;
			}
			return false;
		}

		private static void CreateSubfoldersIfNecessary(string DestinationFolder)
		{
			CreateSubDirectories(DestinationFolder);
		}

		private static void CreateSubDirectories(string dir)
		{
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
		}

		private static string BuildDestinationFilename(string PathAndFilename, string destinationFolder)
		{
			string WindowsPathAndFileName;
			if ('/' != Path.DirectorySeparatorChar)
				WindowsPathAndFileName = PathAndFilename.Replace('/', Path.DirectorySeparatorChar);
			else
				WindowsPathAndFileName = PathAndFilename;
			var ResultingFilename = destinationFolder + Path.DirectorySeparatorChar + WindowsPathAndFileName;
			return ResultingFilename;
		}

		private static bool DownloadFileFromDropboxToFolder(string PathAndFilename, string DestinationFilename, string DestinationFolder)
		{
			Console.WriteLine($"Downloading        :  {PathAndFilename}   to   {DestinationFolder}");
			var command = $"get   {'\"'}{PathAndFilename}{'\"'}   {'\"'}{DestinationFolder}{'\"'}";
			var result = DropboxApiCommand(command);

			if (!File.Exists(DestinationFilename))
			{
				Console.WriteLine("Error: download was done, but destination file doesn't exist!");
				Console.WriteLine(DestinationFilename);
				Console.WriteLine("Please check the following command by hand ith the dropbox client:");
				Console.WriteLine(command);
				return false;
			}
			return true;
		}

		private static void RemoveFileFromSource(string PathAndFilename)
		{
			Console.WriteLine($"Deleting source file from dropbox");
			var command2 = $"rm   {'\"'}{PathAndFilename}{'\"'}";
			var result2 = DropboxApiCommand(command2);
		}

		#endregion



		#region ------------- Dropbox API commands ------------------------------------------------

		private static string DropboxApiCommand(string command)
		{
			try
			{
				return CallProcessAndReturnConsoleOutput(_DropboxClient, command);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception calling the dropbox command line interface!");
				Console.WriteLine($"Tried to start the program '{_DropboxClient}'");
				Console.WriteLine(e.ToString());
				throw;
			}
		}

		private static string CallProcessAndReturnConsoleOutput(string filename, string arguments)
        {
			if (_CommandLineOptions.Verbose)
				Console.WriteLine($"calling '{filename}' with arguments '{arguments}' and timeout {_WaitTimeoutInSeconds} seconds");

            string Output = "";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = filename;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                p.Start();

                //Output = p.StandardOutput.ReadToEnd();
                int MaxLineCount = 1000;
                DateTime Timeout = DateTime.Now.AddSeconds(_WaitTimeoutInSeconds);
				//p.StandardOutput.CurrentEncoding
                while (!p.StandardOutput.EndOfStream)
                {
                    Output += p.StandardOutput.ReadLine() + "\n";
                    MaxLineCount--;
                    if (MaxLineCount <= 0 || DateTime.Now > Timeout)
                        break; // prevent from looping endless
                }
                if (DateTime.Now > Timeout)
                {
                    p.Kill();
                    throw new Exception($"Error, possible endless loop! killing the subprocess after {_WaitTimeoutInSeconds} seconds");
                }
                if (MaxLineCount <= 0)
                {
                    p.Kill();
                    throw new Exception("Error, possible endless loop! killing the subprocess after reading 1000 lines");
                }

                bool ProcessHasExited = p.WaitForExit(_WaitTimeoutInSeconds * 1000); // at max 5 seconds!
                if (!ProcessHasExited)
				{
                    Console.WriteLine($"Error in Method CallProcessAndReturnConsoleOutput! Process hasn't exited after {_WaitTimeoutInSeconds} seconds!");
                    throw new Exception($"Error in Method CallProcessAndReturnConsoleOutput! Process hasn't exited after {_WaitTimeoutInSeconds} seconds!");
				}
            }

            return Output;
        }

        private static string GetPartOfStringBetween(string input, string beg, string end)
        {
            try
            {
                int startpos = input.IndexOf(beg);
                if (startpos == -1)
                    return "";
                int endpos = input.IndexOf(end, startpos + beg.Length + 1);
                if (endpos == -1)
                    return "";
                return input.Substring(startpos + beg.Length, endpos - startpos - beg.Length).Trim();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string GetPartOfStringAfter(string input, string beg)
        {
            try
            {
                int startpos = input.IndexOf(beg);
                if (startpos == -1)
                    return "";

				var start = startpos + beg.Length;
                return input.Substring(start, input.Length-start).Trim();
            }
            catch (Exception)
            {
                return "";
            }
        }

		#endregion
	}
}

