using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOnTap
{
    public class ImageData
    {
        public string Name { get; internal set; }
        public float LevelAngle { get; internal set; }
        public int CP { get; internal set; }
        public int HP { get; internal set; }

        public string Candy { get; internal set; }
        public int ArcX { get; internal set; }
        public int ArcY { get; internal set; }
        public int ArcSize { get; internal set; }
    }

    public class ImageProcessor
    {
        private static List<KeyValuePair<char, bool[,]>> characters;

#if WINDOWS
        private static DirectoryInfo tempDirectory = new DirectoryInfo(@"..\..\..\Temp");
#endif

        static ImageProcessor()
        {
#if ANDROID || RELEASE
            characters = Constants.CharactersCache;
#else
            Reload();
#endif
        }

#if WINDOWS && DEBUG
        internal static void Reload()
        {
            if (!tempDirectory.Exists)
                tempDirectory.Create();

            foreach (FileInfo tempFile in tempDirectory.GetFiles())
                tempFile.Delete();
            string characterDirectory = @"..\..\..\Data\Characters";

            // Preload characters
            characters = new List<KeyValuePair<char, bool[,]>>();

            foreach (string path in Directory.EnumerateFiles(characterDirectory, "*.png", SearchOption.AllDirectories))
            {
                string directory = Path.GetFileName(Path.GetDirectoryName(path));
                string file = Path.GetFileNameWithoutExtension(path).ToLower();

                char character = directory[0];

                // Special characters
                switch (directory)
                {
                    case "[Dot]": character = '.'; break;
                    case "[Slash]": character = '/'; break;
                    case "[Male]": character = '♂'; break;
                    case "[Female]": character = '♀'; break;
                }

                // Variants
                if (file.Contains("lower"))
                    character = char.ToLower(character);
                else if (file.Contains("upper"))
                    character = char.ToUpper(character);

                bool inverted = file.Contains("inv");

                // Load pixels
                Bitmap bitmap = new Bitmap(path);

                bool[,] pixels = new bool[bitmap.Width, bitmap.Height];
                for (int y = 0; y < bitmap.Height; y++)
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        int pixel = bitmap.GetPixel(x, y).ToArgb();

                        if (inverted)
                            pixels[x, y] = whiteSelector(pixel);
                        else
                            pixels[x, y] = graySelector(pixel);
                    }

                using (Bitmap boolBitmap = new Bitmap(bitmap.Width, bitmap.Height))
                {
                    for (int y = 0; y < bitmap.Height; y++)
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            int pixel = bitmap.GetPixel(x, y).ToArgb();

                            if (inverted)
                                boolBitmap.SetPixel(x, y, whiteSelector(pixel) ? Color.Black : Color.White);
                            else
                                boolBitmap.SetPixel(x, y, graySelector(pixel) ? Color.Black : Color.White);
                        }

                    string charString = char.IsUpper(character) ? "_" + character : character.ToString();
                    boolBitmap.Save(Path.Combine(tempDirectory.FullName, $"{charString}.jpg"));
                }

                bitmap.Dispose();
                characters.Add(new KeyValuePair<char, bool[,]>(character, pixels));
            }

            // Precompute characters
            using (StreamWriter output = new StreamWriter(@"..\..\..\Data\Characters.cs"))
            {
                output.WriteLine("using System;");
                output.WriteLine("using System.Collections.Generic;");
                output.WriteLine();
                output.WriteLine("public partial class Constants");
                output.WriteLine("{");
                output.WriteLine("    public static List<KeyValuePair<char, bool[,]>> CharactersCache { get; } = new List<KeyValuePair<char, bool[,]>>()");
                output.WriteLine("    {");

                foreach (var pair in characters)
                {
                    string arrayString = Enumerable.Range(0, pair.Value.GetLength(0)).Select(x => "{ " + Enumerable.Range(0, pair.Value.GetLength(1)).Select(y => pair.Value[x, y] ? "true" : "false").Join(", ") + " }").Join(", ");
                    output.WriteLine("        new KeyValuePair<char, bool[,]>('{0}', new bool[{1}, {2}] {{ {3} }}),", pair.Key, pair.Value.GetLength(0), pair.Value.GetLength(1), arrayString);
                }

                output.WriteLine("    };");
                output.WriteLine("}");
            }
        }
