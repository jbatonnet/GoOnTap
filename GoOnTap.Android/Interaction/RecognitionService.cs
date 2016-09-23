using System;

using Android.Content;
using Android.Runtime;
using Android.Speech;

namespace GoOnTap
{
    [Register("net.thedju.GoOnTap.RecognitionService")]
    public class GoOnTapRecognitionService : RecognitionService
    {
        public override void OnCreate()
        {
            base.OnCreate();
            Log.Trace("RecognitionService.OnCreate");
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Trace("RecognitionService.OnDestroy");
        }

        protected override void OnStartListening(Intent recognizerIntent, Callback listener)
        {
            Log.Trace("RecognitionService.OnStartListening");
        }
        protected override void OnCancel(Callback listener)
        {
            Log.Trace("RecognitionService.OnCancel");
        }
        protected override void OnStopListening(Callback listener)
        {
            Log.Trace("RecognitionService.OnStopListening");
        }
    }
}