using System;
using System.Collections.Generic;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class CommandLineManager
    {
        public class CommandLineArgument
        {
            public const string Silent = "silent";
            public const string SecondInstance = "secondary";
            public const string SmartScan = "smart";
        }

        readonly Dictionary<string, string> _arguments;
        readonly string[] _coreArguments;
        static CommandLineManager _instance;

        private CommandLineManager()
        {
            _coreArguments = new string[] 
            {
                CommandLineArgument.Silent,
                CommandLineArgument.SecondInstance,
                CommandLineArgument.SmartScan
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

        public void ProcessArguments()
        {
            foreach (var arg in _coreArguments)
            {
                if (!_arguments.ContainsKey(arg))
                    continue;

                switch(arg)
                {
                    case CommandLineArgument.Silent:
                        SignalHost.Instance.RaiseSignal(Signal.CloseToTray);
                        break;

                    case CommandLineArgument.SmartScan:
                        WorkQueueManager.Instance.AddWork(WorkSignal.SmartScan);
                        WorkQueueManager.Instance.DoWork();
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
    }
}
