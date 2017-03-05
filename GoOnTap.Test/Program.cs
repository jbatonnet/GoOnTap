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

namespace GoOnTap
{
    public static class Program
    {
        private static Regex DirectoryNameRegex { get; } = new Regex(@"^(?:[^ ]| [^-])+ *[-,] *Lvl *(?<Level>[0-9]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex ScreenshotNameRegex { get; } = new Regex(@"^(?<Name>(?:[^ ]| [^-])+) *[-,] *Lvl *(?<Level>[X0-9.]+) *[-,] *Cp *(?<Cp>[0-9]+) *[-,] *Hp *(?<Hp>[0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Dictionary<string, string> Options { get; private set; }
        public static List<string> Parameters { get; private set; }

        public static DirectoryInfo ScreenshotsDirectory { get; private set; }
        public static int? PlayerLevel { get; private set; }
        public static bool OnlyCandy { get; private set; } = false;

        public static void Main(string[] args)
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
            try
            {
                if (args.Length > 0)
                    ScreenshotsDirectory = new DirectoryInfo(args[0]);
            }
            finally
            {
                if (ScreenshotsDirectory?.Exists != true)
                    ScreenshotsDirectory = new DirectoryInfo(@"..\..\..\Data");
            }

            Console.WriteLine("Testing directory: " + ScreenshotsDirectory.FullName);
            if (PlayerLevel != null)
                Console.WriteLine("Default player level: " + PlayerLevel);
            Console.WriteLine("Use only candy name: " + OnlyCandy);
            Console.WriteLine();

            // Execute tests
            RunTests().Wait();

#if DEBUG
            Console.WriteLine();
            Console.WriteLine("-- End --");
            Console.ReadLine();
#endif
        }

        private static async Task RunTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Log.Verbosity = LogVerbosity.Trace;

            FileInfo[] screenshotsInfo = ScreenshotsDirectory.GetFiles("*.png", SearchOption.AllDirectories).ToArray();
            Console.WriteLine("Found {0} screenshots to test", screenshotsInfo.Length);

            foreach (FileInfo screenshotInfo in screenshotsInfo)
            {
                Bitmap bitmap;

                try
                {
                    Image image = Image.FromFile(screenshotInfo.FullName);
                    bitmap = new Bitmap(image);
                }
                catch
                {
                    Console.Error.WriteLine($"{screenshotInfo.Name} > Invalid screenshot file");
                    continue;
                }

                int[] pixels = new int[bitmap.Width * bitmap.Height];

                BitmapData bitmapData = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length);
                bitmap.UnlockBits(bitmapData);

                ImageData data;
                //using (new ProfileScope("Process screenshot"))
                    data = await ImageProcessor.Process(pixels, bitmap.Width, bitmap.Height);

                // Detect player level
                int? playerLevel = PlayerLevel;

                Match directoryNameMatch = DirectoryNameRegex.Match(screenshotInfo.Directory.Name);
                if (directoryNameMatch.Success)
                    playerLevel = int.Parse(directoryNameMatch.Groups["Level"].Value);
                
                if (playerLevel == null)
                {
                    Console.Error.WriteLine($"{screenshotInfo.Name} > Could not detect player level. Please specify default player level parameter");
                    continue;
                }
                
                // Compute pokemon level
                double pokemonLevel = PokemonInfo.GetPokemonLevel(playerLevel.Value, data.LevelAngle);

                // Find matching pokemon
                if (OnlyCandy)
                    data.Name = "Toto";

                PokemonInfo candyPokemon = string.IsNullOrEmpty(data.Candy) ? null : Constants.Pokemons.MinValue(p =>
                {
                    int englishDiff = p.EnglishName != null ? Utilities.Diff(data.Candy, p.EnglishName) : int.MaxValue;
                    int frenchDiff = p.FrenchName != null ? Utilities.Diff(data.Candy, p.FrenchName) : int.MaxValue;
                    int germanDiff = p.GermanName != null ? Utilities.Diff(data.Candy, p.GermanName) : int.MaxValue;

                    return Math.Min(Math.Min(englishDiff, frenchDiff), germanDiff);
                });
                PokemonInfo pokemon = Constants.Pokemons.MinValue(p =>
                {
                    float nameRatio = 1;

                    Func<string, string> normalize = v => v.Replace("i", "l");

                    if (data.Name != null)
                    {
                        int englishDiff = p.EnglishName != null ? Utilities.Diff(normalize(data.Name), normalize(p.EnglishName)) : int.MaxValue;
                        int frenchDiff = p.FrenchName != null ? Utilities.Diff(normalize(data.Name), normalize(p.FrenchName)) : int.MaxValue;
                        int germanDiff = p.GermanName != null ? Utilities.Diff(normalize(data.Name), normalize(p.GermanName)) : int.MaxValue;

                        nameRatio = Math.Min(Math.Min(englishDiff, frenchDiff), germanDiff);
                    }

                    float evolutionRatio = candyPokemon == null ? -1 : (p.Id - candyPokemon.Id) / 6f;
                    if (evolutionRatio < 0 || evolutionRatio > 1)
                        evolutionRatio = 2;
                    else if (evolutionRatio > 0.5f)
                        evolutionRatio = 0.5f;

                    float cpRatio = data.CP >= p.GetMinimumCP(pokemonLevel) && data.CP <= p.GetMaximumCP(pokemonLevel) ? 0.25f : 1;
                    float hpRatio = data.HP >= p.GetMinimumHP(pokemonLevel) && data.HP <= p.GetMaximumHP(pokemonLevel) ? 0.25f : 1;

                    return nameRatio * (evolutionRatio + 0.1f) * cpRatio * hpRatio;
                });

                // Simulate IV possibilities
                Tuple<int, int, int>[] ivPossibilities = Enumerable.Range(0, 16).SelectMany(atk => Enumerable.Range(0, 16).SelectMany(def => Enumerable.Range(0, 16).Select(sta => Tuple.Create(atk, def, sta)))).ToArray();
                Tuple<int, int, int>[] matchingIVs = ivPossibilities.AsParallel().Where(ivPossibility =>
                {
                    int cp = (int)(((pokemon.BaseAttack + ivPossibility.Item1) * Math.Sqrt(pokemon.BaseDefense + ivPossibility.Item2) * Math.Sqrt(pokemon.BaseStamina + ivPossibility.Item3) * Math.Pow(Constants.CPMultipliers[pokemonLevel], 2)) / 10);
                    int hp = (int)((pokemon.BaseStamina + ivPossibility.Item3) * Constants.CPMultipliers[pokemonLevel]);

                    return data.CP == cp && data.HP == hp;
                }).ToArray();

                string displayName = pokemon.EnglishName.Replace("♀", "F").Replace("♂", "M");
                Console.WriteLine($"{screenshotInfo.Name} > {{ Name: {data.Name}, Lvl: {pokemonLevel}, CP: {data.CP}, HP: {data.HP}, Pokemon: {displayName} }}");

                // Validate against file name
                Match screenshotNameMatch = ScreenshotNameRegex.Match(screenshotInfo.Name);
                if (screenshotNameMatch.Success)
                {
                    // Decode file name
                    string validationName = screenshotNameMatch.Groups["Name"].Value;

                    string validationLevelString = screenshotNameMatch.Groups["Level"].Value;
                    double? validationLevel = null;
                    if (validationLevelString.ToLower() != "x")
                        validationLevel = double.Parse(validationLevelString);

                    int validationHp = int.Parse(screenshotNameMatch.Groups["Hp"].Value);
                    int validationCp = int.Parse(screenshotNameMatch.Groups["Cp"].Value);

                    // Validate data
                    bool validation = true;

                    validation &= validationName.Trim().ToLower() == displayName.Trim().ToLower();
                    validation &= validationLevel == null || (validationLevel == pokemonLevel);
                    validation &= validationHp == data.HP;
                    validation &= validationCp == data.CP;

                    if (!validation)
                        Console.Error.WriteLine($"{screenshotInfo.Name} > Detected info does not match file name");
                }
            }
        }
    }
}