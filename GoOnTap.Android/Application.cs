using System;
using System.IO;
using Android.App;
using Android.Graphics;
using Android.Runtime;
using Android.Utilities;
using Java.Lang;

namespace GoOnTap.Android
{
    [Application]
    public class GoOnTapApplication : BaseApplication
    {
        public override string Name => "Go on Tap";
        
        public static Config Config { get; private set; }

        public GoOnTapApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate();

            Config = new Config(this);
            Log.Verbosity = LogVerbosity.Trace;
        }
    }
}