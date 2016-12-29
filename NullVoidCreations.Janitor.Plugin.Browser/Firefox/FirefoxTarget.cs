using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Base;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Plugin.Browser.Firefox
{
    public class FirefoxTarget: ScanTargetBase
    {
        readonly List<string> _profiles;

        public FirefoxTarget()
            : base("Mozilla Firefox", new Version("1.0.0.0"), new DateTime(2016, 12, 31))
        {
            IconSource = "/NullVoidCreations.Janitor.Plugin.Browser;component/Resources/Firefox.png";
            _profiles = new List<string>();

            var configFile = Path.Combine(KnownPaths.Instance.AppDataRoaming, @"Mozilla\Firefox\profiles.ini");
            if (File.Exists(configFile))
            {
                foreach (var line in File.ReadAllLines(configFile))
                {
                    if (line.StartsWith("Path=Profiles/", StringComparison.InvariantCultureIgnoreCase))
                        _profiles.Add(line.Substring("Path=Profiles/".Length));
                }
            }

            var areas = new List<ScanAreaBase>()
            {
                new FirefoxAreaCache(this),
                new FirefoxAreaHistory(this),
                new FirefoxAreaSession(this)
            };
            Areas = areas;
        }

        internal List<string> Profiles
        {
            get { return _profiles; }
        }
    }
}
