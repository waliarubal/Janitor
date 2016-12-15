using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Models;

namespace NullVoidCreations.Janitor.Plugin.Browser.Firefox
{
    public class FirefoxTarget: ScanTargetBase
    {
        readonly List<string> _profiles;

        public FirefoxTarget()
            : base("Mozilla Firefox", new Version("1.0.0.0"), DateTime.Now)
        {
            IconSource = "/NullVoidCreations.Janitor.Plugin.Browser;component/Resources/Firefox.png";

            var configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Mozilla\Firefox\profiles.ini");

            _profiles = new List<string>();
            foreach (var line in File.ReadAllLines(configFile))
            {
                if (line.StartsWith("Path=Profiles/", StringComparison.InvariantCultureIgnoreCase))
                    _profiles.Add(line.Substring("Path=Profiles/".Length));
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
