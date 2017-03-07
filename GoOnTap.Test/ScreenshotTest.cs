using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xunit;

namespace GoOnTap
{
    public class ScreenshotTest
    {
        [Theory]
        [ScreenshotData]
        public static void Test(FileInfo screenshotInfo)
        {
            Bitmap bitmap;

            try
            {
                Image image = Image.FromFile(screenshotInfo.FullName);
                bitmap = new Bitmap(image);
            }
            catch (Exception e)
            {
                throw new IOException("Invalid screenshot file", e);
            }

            int[] pixels = new int[bitmap.Width * bitmap.Height];

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length);
            bitmap.UnlockBits(bitmapData);

            Task<ImageData> processTask = ImageProcessor.Process(pixels, bitmap.Width, bitmap.Height);
            processTask.Wait();

            ImageData data = processTask.Result;

            // Detect player level
            int? playerLevel = Program.PlayerLevel;

            Match directoryNameMatch = Program.DirectoryNameRegex.Match(screenshotInfo.Directory.Name);
            if (directoryNameMatch.Success)
                playerLevel = int.Parse(directoryNameMatch.Groups["Level"].Value);

            if (playerLevel == null)
                throw new Exception("Could not detect player level. Please specify default player level parameter.");

            // Compute pokemon level
            double pokemonLevel = PokemonInfo.GetPokemonLevel(playerLevel.Value, data.LevelAngle);

            // Find matching pokemon
            if (Program.OnlyCandy)
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
            Match screenshotNameMatch = Program.ScreenshotNameRegex.Match(screenshotInfo.Name);
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
                Assert.True(validationName.Trim().ToLower() == displayName.Trim().ToLower(), $"Detected name is \"{displayName}\". Should be \"{validationName}\"");
                Assert.True(validationCp == data.CP, $"Detected CP is {data.CP}. Should be {validationCp}");
                Assert.True(validationHp == data.HP, $"Detected HP is {data.HP}. Should be {validationHp}");

                if (validationLevel != null)
                    Assert.True(validationLevel == pokemonLevel, $"Detected level is {pokemonLevel}. Should be {validationLevel} (arc angle is {data.LevelAngle})");
            }
        }
    }
}