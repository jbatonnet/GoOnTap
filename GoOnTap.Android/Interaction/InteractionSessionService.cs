using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Service.Voice;

namespace GoOnTap.Android
{
    [Register("net.thedju.GoOnTap.InteractionSessionService")]
    public class GoOnTapInteractionSessionService : VoiceInteractionSessionService
    {
        public override VoiceInteractionSession OnNewSession(Bundle args)
        {
            return new GoOnTapInteractionSession(this);
        }

        /*public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);
            return StartCommandResult.Sticky;
        }*/
    }
}