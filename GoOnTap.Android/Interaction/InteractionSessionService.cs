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
    }
}