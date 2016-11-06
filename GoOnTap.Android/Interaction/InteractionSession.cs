using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.App.Assist;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Service.Voice;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Util;

using Uri = Android.Net.Uri;

namespace GoOnTap.Android
{
    public class GoOnTapInteractionSession : VoiceInteractionSession
    {
        private View emptyZone, assistantLayout, imageLayout;
        private TextView name;
        private TextView levelValue, cpValue, hpValue, playerLevelValue, averageValue;
        private SeekBar levelSeek, cpSeek, hpSeek, playerLevelSeek;
        private View arcFeedback;
        private ImageView image;
        private TableLayout ivTable;

        private ImageData data;
        private double pokemonLevel;
        private PokemonInfo pokemon;

        private Uri iconSetUri;
        private Stream iconSetStream;
        private BitmapRegionDecoder iconSetBitmapRegionDecoder;
        private Bitmap lastBitmap;

        public GoOnTapInteractionSession(Context context) : base(context) { }

        public override View OnCreateContentView()
        {
            // Get views
            View view = LayoutInflater.Inflate(Resource.Layout.Assistant, null);

            name = view.FindViewById<TextView>(Resource.Id.Name);
            imageLayout = view.FindViewById(Resource.Id.ImageLayout);
            image = view.FindViewById<ImageView>(Resource.Id.Image);
            ivTable = view.FindViewById<TableLayout>(Resource.Id.IVTable);
            arcFeedback = view.FindViewById(Resource.Id.ArcFeedback);
            averageValue = view.FindViewById<TextView>(Resource.Id.AverageValue);

            // Pokemon popup menu
            PopupMenu menu = new PopupMenu(image.Context, image);

            menu.MenuItemClick += (a, b) =>
            {
                int id = b.Item.ItemId;
                pokemon = Constants.Pokemons.First(p => p.Id == id);

                RefreshStats();
            };

            Task menuTask = Task.Run(() =>
            {
                foreach (PokemonInfo pokemon in Constants.Pokemons)
                    menu.Menu.Add(1, pokemon.Id, pokemon.Id, pokemon.Id + " - " + pokemon.GetLocalizedName(Locale.Default.ToString()));
            });

            #region Pokemon name

            image.Click += (s, e) =>
            {
                menuTask.Wait();
                menu.Show();
            };

            #endregion
            #region Pokemon level

            levelValue = view.FindViewById<TextView>(Resource.Id.LevelValue);
            levelSeek = view.FindViewById<SeekBar>(Resource.Id.LevelSeek);

            levelSeek.ProgressChanged += LevelSeek_ProgressChanged;

            view.FindViewById(Resource.Id.LevelSeekLess).Click += (s, e) =>
            {
                if (levelSeek.Progress > 0)
                    levelSeek.Progress--;
            };
            view.FindViewById(Resource.Id.LevelSeekMore).Click += (s, e) =>
            {
                if (levelSeek.Progress < levelSeek.Max)
                    levelSeek.Progress++;
            };

            #endregion
            #region Pokemon CP

            cpValue = view.FindViewById<TextView>(Resource.Id.CPValue);
            cpSeek = view.FindViewById<SeekBar>(Resource.Id.CPSeek);

            cpSeek.ProgressChanged += CPSeek_ProgressChanged;

            view.FindViewById(Resource.Id.CPSeekLess).Click += (s, e) =>
            {
                if (cpSeek.Progress > 0)
                    cpSeek.Progress--;
            };
            view.FindViewById(Resource.Id.CPSeekMore).Click += (s, e) =>
            {
                if (cpSeek.Progress < cpSeek.Max)
                    cpSeek.Progress++;
            };

            #endregion
            #region Pokemon HP

            hpValue = view.FindViewById<TextView>(Resource.Id.HPValue);
            hpSeek = view.FindViewById<SeekBar>(Resource.Id.HPSeek);

            hpSeek.ProgressChanged += HPSeek_ProgressChanged;

            view.FindViewById(Resource.Id.HPSeekLess).Click += (s, e) =>
            {
                if (hpSeek.Progress > 0)
                    hpSeek.Progress--;
            };
            view.FindViewById(Resource.Id.HPSeekMore).Click += (s, e) =>
            {
                if (hpSeek.Progress < hpSeek.Max)
                    hpSeek.Progress++;
            };

            #endregion
            #region Player level

            playerLevelValue = view.FindViewById<TextView>(Resource.Id.PlayerLevelValue);
            playerLevelSeek = view.FindViewById<SeekBar>(Resource.Id.PlayerLevelSeek);

            playerLevelSeek.ProgressChanged += PlayerLevelSeek_ProgressChanged;

            view.FindViewById(Resource.Id.PlayerLevelSeekLess).Click += (s, e) =>
            {
                if (playerLevelSeek.Progress > 0)
                    playerLevelSeek.Progress--;
            };
            view.FindViewById(Resource.Id.PlayerLevelSeekMore).Click += (s, e) =>
            {
                if (playerLevelSeek.Progress < playerLevelSeek.Max)
                    playerLevelSeek.Progress++;
            };

            #endregion

            // Handle transparent zone to hide
            emptyZone = view.FindViewById(Resource.Id.EmptyZone);
            emptyZone.Click += (s, e) => Hide();

            // Adjust bottom margin according to navigation bar
            assistantLayout = view.FindViewById(Resource.Id.AssistantLayout);
            RelativeLayout.LayoutParams assistantLayoutParams = assistantLayout.LayoutParameters.JavaCast<RelativeLayout.LayoutParams>();

            int navigationBarHeight = 0;

            int navigationBarId = Context.Resources.GetIdentifier("navigation_bar_height", "dimen", "android");
            if (navigationBarId > 0)
                navigationBarHeight = Context.Resources.GetDimensionPixelSize(navigationBarId);

            assistantLayoutParams.BottomMargin = navigationBarHeight;

            return view;
        }
        public override void OnShow(Bundle args, ShowFlags showFlags)
        {
            base.OnShow(args, showFlags);

            int playerLevel = GoOnTapApplication.Config.PlayerLevel;

            name.Text = "?";
            image.SetImageBitmap(null);

            levelValue.Text = "?";
            cpValue.Text = "?";
            hpValue.Text = "?";
            averageValue.Text = "?";
            playerLevelValue.Text = playerLevel.ToString();

            arcFeedback.Visibility = ViewStates.Gone;

            levelSeek.Enabled = false;
            cpSeek.Enabled = false;
            hpSeek.Enabled = false;
            playerLevelSeek.Enabled = false;

            levelSeek.Progress = 0;
            cpSeek.Progress = 0;
            hpSeek.Progress = 0;
            playerLevelSeek.Progress = playerLevel - 1;

            while (ivTable.ChildCount > 1)
                ivTable.RemoveViewAt(1);
        }
        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnHandleAssist(Bundle data, AssistStructure structure, AssistContent content)
        {
            //Toast.MakeText(Context, "OnHandleAssist", ToastLength.Short).Show();
        }
        public override void OnHandleScreenshot(Bitmap screenshot)
        {
            // Read raw pixels
            int[] pixels = new int[screenshot.Width * screenshot.Height];
            screenshot.GetPixels(pixels, 0, screenshot.Width, 0, 0, screenshot.Width, screenshot.Height);

            Task.Run(async () =>
            {
                // Process image
                try
                {
                    data = await ImageProcessor.Process(pixels, screenshot.Width, screenshot.Height);

                    Log.Trace("Got pokemon data: {{ Name: {0}, CP: {1}, HP: {2}, Level: {3} }}", data.Name, data.CP, data.HP, data.LevelAngle);
                }
                catch
                {
                    assistantLayout.Post(() => Toast.MakeText(Context, "Could not detect pokemon specs", ToastLength.Short).Show());

                    Hide();
                    return;
                }

                // Compute pokémon level
                int playerLevel = GoOnTapApplication.Config.PlayerLevel;
                pokemonLevel = GetPokemonLevel(data.LevelAngle);
                Log.Trace("Found pokemon level: {0}", pokemonLevel);

                // Find matching pokémon
                PokemonInfo candyPokemon = string.IsNullOrEmpty(data.Candy) ? null : Constants.Pokemons.MinValue(p =>
                {
                    int englishDiff = p.EnglishName != null ? Utilities.Diff(data.Candy, p.EnglishName) : int.MaxValue;
                    int frenchDiff = p.FrenchName != null ? Utilities.Diff(data.Candy, p.FrenchName) : int.MaxValue;
                    int germanDiff = p.GermanName != null ? Utilities.Diff(data.Candy, p.GermanName) : int.MaxValue;

                    return Math.Min(Math.Min(englishDiff, frenchDiff), germanDiff);
                });

                pokemon = Constants.Pokemons.MinValue(p =>
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

                Log.Trace("Found pokemon info: {0}", pokemon.EnglishName);

                RefreshStats(true);
            }).ContinueWith(t =>
            {
                Log.Error(t.Exception.ToString());
#if DEBUG
                assistantLayout.Post(() => Toast.MakeText(Context, t.Exception.ToString(), ToastLength.Long).Show());
#else
                assistantLayout.Post(() => Toast.MakeText(Context, "Could not detect pokemon specs", ToastLength.Short).Show());
#endif
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void LevelSeek_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (!levelSeek.Enabled)
                return;

            pokemonLevel = e.Progress / 2.0 + 1;
            levelValue.Text = pokemonLevel.ToString();

            RefreshStats();
        }
        private void CPSeek_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (!cpSeek.Enabled)
                return;

            int cpMin = pokemon.GetMinimumCP(pokemonLevel);
            data.CP = cpMin + cpSeek.Progress;
            cpValue.Text = data.CP.ToString();

            RefreshIVs();
        }
        private void HPSeek_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (!hpSeek.Enabled)
                return;

            int hpMin = pokemon.GetMinimumHP(pokemonLevel);
            data.HP = hpMin + hpSeek.Progress;
            hpValue.Text = data.HP.ToString();

            RefreshIVs();
        }
        private void PlayerLevelSeek_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (!playerLevelSeek.Enabled)
                return;

            int playerLevel = playerLevelSeek.Progress + 1;
            GoOnTapApplication.Config.PlayerLevel = playerLevel;
            playerLevelValue.Text = playerLevel.ToString();

            pokemonLevel = GetPokemonLevel(data.LevelAngle);
            levelValue.Text = pokemonLevel.ToString();

            levelSeek.Enabled = false;
            levelSeek.Max = playerLevel * 2 + 1;
            levelSeek.Progress = (int)(pokemonLevel * 2) - 2;
            levelSeek.Enabled = true;

            RefreshStats();
        }

