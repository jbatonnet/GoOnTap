using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GoOnTap.Test.Properties;
using Xunit;

namespace GoOnTap
{
    public static class Program
    {
        internal static Regex DirectoryNameRegex { get; } = new Regex(@"^(?:[^ ]| [^-])+ *[-,] *Lvl *(?<Level>[0-9]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        internal static Regex ScreenshotNameRegex { get; } = new Regex(@"^(?<Name>(?:[^ ]| [^-])+) *[-,] *Lvl *(?<Level>[X0-9.]+) *[-,] *Cp *(?<Cp>[0-9]+) *[-,] *Hp *(?<Hp>[0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Dictionary<string, string> Options { get; private set; }
        public static List<string> Parameters { get; private set; }

        public static DirectoryInfo ScreenshotsDirectory { get; private set; } = new DirectoryInfo(@"..\..\..\Test");
        public static int? PlayerLevel { get; private set; }
        public static bool OnlyCandy { get; private set; } = false;

        static Program()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Log.Verbosity = LogVerbosity.Trace;

            if (!string.IsNullOrEmpty(Settings.Default.ScreenshotsDirectory))
                ScreenshotsDirectory = new DirectoryInfo(Settings.Default.ScreenshotsDirectory);
        }

        public static int Main(string[] args)
        {
            Console.WriteLine("-- GoOnTap.Test --");

            Options = args.Where(a => a.StartsWith("/"))
                          .Select(a => a.TrimStart('/'))
                          .Select(a => new { Parameter = a.Trim(), Separator = a.Trim().IndexOf(':') })
                          .ToDictionary(a => a.Separator == -1 ? a.Parameter : a.Parameter.Substring(0, a.Separator).ToLower(), a => a.Separator == -1 ? null : a.Parameter.Substring(a.Separator + 1));

            Parameters = args.Where(a => !a.StartsWith("/"))
                             .ToList();

            // Decode parameters
            if (Options.ContainsKey("playerlevel"))
            {
                int playerLevel;
                if (!int.TryParse(Options["playerlevel"], out playerLevel))
                    throw new FormatException("Invalid specified player level");

                PlayerLevel = playerLevel;
            }
            if (Options.ContainsKey("onlycandy"))
                OnlyCandy = true;

            // Check specified directory
            
            if (args.Length > 0)
            {
                try
                {
                    ScreenshotsDirectory = new DirectoryInfo(args[0]);
                    if (!ScreenshotsDirectory.Exists)
                        throw new FileNotFoundException();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Specified directory could not be found. " + e);
                }
            }

            Console.WriteLine("Testing directory: " + ScreenshotsDirectory.FullName);
            if (PlayerLevel != null)
                Console.WriteLine("Default player level: " + PlayerLevel);
            Console.WriteLine("Use only candy name: " + OnlyCandy);
            Console.WriteLine();

            // Execute tests
            bool result = RunTests();

#if DEBUG
            Console.WriteLine();
            Console.WriteLine("-- End --");
            Console.ReadLine();
#else
            if (!result)
                return -1;
#endif

            return 0;
        }

        private static bool RunTests()
        {
            bool success = true;

            FileInfo[] screenshotsInfo = ScreenshotsDirectory.GetFiles("*.png", SearchOption.AllDirectories).ToArray();
            Console.WriteLine("Found {0} screenshots to test", screenshotsInfo.Length);

            foreach (FileInfo screenshotInfo in screenshotsInfo)
            {
                try
                {
                    ScreenshotTest.Test(screenshotInfo);
                }
                catch (Exception e)
                {
                    success = false;
                    Console.Error.WriteLine($"{screenshotInfo.Name} > {e.Message}");
                }
            }

            return success;
        }
    }
}