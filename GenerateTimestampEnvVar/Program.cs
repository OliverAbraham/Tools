using System;

namespace GenerateTimestampEnvVar
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine($"set datestr={DateTime.Now.ToString("yyyy-MM-dd")}");
			Console.WriteLine($"set timestr={DateTime.Now.ToString("HH-mm-ss")}");
			Console.WriteLine($"set timestamp={DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}");
		}
	}
}
