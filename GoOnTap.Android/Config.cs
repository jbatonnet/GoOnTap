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

        [Browsable(false)]
        public int PlayerLevel
        {
            get
            {
                int value = GetValue(24);
                if (value < 1) value = 1;
                if (value > 40) value = 40;

                return value;
            }
            set
            {
                if (value < 1) value = 1;
                if (value > 40) value = 40;

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