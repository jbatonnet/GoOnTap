using System;

using Android;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Service.Voice;

namespace GoOnTap.Android
{
    //[Service(Permission = Manifest.Permission.BindVoiceInteraction)]
    //[MetaData("android.voice_interaction", Resource = "@xml/interaction_service")]
    //[IntentFilter(new[] { Intent.Voice })]
    [Register("net.thedju.GoOnTap.InteractionService")]
    public class GoOnTapInteractionService : VoiceInteractionService
    {
        public static GoOnTapInteractionService Instance { get; private set; }

        public GoOnTapInteractionService()
        {
            Instance = this;
        }
    }
}