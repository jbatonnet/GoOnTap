using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Android.Content;
using Android.Net;
using Android.Utilities;

namespace GoOnTap.Android
{
    public class Config : BaseConfig
    {
        public static Dictionary<string, Uri> DefaultIconSets { get; } = new Dictionary<string, Uri>()
        {
            ["Icons by Geovanny Gavilanes"] = Uri.Parse("android.resource://net.thedju.GoOnTap/" + Resource.Drawable.IconSet1),
            ["Icons by roundicons.com"] = Uri.Parse("android.resource://net.thedju.GoOnTap/" + Resource.Drawable.IconSet2),
        };

        [DisplayName("Player level")]
        [Description("Current level of Pokemon Go player")]
        public int PlayerLevel
        {
            get
            {
                return GetValue(24);
            }
            set
            {
                SetValue(value);
            }
        }

        [DisplayName("Icon set")]
        [Description("Current Pokemon icon set")]
        public Uri IconSetUri
        {
            get
            {
                return GetValue<Uri>(null);
            }
            set
            {
                SetValue(value);
            }
        }

        [Browsable(false)]
        public byte[] IconSetBytes
        {
            get
            {
                return CacheValue<byte[]>(null);
            }
            set
            {
                SetValue(value);
            }
        }

        public Config(Context context) : base(context) { }
    }
}