        private CancellationTokenSource ivTaskCancelation;

        private void RefreshStats(bool now = false)
        {
            if (data == null || pokemon == null)
                return;

            Task.Run(() =>
            {
                int playerLevel = GoOnTapApplication.Config.PlayerLevel;

                int cpMin = pokemon.GetMinimumCP(pokemonLevel);
                int cpMax = pokemon.GetMaximumCP(pokemonLevel);

                int hpMin = pokemon.GetMinimumHP(pokemonLevel);
                int hpMax = pokemon.GetMaximumHP(pokemonLevel);

                // Update UI
                assistantLayout.Post(() =>
                {
                    // Text views
                    levelValue.Text = pokemonLevel.ToString();
                    cpValue.Text = data.CP.ToString();
                    hpValue.Text = data.HP.ToString();

                    // Seek bars
                    levelSeek.Enabled = false;
                    cpSeek.Enabled = false;
                    hpSeek.Enabled = false;
                    playerLevelSeek.Enabled = false;

                    levelSeek.Max = playerLevel * 2 + 1;
                    levelSeek.Progress = (int)(pokemonLevel * 2) - 2;
                    cpSeek.Max = cpMax - cpMin;
                    cpSeek.Progress = data.CP - cpMin;
                    hpSeek.Max = hpMax - hpMin;
                    hpSeek.Progress = data.HP - hpMin;
                    playerLevelSeek.Progress = playerLevel - 1;

                    levelSeek.Enabled = true;
                    cpSeek.Enabled = true;
                    hpSeek.Enabled = true;
                    playerLevelSeek.Enabled = true;

                    // Arc feedback
                    float density = Context.Resources.DisplayMetrics.Density;
                    float levelAngle = (float)PokemonInfo.GetLevelAngle(pokemonLevel, playerLevel) / 180 * 178 + 1;
                    float feedbackX = data.ArcX - (float)Math.Cos(levelAngle * Math.PI / 180) * data.ArcSize;
                    float feedbackY = data.ArcY - (float)Math.Sin(levelAngle * Math.PI / 180) * data.ArcSize;

                    RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams((int)(20 * density), (int)(20 * density));
                    layoutParams.LeftMargin = (int)(feedbackX - 10 * density);
                    layoutParams.TopMargin = (int)(feedbackY - 10 * density);

                    arcFeedback.LayoutParameters = layoutParams;
                    arcFeedback.Visibility = ViewStates.Visible;

                    string pokemonName = pokemon.GetLocalizedName(Locale.Default.ToString());

                    // Pokemon image
                    if (name.Text != pokemonName)
                    {
                        name.Text = pokemonName;

                        lastBitmap?.Recycle();
                        lastBitmap = GetPokemonIcon(pokemon.Id);
                        image.SetImageBitmap(lastBitmap);
                    }
                });

                RefreshIVs(now);
            });
        }
        private void RefreshIVs(bool now = false)
        {
            if (data == null || pokemon == null)
                return;

            ivTaskCancelation?.Cancel();
            ivTaskCancelation = new CancellationTokenSource();

            Task.Run(async () =>
            {
                if (!now)
                    await Task.Delay(500);

                if (ivTaskCancelation.IsCancellationRequested)
                    return;

                // Simulate IV possibilities
                Tuple<int, int, int>[] ivPossibilities = Enumerable.Range(0, 16).SelectMany(atk => Enumerable.Range(0, 16).SelectMany(def => Enumerable.Range(0, 16).Select(sta => Tuple.Create(atk, def, sta)))).ToArray();
                Tuple<int, int, int>[] matchingIVs = ivPossibilities.AsParallel().Where(ivPossibility =>
                {
                    int cp = pokemon.GetCP(pokemonLevel, ivPossibility.Item1, ivPossibility.Item2, ivPossibility.Item3);
                    int hp = pokemon.GetHP(pokemonLevel, ivPossibility.Item3);

                    return data.CP == cp && data.HP == hp;
                }).ToArray();

                // Sort and limit number
                matchingIVs = matchingIVs.OrderByDescending(iv => iv.Item1 + iv.Item2 + iv.Item3).ToArray();

                // Update UI
                assistantLayout.Post(() =>
                {
                    if (matchingIVs.Length == 0)
                        averageValue.Text = "?";
                    else
                        averageValue.Text = ((int)matchingIVs.Average(t => (t.Item1 + t.Item2 + t.Item3) * 100f / 45)) + " %";

                    while (ivTable.ChildCount > 1)
                        ivTable.RemoveViewAt(1);

                    Action<string, string, string, string> addRow = (attack, defense, stamina, perfection) =>
                    {
                        TableRow tableRow = new TableRow(Context);
                        tableRow.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent);

                        TextView spacerView1 = new TextView(Context);
                        spacerView1.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.MatchParent, 1);
                        tableRow.AddView(spacerView1);

                        TextView attackView = new TextView(Context);
                        attackView.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.MatchParent, 1);
                        attackView.Text = attack;
                        attackView.Gravity = GravityFlags.CenterHorizontal;
                        attackView.SetTextColor(Color.Black);
                        tableRow.AddView(attackView);

                        TextView defenseView = new TextView(Context);
                        defenseView.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.MatchParent, 1);
                        defenseView.Text = defense;
                        defenseView.Gravity = GravityFlags.CenterHorizontal;
                        defenseView.SetTextColor(Color.Black);
                        tableRow.AddView(defenseView);

                        TextView staminaView = new TextView(Context);
                        staminaView.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.MatchParent, 1);
                        staminaView.Text = stamina;
                        staminaView.Gravity = GravityFlags.CenterHorizontal;
                        staminaView.SetTextColor(Color.Black);
                        tableRow.AddView(staminaView);

                        TextView perfectionView = new TextView(Context);
                        perfectionView.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.MatchParent, 1);
                        perfectionView.Text = perfection;
                        perfectionView.Gravity = GravityFlags.CenterHorizontal;
                        perfectionView.SetTextColor(Color.Black);
                        tableRow.AddView(perfectionView);

