using System;
using System.IO;
using System.Linq;
using CommandLine;

namespace DeleteOldFiles
{
	class Program
	{
		#region ------------- Command line options ------------------------------------------------
		class Options
		{
			[Option('f', "filename", Required = true, HelpText = "Path and filename mask to search for, i.e. D:\\Old\\a*.mp3")]
			public string PathAndFilenameMask { get; set; }

			[Option('o', "olderthandays", Required = true, HelpText = "Delete files older than this days")]
			public int OlderThanDays { get; set; }
		}
		#endregion



		#region ------------- Fields --------------------------------------------------------------
		private static Options _options;
		#endregion



		#region ------------- Init ----------------------------------------------------------------
		static void Main(string[] args)
		{
			ParseCommandLineArguments();
			if (_options == null)
			{
				Console.WriteLine("Missing arguments!");
				return;
			}
			Console.WriteLine($"Delete old files:");

			var dir  = Path.GetDirectoryName(_options.PathAndFilenameMask);
			var name = Path.GetFileName     (_options.PathAndFilenameMask);
			Console.WriteLine($"Delete in this folder          : '{dir}'");
			Console.WriteLine($"Delete the following files     : '{name}'");
			Console.WriteLine($"Delete file that are older than: {_options.OlderThanDays} days");


			var files = Directory.GetFiles(dir, name);


			Console.WriteLine($"\nFound the following files:");
			foreach (var file in files) 
				Console.WriteLine(file);


			var filteredFiles = (from f in files where FileMatchesTheFilter(f) select f).ToList();


			Console.WriteLine($"\nThe following files match the filter:");
			foreach (var file in filteredFiles) 
				Console.WriteLine($"{file,-100} ({GetAge(file)} days old)");
			

			Console.WriteLine($"\nDeleting the files...");
			foreach (var file in filteredFiles) 
				File.Delete(file);
			Console.WriteLine($"Files deleted.");
		}
		#endregion



		#region ------------- Private methods -----------------------------------------------------
		private static bool FileMatchesTheFilter(string filename)
		{
			int ageInDays = GetAge(filename);
			if (ageInDays > _options.OlderThanDays)
				return true;
			else
				return false;
		}

		private static int GetAge(string filename)
		{
			var lastWriteTime = File.GetLastWriteTime(filename);
			var age = DateTime.Now - lastWriteTime;
			return (int)age.TotalDays;
		}

		private static void ParseCommandLineArguments()
		{
			string[] args = Environment.GetCommandLineArgs();
			CommandLine.Parser.Default.ParseArguments<Options>(args)
				.WithParsed<Options>(o =>
				{
					_options = o;
				})
				.WithNotParsed<Options>(errs =>
				{
				});
		}
		#endregion
	}
}
