using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Provider;
using Android.Content;
using Android.Utilities;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Runtime;

using Uri = Android.Net.Uri;
using Java.IO;
using Android.Support.Design.Widget;

namespace GoOnTap.Android
{
    [Activity(Label = "Go on Tap", MainLauncher = true, Icon = "@mipmap/ic_launcher", Theme = "@style/AppTheme.NoActionBar")]
    public class HomeActivity : BaseActivity
    {
        private const int PICK_IMAGE = 1;

        private bool isAssistant = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState, Resource.Layout.HomeActivity);

            // Subscribe events
            FindViewById(Resource.Id.SettingsButton).Click += (s, e) =>
            {
                StartActivity(new Intent(Settings.ActionVoiceInputSettings));

                RefreshIsAssistant();
                if (!isAssistant)
                    Toast.MakeText(this, "Tap on assist app to select Go on Tap", ToastLength.Long).Show();
            };
        }
        protected override void OnResume()
        {
            base.OnResume();

            // Check if we are the default assistant
            RefreshIsAssistant();

            View statusView = FindViewById(Resource.Id.StatusView);
            TextView statusLabel = FindViewById<TextView>(Resource.Id.StatusLabel);

            if (isAssistant)
            {
                statusLabel.Text = $"{GoOnTapApplication.Instance.Name} is the default assist app";

                statusView.SetBackgroundColor(Color.Honeydew);
                statusLabel.SetTextColor(Color.SeaGreen);
            }
            else
            {
                statusLabel.Text = $"{GoOnTapApplication.Instance.Name} is not the default assist app";

                statusView.SetBackgroundColor(Color.MistyRose);
                statusLabel.SetTextColor(Color.Crimson);
            }
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.HomeMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.HomeMenu_Settings:
                {
                    StartActivity(typeof(SettingsActivity));
                    break;
                }
            }

            return true;
        }

        private void RefreshIsAssistant()
        {
            string assistant = Settings.Secure.GetString(ContentResolver, "voice_interaction_service");
            isAssistant = false;

            if (assistant != null)
            {
                ComponentName componentName = ComponentName.UnflattenFromString(assistant);
                if (componentName != null && componentName.PackageName == PackageName)
                    isAssistant = true;
            }
        }
    }
}