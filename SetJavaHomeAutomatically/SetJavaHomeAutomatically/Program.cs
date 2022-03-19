using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SetJavaHomeAutomatically
{
    class Program
    {
        private const string VERSION = "2021-05-09";

        static void Main(string[] args)
        {
            Console.WriteLine("SetJavaHomeAutomatically - Oliver Abraham - mail@olive-abraham.de - Version {VERSION}");
            Console.WriteLine("Searches for the installed Java version and sets the JAVA_HOME environment variable.");

            var programDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            TryDirectory(programDirectory);

            programDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            TryDirectory(programDirectory);
        }

        private static void TryDirectory(string programDirectory)
        {
            try
            {
                var path = Path.Combine(programDirectory, "Java");
                var javaDirectories = Directory.GetDirectories(path, "*");
                if (javaDirectories != null && javaDirectories.Any())
                {
                    foreach (var dir in javaDirectories)
                    {
                        CheckDirectory(dir);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private static void CheckDirectory(string dir)
        {
            Console.WriteLine($"Checking folder '{dir}'");

            var javaExe = Path.Combine(dir, "bin", "java.exe");
            if (File.Exists(javaExe))
            {
                Console.WriteLine($"Found Java installation directory '{javaExe}' with java.exe");
                Console.WriteLine($"Setting environment variable JAVA_HOME to '{dir}'");
                Environment.SetEnvironmentVariable("JAVA_HOME", dir);
            }
        }
    }
}
