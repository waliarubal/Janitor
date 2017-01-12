using System;
using System.Collections.Generic;
using System.IO;
using NullVoidCreations.Janitor.Shared.Helpers;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class CommandLineManager: ISignalObserver
    {
        public class CommandLineArgument
        {
            public const string Silent = "silent";
            public const string Keygen = "keygen";
            public const string Email = "email";
            public const string NoOfDays = "days";
            public const string KeyFile = "key_file";
            public const string SecondInstance = "secondary";
            public const string FixIssues = "fix";
        }

        readonly Dictionary<string, string> _arguments;
        readonly string[] _coreArguments;
        static CommandLineManager _instance;

        private CommandLineManager()
        {
            _coreArguments = new string[] 
            {
                CommandLineArgument.Silent,
                CommandLineArgument.Keygen
            };
            _arguments = new Dictionary<string, string>();
        }

        #region properties

        public static CommandLineManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CommandLineManager();

                return _instance;
            }
        }

        public string this[string parameter]
        {
            get
            {
                if (_arguments.ContainsKey(parameter))
                    return _arguments[parameter];

                return null;
            }
        }

        #endregion

        bool GenerateKey()
        {
            var email = this[CommandLineArgument.Email];
            var days = this[CommandLineArgument.NoOfDays];
            var keyFile = this[CommandLineArgument.KeyFile];
            if (email == null || days == null || keyFile == null)
                return false;

            int noOfDays;
            if (!int.TryParse(days, out noOfDays))
                return false;

            var key = LicenseExManager.Instance.GenerateLicenseKey(email, noOfDays);
            if (!FileSystemHelper.Instance.DeleteFile(keyFile))
                return false;

            File.WriteAllText(keyFile, key);
            return true;
        }

        public void ProcessArguments()
        {
            foreach (var arg in _coreArguments)
            {
                if (!_arguments.ContainsKey(arg))
                    continue;

                switch(arg)
                {
                    case CommandLineArgument.Silent:
                        SignalHost.Instance.RaiseSignal(this, Signal.CloseToTray);
                        break;

                    case CommandLineArgument.Keygen:
                        GenerateKey();
                        break;
                }
            }
        }

        public void LoadArguments(string[] arguments)
        {
            _arguments.Clear();
            foreach (var argument in arguments)
            {
                if (string.IsNullOrEmpty(argument))
                    continue;
                if (!argument.StartsWith("/"))
                    continue;

                var parts = argument.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    var argName = parts[0].Remove(0, 1);
                    var argValue = string.Empty;
                    if (parts.Length > 1)
                        argValue = parts[1];

                    if (_arguments.ContainsKey(argName))
                        _arguments[argName] = argValue;
                    else
                        _arguments.Add(argName, argValue);
                }
            }
        }

        public void SignalReceived(ISignalObserver sender, Signal signal, params object[] data)
        {
            
        }
    }
}
