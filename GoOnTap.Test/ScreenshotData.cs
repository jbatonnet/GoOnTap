using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Sdk;

namespace GoOnTap
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ScreenshotDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null)
                throw new ArgumentNullException(nameof(testMethod));

            FileInfo[] screenshotsInfo = Program.ScreenshotsDirectory.GetFiles("*.png", SearchOption.AllDirectories).ToArray();
            Console.WriteLine("Found {0} screenshots to test", screenshotsInfo.Length);

            foreach (FileInfo screenshotInfo in screenshotsInfo)
                yield return new object[] { screenshotInfo };
        }
    }
}