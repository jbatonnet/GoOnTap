using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using PokemonGo.Test.Properties;

namespace GoOnTap
{
    public static class Program
    {
        static void Main(string[] args)
        {
            MainContent();
            Console.ReadLine();
        }

        static async void MainContent()
        { 
            Log.Verbosity = LogVerbosity.Trace;

            DirectoryInfo screenshotsDirectory = new DirectoryInfo(@"..\..\..\Data");

            string[] screenshots = Settings.Default.Screenshots.OfType<string>().ToArray();

            foreach (string screenshot in screenshots)
            {
                if (string.IsNullOrWhiteSpace(screenshot))
                    continue;

                string path = Path.Combine(screenshotsDirectory.FullName, screenshot);
                if (!File.Exists(path))
                    continue;

                int playerLevel = int.Parse(new string(Path.GetDirectoryName(screenshot).SkipWhile(c => !char.IsDigit(c)).ToArray()));

                Image image = Image.FromFile(path);
                Bitmap bitmap = new Bitmap(image);

                int[] pixels = new int[image.Width * image.Height];

                BitmapData bitmapData = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length);
                bitmap.UnlockBits(bitmapData);

                #if WINDOWS

                ImageProcessor.Reload();

                #endif

                ImageData data;
                //using (new ProfileScope("Process screenshot"))
                    data = await ImageProcessor.Process(pixels, image.Width, image.Height);

                // Compute pokemon level
                double maxLevel = playerLevel + 1.5;
                Dictionary<double, double> levels = new Dictionary<double, double>();

                for (double level = 1; level <= maxLevel; level += 0.5)
                {
                    double angle = (Constants.CPMultipliers[level] - 0.094) * 202.037116 / Constants.CPMultipliers[playerLevel];
                    levels.Add(level, Math.Abs(data.LevelAngle - angle));
                }

                double pokemonLevel = levels.OrderBy(p => p.Value).First().Key;

                // Find matching pokemon
                PokemonInfo pokemon;

                //using (new ProfileScope("Find matching pokémon"))
                {
                    pokemon = Constants.Pokemons.MinValue(p =>
                    {
                        int englishDiff = p.EnglishName != null ? Utilities.Diff(data.Name, p.EnglishName) : int.MaxValue;
                        int frenchDiff = p.FrenchName != null ? Utilities.Diff(data.Name, p.FrenchName) : int.MaxValue;

                        return Math.Min(englishDiff, frenchDiff);
                    });
                }

                // Simulate IV possibilities
                Tuple<int, int, int>[] matchingIVs;

                //using (new ProfileScope("Simulate IV possibilities"))
                {
                    Tuple<int, int, int>[] ivPossibilities = Enumerable.Range(0, 16).SelectMany(atk => Enumerable.Range(0, 16).SelectMany(def => Enumerable.Range(0, 16).Select(sta => Tuple.Create(atk, def, sta)))).ToArray();
                    matchingIVs = ivPossibilities.AsParallel().Where(ivPossibility =>
                    {
                        int cp = (int)(((pokemon.BaseAttack + ivPossibility.Item1) * Math.Sqrt(pokemon.BaseDefense + ivPossibility.Item2) * Math.Sqrt(pokemon.BaseStamina + ivPossibility.Item3) * Math.Pow(Constants.CPMultipliers[pokemonLevel], 2)) / 10);
                        int hp = (int)((pokemon.BaseStamina + ivPossibility.Item3) * Constants.CPMultipliers[pokemonLevel]);

                        return data.CP == cp && data.HP == hp;
                    }).ToArray();
                }

                Console.WriteLine("{0} > {{ Name: {1}, Lvl: {2}, CP: {3}, HP: {4}, Pokemon: {5} }}", Path.GetFileName(screenshot), data.Name, pokemonLevel, data.CP, data.HP, pokemon.EnglishName);
                pokemon.ToString();
            }
        }
    }
}