#endif

        public static async Task<ImageData> Process(int[] pixels, int width, int height)
        {
            float density = width / 160f;

            Func<float, float, int> getPixel = (x, y) => pixels[(int)y * width + (int)x];
            Func<string, Func<char, bool>, Func<int, bool>, int, int, int, int, string> readString = (name, charSelector, pixelSelector, left, top, right, bottom) =>
            {
#if WINDOWS && DEBUG
                //ImageProcessor.Reload();
#endif

                try
                {
                    // Split chars to read on the picture
                    List<bool[,]> charsToRead = new List<bool[,]>();

                    for (int lastX = left, currentX = left; currentX < right; currentX++)
                    {
                        if (currentX == right - 1 || Enumerable.Range(top, bottom - top).All(y => !pixelSelector(getPixel(currentX, y))))
                        {
                            if (currentX == right - 1)
                                currentX++;

                            int detectedTop = Enumerable.Range(top, bottom - top).FirstOrDefault(y => Enumerable.Range(lastX, currentX - lastX - 1).Any(x => pixelSelector(getPixel(x, y))), top);
                            int detectedBottom = Enumerable.Range(top, bottom - top).Reverse().FirstOrDefault(y => Enumerable.Range(lastX, currentX - lastX - 1).Any(x => pixelSelector(getPixel(x, y))), bottom) + 1;

                            bool[,] charToRead = new bool[currentX - lastX - 1, detectedBottom - detectedTop];

                            for (int y = 0; y < detectedBottom - detectedTop; y++)
                                for (int x = 0; x < currentX - lastX - 1; x++)
                                    charToRead[x, y] = pixelSelector(getPixel(lastX + 1 + x, detectedTop + y));

#if WINDOWS && DEBUG
                            using (Bitmap colorBitmap = new Bitmap(currentX - lastX - 1, detectedBottom - detectedTop))
                            using (Bitmap boolBitmap = new Bitmap(currentX - lastX - 1, detectedBottom - detectedTop))
                            {
                                for (int y = 0; y < detectedBottom - detectedTop; y++)
                                    for (int x = 0; x < currentX - lastX - 1; x++)
                                    {
                                        int pixel = getPixel(lastX + 1 + x, detectedTop + y);
                                        colorBitmap.SetPixel(x, y, Color.FromArgb(pixel));
                                        boolBitmap.SetPixel(x, y, pixelSelector(pixel) ? Color.Black : Color.White);
                                    }

                                colorBitmap.Save(Path.Combine(tempDirectory.FullName, $"{name}-Color-{top}-{currentX}.png"));
                                boolBitmap.Save(Path.Combine(tempDirectory.FullName, $"{name}-Bool-{top}-{currentX}.png"));
                            }
#endif

                            charsToRead.Add(charToRead);

                            while (Enumerable.Range(top, bottom - top).All(y => !pixelSelector(getPixel(currentX + 1, y))))
                                currentX++;

                            lastX = currentX;
                        }
                    }

                    // Decode chars
                    char[] result = charsToRead.Select((charToRead, i) =>
                    {
                        string _name = name;
                        float charRatio = (float)charToRead.GetLength(1) / charToRead.GetLength(0);

                        ConcurrentBag<KeyValuePair<char, float>> matches = new ConcurrentBag<KeyValuePair<char, float>>();
                        matches.Add(new KeyValuePair<char, float>('?', 0.75f));

                        foreach (var p in characters/*.Where(charSelector)*/)
                        {
                            char c = p.Key;
                            bool[,] reference = p.Value;

                            float referenceRatio = (float)reference.GetLength(1) / reference.GetLength(0);

                            if (Math.Abs(charRatio / referenceRatio - 1) > 0.2)
                                continue;

                            int diffs = 0;
                            int testWidth = charToRead.GetLength(0);
                            int testHeight = charToRead.GetLength(1);

                            for (int y = 0; y < testHeight; y++)
                                for (int x = 0; x < testWidth; x++)
                                {
                                    int testX = (int)(((float)x / testWidth) * reference.GetLength(0));
                                    int testY = (int)(((float)y / (testHeight)) * reference.GetLength(1));

                                    if (charToRead[x, y] ^ reference[testX, testY])
                                        diffs++;
                                }

                            int count = Enumerable.Range(0, testWidth).Sum(x => Enumerable.Range(0, testHeight).Sum(y => charToRead[x, y] ? 1 : 0));
                            float ratio = (float)diffs / count;

                            matches.Add(new KeyValuePair<char, float>(c, ratio));
                        }

                        return matches.OrderBy(p => p.Value)
                                      .First().Key;
                    }).ToArray();

                    return new string(result.Where(c => charSelector(c)).ToArray());
                }
                catch
                {
                    return "";
                }
            };

            #region 1. Detect top-left edge of details zone

            float edgeX = density * 4.75f;
            int edgeY = Enumerable.Range((int)density * 20, height).First(y =>
            {
                int e = getPixel(edgeX, y);

                if (!whiteSelector(e))
                    return false;

                int l = getPixel(edgeX - density * 1.5f, y);
                int t = getPixel(edgeX, y - density * 1.5f);

                if (whiteSelector(l) || whiteSelector(t))
                    return false;

                int r = getPixel(edgeX + density * 3, y);
                int b = getPixel(edgeX, y + density * 3);

                if (!whiteSelector(r) || !whiteSelector(b))
                    return false;

                return true;
            });

            #endregion
            #region 2. Detect bottom-left of CP arc

            float arcY = edgeY - density * 8.8f;
            
            // Find arc X position
            int arcX = Enumerable.Range(0, width).FirstOrDefault(x =>
            {
                int a = getPixel(x, arcY);

                if (!whiteSelector(a))
                    return false;

                int ll = getPixel(x - density * 1.5f, arcY);
                int rr = getPixel(x + density * 1.5f, arcY);

                if (whiteSelector(ll) || whiteSelector(rr))
                    return false;

                int l = getPixel(x - density * 0.2f, arcY);
                int r = getPixel(x + density * 0.2f, arcY);

                if (!whiteSelector(l) || !whiteSelector(r))
                    return false;

                return true;
            });

            // 2nd check a bit lower if needed
            if (arcX == 0)
            {
                float arcLowerY = edgeY - density * 7.5f;

                arcX = Enumerable.Range(0, width).FirstOrDefault(x =>
                {
                    int a = getPixel(x, arcLowerY);

                    if (!whiteSelector(a))
                        return false;

                    int ll = getPixel(x - density * 1.5f, arcLowerY);
                    int rr = getPixel(x + density * 1.5f, arcLowerY);

                    if (whiteSelector(ll) || whiteSelector(rr))
                        return false;

                    int l = getPixel(x - density * 0.2f, arcLowerY);
                    int r = getPixel(x + density * 0.2f, arcLowerY);

                    if (!whiteSelector(l) || !whiteSelector(r))
                        return false;

                    return true;
                });
            }

            // Adjust arc Y position
            arcY = density * 0.75f + Enumerable.Range((int)(arcY - density * 2), (int)(density * 4.5f)).Reverse().First(y =>
            {
                int a = getPixel(arcX, y);

                if (!whiteSelector(a))
                    return false;

                int bb = getPixel(arcX, y + density * 0.2f);

                if (!whiteSelector(bb))
                    return false;

                int b = getPixel(arcX, y + density * 1.5f);

                if (whiteSelector(b))
                    return false;

                return true;
            });

            float arcWidth = width - arcX * 2;
            float arcCenter = arcX + arcWidth / 2;

            #endregion
            #region 3. Detect pokemon level

            Task<int> pokemonLevel = Task.Run(() =>
            {
                IEnumerable<int> angles = Enumerable.Range(-10, 1821).Reverse().Where(d =>
                {
                    float r = (float)d * (float)Math.PI / 1800;

                    // Get a pixel matching the angle slightly outside the arc
                    float x1 = arcCenter - (float)Math.Cos(r) * (arcWidth * 1.01f) / 2;
                    float y1 = arcY - (float)Math.Sin(r) * (arcWidth * 1.01f) / 2;

                    int p = getPixel(x1, y1);

                    if (!whiteSelector(p))
                        return false;

                    // Get a pixel matching the angle slightly inside the arc
                    float x2 = arcCenter - (float)Math.Cos(r) * (arcWidth * 0.99f) / 2;
                    float y2 = arcY - (float)Math.Sin(r) * (arcWidth * 0.99f) / 2;

                    int n = getPixel(x2, y2);

                    if (!whiteSelector(n))
                        return false;

                    // Check a pixel slightly too far out to reject e.g. gym overlays
                    float x3 = arcCenter - (float)Math.Cos(r) * (arcWidth * 1.03f) / 2;
                    float y3 = arcY - (float)Math.Sin(r) * (arcWidth * 1.03f) / 2;

                    int p2 = getPixel(x3, y3);

                    if (whiteSelector(p2))
                        return false;

                    // And another one, too close to the arc center to be the level indicator
                    float x4 = arcCenter - (float)Math.Cos(r) * (arcWidth * 0.97f) / 2;
                    float y4 = arcY - (float)Math.Sin(r) * (arcWidth * 0.97f) / 2;

                    int n2 = getPixel(x4, y4);

                    if (whiteSelector(n2))
                        return false;

                    // Sanity check: make sure the center of the spot is white
                    float x5 = arcCenter - (float)Math.Cos(r) * arcWidth / 2;
                    float y5 = arcY - (float)Math.Sin(r) * arcWidth / 2;

                    int c = getPixel(x5, y5);

                    if (!whiteSelector(c))
                        return false;

                    // We found a white spot on the arc with the correct radius
                    return true;
                });

                int max = angles.Max();
                return Math.Min(Math.Max((int)angles.Where(d => (d >= (max - 20))).Average(), 0), 1800);
            });

            #endregion
            #region 4. Read pokemon CP

            Task<int> pokemonCp = Task.Run(() =>
            {
                float cpWidth = density * 60;
                float cpHeight = density * 40;
                float cpBase = arcY - arcWidth / 2 - density * 10;

                int cpValue = -1;

                try
                {
                    int cpTop = (int)cpBase - Enumerable.Range(0, (int)(cpHeight / 2)).First(y =>
                    {
                        if (Enumerable.Range((int)(arcCenter - cpWidth / 2), (int)cpWidth).Any(x => whiteSelector(getPixel(x, cpBase - y))))
                            return false;

                        return true;
                    });
                    int cpBottom = (int)cpBase + Enumerable.Range(0, (int)(cpHeight / 2)).FirstOrDefault(y =>
                    {
                        if (Enumerable.Range((int)(arcCenter - cpWidth / 2), (int)cpWidth).Any(x => whiteSelector(getPixel(x, cpBase + y))))
                            return false;

                        return true;
                    });
                    int cpLeft = (int)arcCenter - (int)(cpWidth / 2) + Enumerable.Range(0, (int)(cpWidth / 2)).First(x =>
                    {
                        if (Enumerable.Range(cpTop, cpBottom - cpTop).Any(y => whiteSelector(getPixel((int)arcCenter - (int)(cpWidth / 2) + x, y))))
                            return true;

                        return false;
                    });
                    int cpRight = (int)arcCenter + 1 + (int)(cpWidth / 2) - Enumerable.Range(0, (int)(cpWidth / 2)).First(x =>
                    {
                        if (Enumerable.Range(cpTop, cpBottom - cpTop).Any(y => whiteSelector(getPixel((int)arcCenter + (int)(cpWidth / 2) - x, y))))
                            return true;

                        return false;
                    });

                    string cpCharacters = "0" + readString("CP", c => char.IsDigit(c) || c == '?', whiteSelector, cpLeft, cpTop, cpRight, cpBottom).TrimStart('?');

                    int.TryParse(cpCharacters, out cpValue);
                }
                catch { }

                return cpValue;
            });

#if DEBUG
            pokemonCp.Wait();
#endif

            #endregion
            #region 5. Read pokemon name

            Task<string> pokemonName = Task.Run(() =>
            {
                float nameWidth = density * 80;
                float nameHeight = density * 50;
                float nameBase = edgeY + density * 27;

                string name = null;

                try
                {
                    // Skip blank area
                    nameBase = nameBase - density - Enumerable.Range(0, (int)nameHeight).First(y => Enumerable.Range((int)(arcCenter - nameWidth / 2), (int)nameWidth).Any(x => graySelector(getPixel(x, nameBase - y))));

                    int nameTop = (int)nameBase - Enumerable.Range(0, (int)(nameHeight / 2)).First(y =>
                    {
                        if (Enumerable.Range((int)(arcCenter - nameWidth / 2), (int)nameWidth).Any(x => graySelector(getPixel(x, nameBase - y))))
                            return false;

                        return true;
                    });
                    int nameBottom = (int)nameBase + Enumerable.Range(0, (int)(nameHeight / 2)).First(y =>
                    {
                        if (Enumerable.Range((int)(arcCenter - nameWidth / 2), (int)nameWidth).Any(x => graySelector(getPixel(x, nameBase + y))))
                            return false;

                        return true;
                    });
                    int nameLeft = (int)arcCenter - (int)(nameWidth / 2) + Enumerable.Range(0, (int)(nameWidth / 2)).First(x =>
                    {
                        if (Enumerable.Range(nameTop, nameBottom - nameTop).Any(y => graySelector(getPixel((int)arcCenter - (int)(nameWidth / 2) + x, y))))
                            return true;

                        return false;
                    });
                    int nameRight = (int)arcCenter + 1 + (int)(nameWidth / 2) - Enumerable.Range(0, (int)(nameWidth / 2)).First(x =>
                    {
                        if (Enumerable.Range(nameTop, nameBottom - nameTop).Any(y => graySelector(getPixel((int)arcCenter + (int)(nameWidth / 2) - x, y))))
                            return true;

                        return false;
                    });

                    name = readString("Name", c => !char.IsDigit(c), graySelector, nameLeft, nameTop, nameRight, nameBottom);
                }
                catch
                {
                    return null;
                }

                return name;
            });

#if DEBUG
            pokemonName.Wait();
#endif

            #endregion
            #region 6. Read pokemon HP

            Task<int> pokemonHp = Task.Run(() =>
            {
                float hpWidth = density * 50;
                float hpHeight = density * 8;

                int hpValue = -1;

                try
                {
                    float hpBase = edgeY + density * 45f;
                    hpBase = hpBase - density - Enumerable.Range(0, (int)hpHeight).First(y => Enumerable.Range((int)(arcCenter - hpWidth / 2), (int)hpWidth).Any(x => graySelector(getPixel(x, hpBase - y))));

                    int hpTop = (int)hpBase - Enumerable.Range(0, (int)(hpHeight / 2)).First(y =>
                    {
                        if (Enumerable.Range((int)(arcCenter - hpWidth / 2), (int)hpWidth).Any(x => graySelector(getPixel(x, hpBase - y))))
                            return false;

                        return true;
                    });
                    int hpBottom = (int)hpBase + Enumerable.Range(0, (int)(hpHeight / 2)).First(y =>
                    {
                        if (Enumerable.Range((int)(arcCenter - hpWidth / 2), (int)hpWidth).Any(x => graySelector(getPixel(x, hpBase + y))))
                            return false;

                        return true;
                    });
                    int hpLeft = (int)arcCenter - (int)(hpWidth / 2) + Enumerable.Range(0, (int)(hpWidth / 2)).First(x =>
                    {
                        if (Enumerable.Range(hpTop, hpBottom - hpTop).Any(y => graySelector(getPixel((int)arcCenter - (int)(hpWidth / 2) + x, y))))
                            return true;

                        return false;
                    });
                    int hpRight = (int)arcCenter + 1 + (int)(hpWidth / 2) - Enumerable.Range(0, (int)(hpWidth / 2)).First(x =>
                    {
                        if (Enumerable.Range(hpTop, hpBottom - hpTop).Any(y => graySelector(getPixel((int)arcCenter + (int)(hpWidth / 2) - x, y))))
                            return true;

                        return false;
                    });

                    string hpCharacters = readString("HP", c => true, graySelector, hpLeft, hpTop, hpRight, hpBottom);

                    // Only last number after separator
                    hpCharacters = new string(hpCharacters.Where(c => char.IsDigit(c) || c == '/' || c == '?').ToArray());
                    hpCharacters = "0" + new string(hpCharacters.Reverse().TakeWhile(c => char.IsDigit(c)).Reverse().ToArray());

                    int.TryParse(hpCharacters, out hpValue);

                    // FIXME: Ugly fix
                    if (hpValue >= 10000 && hpValue <= 99999 && (hpValue / 100) % 10 == 1)
                        hpValue %= 100;
                    if (hpValue >= 1000000 && hpValue <= 9999999 && (hpValue / 1000) % 10 == 1)
                        hpValue %= 1000;
                }
                catch { }

                return hpValue;
            });

#if DEBUG
            pokemonHp.Wait();
#endif

            #endregion
            #region 7. Read candy name

            Task<string> candyName = Task.Run(() =>
            {
                float candyWidth = density * 65;
                float candyHeight = density * 8;

                float candyBase = edgeY + density * 101;
                float candyCenter = width * 2 / 3;

                string name = null;

                try
                {
                    // Skip green area
                    candyBase = candyBase - Enumerable.Range(0, (int)candyHeight).First(y => !Enumerable.Range((int)(candyCenter - candyWidth / 2), (int)candyWidth).Any(x => greenSelector(getPixel(x + density * 6, candyBase - y))));

                    // Skip blank area
                    candyBase = candyBase - density - Enumerable.Range(0, (int)candyHeight).First(y => Enumerable.Range((int)(candyCenter - candyWidth / 2), (int)candyWidth).Any(x => lightGraySelector(getPixel(x, candyBase - y))));

                    int candyTop = (int)candyBase - Enumerable.Range(0, (int)(candyHeight / 2)).First(y =>
                    {
                        if (Enumerable.Range((int)(candyCenter - candyWidth / 2), (int)candyWidth).Any(x => lightGraySelector(getPixel(x, candyBase - y))))
                            return false;

                        return true;
                    });
                    int candyBottom = (int)candyBase + Enumerable.Range(0, (int)(candyHeight / 2)).FirstOrDefault(y =>
                    {
                        if (candyBase + y >= height)
                            return false;
                        if (Enumerable.Range((int)(candyCenter - candyWidth / 2), (int)candyWidth).Any(x => lightGraySelector(getPixel(x, candyBase + y))))
                            return false;

                        return true;
                    }, 0);

                    if (candyBottom == candyTop)
                        return null;

                    int candyLeft = (int)candyCenter - (int)(candyWidth / 2) + Enumerable.Range(0, (int)(candyWidth / 2)).FirstOrDefault(x =>
                    {
                        if (Enumerable.Range(candyTop, candyBottom - candyTop).Any(y => lightGraySelector(getPixel((int)candyCenter - (int)(candyWidth / 2) + x, y))))
                            return true;

                        return false;
                    }, 0);
                    int candyRight = (int)candyCenter + 1 + (int)(candyWidth / 2) - Enumerable.Range(0, (int)(candyWidth / 2)).FirstOrDefault(x =>
                    {
                        if (Enumerable.Range(candyTop, candyBottom - candyTop).Any(y => lightGraySelector(getPixel((int)candyCenter + (int)(candyWidth / 2) - x, y))))
                            return true;

                        return false;
                    }, 0);

                    if (candyLeft == 0 || candyRight == 0)
                        return null;

                    string candyCharacters = readString("Candy", c => true, lightGraySelector, candyLeft, candyTop, candyRight, candyBottom);

                    name = candyCharacters.ToLower().Replace("?", "")
                        .Replace("bonbons", "")
                        .Replace("bonbon", "")
                        .Replace("candy", "")
                        .Replace("candies", "")
                        .Replace("canoy", "")
                        .replace("canoies", "");
                }
                catch { }

                return name;
            });

#if DEBUG
            candyName.Wait();
#endif

            #endregion

#if WINDOWS
            pokemonName.Wait();
            pokemonLevel.Wait();
            pokemonCp.Wait();
            pokemonHp.Wait();
#endif

            return new ImageData()
            {
                Name = await pokemonName,
                LevelAngle = await pokemonLevel,
                CP = await pokemonCp,
                HP = await pokemonHp,

                Candy = await candyName,
                ArcX = (int)(arcX + arcWidth / 2),
                ArcY = (int)arcY,
                ArcSize = (int)(arcWidth / 2)
            };
        }

        private static Func<int, int> getRed = p => (p >> 16) & 0xFF;
        private static Func<int, int> getGreen = p => (p >> 8) & 0xFF;
        private static Func<int, int> getBlue = p => p & 0xFF;

        //private static Func<int, float> getBrightness = p => ((p >> 16) & 0xFF) / 255f * 0.2126f + ((p >> 8) & 0xFF) / 255f * 0.7152f + (p & 0xFF) / 255f * 0.0722f;
        private static Func<int, float> getBrightness = p => ((p >> 16) & 0xFF) / 255f * 0.3333f + ((p >> 8) & 0xFF) / 255f * 0.3333f + (p & 0xFF) / 255f * 0.3333f;
        private static Func<int, float> getSaturation = p =>
        {
            float r = ((p >> 16) & 0xFF) / 255f;
            float g = ((p >> 8) & 0xFF) / 255f;
            float b = (p & 0xFF) / 255f;

            float cMax = r > g && r > b ? r :
                         g > r && g > b ? g :
                         b;
            float cMin = r < g && r < b ? r :
                         g < r && g < b ? g :
                         b;

            float d = cMax - cMin;

            return d == 0 ? 0 : d / (1 - Math.Abs(2 * (cMax + cMin) / 2 - 1));
        };

        private const float whiteBrightness = 0.96f;
        private const float grayBrightness = 0.55f;
        private const float lightGrayBrightness = 0.80f;

        private static Func<int, bool> whiteSelector = p => getBrightness(p) > whiteBrightness;
        private static Func<int, bool> graySelector = p => getBrightness(p) < grayBrightness;
        private static Func<int, bool> greenSelector = p => Math.Abs(getRed(p) - 232) + Math.Abs(getGreen(p) - 240) + Math.Abs(getBlue(p) - 225) < 10;
        private static Func<int, bool> lightGraySelector = p => getBrightness(p) < lightGrayBrightness;
    }
}