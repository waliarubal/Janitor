using System;
using System.Collections.Generic;
using NullVoidCreations.Janitor.Shared.Base;

namespace NullVoidCreations.Janitor.Plugin.Miscellaneous.Multimedia
{
    public class MultimediaTarget: ScanTargetBase
    {
        public MultimediaTarget()
            : base("Multimedia", new Version("17.3.22.0"), new DateTime(2017, 3, 22))
        {
            IconSource = "pack://application:,,,/plugin_miscellaneous;component/Resources/Multimedia22.png";

            var areas = new List<ScanAreaBase>()
            {
                new MultimediaAreaAdobeFlashPlayer(this),
                new MultimediaAreaSteam(this),
                new MultimediaAreaVlcMediaPlayer(this),
                new MultimediaAreaWindowsMediaPlayer(this)
            };
            Areas = areas;
        }
    }
}
