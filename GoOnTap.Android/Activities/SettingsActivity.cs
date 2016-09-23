using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Support.V4.App;
using Android.Utilities;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Uri = Android.Net.Uri;

namespace GoOnTap.Android
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class SettingsActivity : BaseActivity
    {
        private const int PICK_IMAGE = 1;

        public class GoOnTapSettingsFragment : SettingsFragment
        {
            public ListPreference IconSetPreference { get; private set; }

            public GoOnTapSettingsFragment(BaseConfig config) : base(config) { }

            protected override void OnAddPreferences(PreferenceScreen preferenceScreen)
            {
                base.OnAddPreferences(preferenceScreen);

                // Add about button
                Preference aboutPreference = new Preference(Context);
                aboutPreference.Title = "About";
                aboutPreference.Summary = "App info and credits";
                aboutPreference.PreferenceClick += AboutPreference_PreferenceClick;
                preferenceScreen.AddPreference(aboutPreference);
            }
            protected override Preference CreatePreference(PropertyInfo property)
            {
                if (property.Name == nameof(Config.IconSetUri))
                {
                    IconSetPreference = new ListPreference(Activity);

                    IconSetPreference.Key = property.Name;
                    IconSetPreference.Title = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

                    IconSetPreference.SetEntries(Config.DefaultIconSets.Keys.Concat("Other icon set").ToArray());
                    IconSetPreference.SetEntryValues(Config.DefaultIconSets.Values.Select(u => u.ToString()).Concat("").ToArray());

                    IconSetPreference.PreferenceChange += IconSetPreference_PreferenceChange;

                    return IconSetPreference;
                }

                return base.CreatePreference(property);
            }
            protected override void GetPreference(Preference preference, PropertyInfo property)
            {
                if (property.Name == nameof(Config.IconSetUri))
                {
                    int index = Config.DefaultIconSets.Values.IndexOf(u => u.ToString() == GoOnTapApplication.Config.IconSetUri?.ToString());

                    preference.Summary = index == -1 ? GoOnTapApplication.Config.IconSetUri?.Path : Config.DefaultIconSets.Keys.ElementAt(index);
                    (preference as ListPreference).SetValueIndex(index == -1 ? Config.DefaultIconSets.Count : index);
                }
                else
                    base.GetPreference(preference, property);
            }

            private void IconSetPreference_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
            {
                string selectedValue = e.NewValue.ToString();

                if (string.IsNullOrEmpty(selectedValue))
                {
                    Intent getIntent = new Intent(Intent.ActionGetContent);
                    getIntent.SetType("image/*");

                    Intent pickIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                    pickIntent.SetType("image/*");

                    Intent chooserIntent = Intent.CreateChooser(getIntent, "Select an icon set");
                    chooserIntent.PutExtra(Intent.ExtraInitialIntents, new Intent[] { pickIntent });

                    Activity.StartActivityForResult(chooserIntent, PICK_IMAGE);
                }
                else
                {
                    Uri iconSetUri = Uri.Parse(selectedValue);

                    GoOnTapApplication.Config.IconSetUri = iconSetUri;
                    GoOnTapApplication.Config.IconSetBytes = null;

                    // Refresh interface
                    int index = Config.DefaultIconSets.Values.IndexOf(u => u.ToString() == iconSetUri?.ToString());

                    e.Preference.Summary = index == -1 ? iconSetUri?.Path : Config.DefaultIconSets.Keys.ElementAt(index);
                    (e.Preference as ListPreference).SetValueIndex(index == -1 ? Config.DefaultIconSets.Count : index);
                }
            }
            private void AboutPreference_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
            {
                AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(Context)
                    .SetTitle($"About {GoOnTapApplication.Instance.Name}")
                    .SetMessage($"{GoOnTapApplication.Instance.Name} app has been developed by Julien Batonnet.\n\nDefault Iconsets built with icons from Geovanny Gavilanes (https://www.iconfinder.com/iconsets/151-1) and from roundicons.com (https://www.iconfinder.com/iconsets/pokemon-go-vol-1).\n\nPokémon, Pokémon character names are trademarks of Nintendo.")
                    .SetPositiveButton("OK", (a, b) => { });

                AlertDialog dialog = dialogBuilder.Create();
                dialog.Show();
            }
        }

        private GoOnTapSettingsFragment settingsFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            OnCreate(savedInstanceState, Resource.Layout.SettingsActivity);
            Title = "Settings";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.SettingsActivity_Fragment, settingsFragment = new GoOnTapSettingsFragment(GoOnTapApplication.Config))
                           .Commit();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
                return;

            if (requestCode == PICK_IMAGE)
            {
                GoOnTapApplication.Config.IconSetUri = data.Data;

                // Read icon set
                ParcelFileDescriptor parcelFileDescriptor = ContentResolver.OpenFileDescriptor(data.Data, "r");
                FileDescriptor fileDescriptor = parcelFileDescriptor.FileDescriptor;

                FileInputStream fileInputStream = new FileInputStream(fileDescriptor);
                MemoryStream memoryStream = new MemoryStream();

                byte[] buffer = new byte[1024];
                int count;
                while ((count = fileInputStream.Read(buffer, 0, buffer.Length)) > 0)
                    memoryStream.Write(buffer, 0, count);

                fileInputStream.Close();
                parcelFileDescriptor.Close();

                GoOnTapApplication.Config.IconSetBytes = memoryStream.ToArray();
            }
        }
    }
}