                        TextView spacerView2 = new TextView(Context);
                        spacerView2.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.MatchParent, 1);
                        tableRow.AddView(spacerView2);

                        ivTable.AddView(tableRow, new TableLayout.LayoutParams(TableLayout.LayoutParams.MatchParent, TableLayout.LayoutParams.WrapContent));
                    };

                    for (int i = 0; i < matchingIVs.Length; i++)
                    {
                        if (matchingIVs.Length > 8 && i >= 4 && i < matchingIVs.Length - 4)
                        {
                            addRow("...", "...", "...", "...");
                            i = matchingIVs.Length - 4;
                        }

                        Tuple<int, int, int> ivTuple = matchingIVs[i];

                        addRow(ivTuple.Item1.ToString(), ivTuple.Item2.ToString(), ivTuple.Item3.ToString(), ((ivTuple.Item1 + ivTuple.Item2 + ivTuple.Item3) * 100 / 45) + " %");
                    }
                });
            });
        }

        private double GetPokemonLevel(float levelAngle)
        {
            int playerLevel = GoOnTapApplication.Config.PlayerLevel;
            double maxLevel = Math.Min(playerLevel + 1.5, 40);
            Dictionary<double, double> levels = new Dictionary<double, double>();

            for (double level = 1; level <= maxLevel; level += 0.5)
            {
                double angle = PokemonInfo.GetLevelAngle(level, playerLevel);
                levels.Add(level, Math.Abs(levelAngle - angle));
            }

            return levels.OrderBy(p => p.Value).First().Key;
        }
        private Bitmap GetPokemonIcon(int id)
        {
            if (iconSetUri == null || GoOnTapApplication.Config.IconSetUri == null || iconSetUri.ToString() != GoOnTapApplication.Config.IconSetUri.ToString())
            {
                iconSetBitmapRegionDecoder?.Recycle();
                iconSetStream?.Dispose();

                try
                {
                    byte[] iconSetBytes = GoOnTapApplication.Config.IconSetBytes;

                    if (iconSetBytes != null)
                        iconSetStream = new MemoryStream(iconSetBytes);
                    else
                        iconSetStream = Context.ContentResolver.OpenInputStream(GoOnTapApplication.Config.IconSetUri);
                }
                catch (Exception e)
                {
                    GoOnTapApplication.Config.IconSetUri = Config.DefaultIconSets.Values.First();
                    GoOnTapApplication.Config.IconSetBytes = null;

                    iconSetStream = Context.ContentResolver.OpenInputStream(GoOnTapApplication.Config.IconSetUri);
                }

                try
                {
                    iconSetBitmapRegionDecoder = BitmapRegionDecoder.NewInstance(iconSetStream, false);
                }
                catch
                {
                    return null;
                }

                iconSetUri = GoOnTapApplication.Config.IconSetUri;
            }

            if (iconSetBitmapRegionDecoder == null)
                return null; 

            int x = (id - 1) % 10;
            int y = (id - 1) / 10;
            int width = iconSetBitmapRegionDecoder.Width / 10;
            int height = width;

            Bitmap bitmap = iconSetBitmapRegionDecoder.DecodeRegion(new Rect(width * x, height * y, width * x + width, height * y + height), new BitmapFactory.Options());
            return bitmap;
        }
    }
}