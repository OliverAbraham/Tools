using System.Diagnostics;

namespace DrivePowerStateMonitor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DrivePowerStateMonitor");
            var state = "";
            var startTime = DateTime.Now;
            var standbyMinutes = 0;

            do
            {
                state = "UNKNOWN";
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = @"smartctl";
                startInfo.Arguments = string.Join(" ", args);//" -i -n standby /dev/sdb";
                startInfo.RedirectStandardOutput = true;
                var result = Process.Start(startInfo);
                if (result != null)
                {
                    result.WaitForExit(2000);
                    if (result.StandardOutput is not null)
                    {
                        var output = result.StandardOutput.ReadToEnd();
                        if (output != null)
                        {
                            state = (output.Contains("is in STANDBY")) ? "STANDBY" : "ACTIVE";
                        }
                    }
                }                    

                var currentTime = DateTime.Now;
                int totalMinutes = (int)((currentTime - startTime).TotalMinutes);
                if (state == "STANDBY")
                    standbyMinutes++;
                int ratio = (totalMinutes==0) ? 0 : (standbyMinutes * 100) / totalMinutes;

                var statusText = $"{currentTime} {state,10} totalminutes: {totalMinutes,6} standbyminutes: {standbyMinutes,6} ratio: {ratio,3} %";
                Console.WriteLine (statusText);
                File.AppendAllText("drivestatus.log", $"{statusText}\n");

                for (int i = 0; i<60 && !Console.KeyAvailable; i++)
                    Thread.Sleep(1000);
            }
            while (!Console.KeyAvailable);
        }
    }
}
