using System.ComponentModel;
using System.Collections.Generic;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class NotificationBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        readonly Dictionary<string, object> _properties;

        protected NotificationBase()
        {
            _properties = new Dictionary<string, object>();
        }

        protected object this[string key]
        {
            get { return GetValue<object>(key); }
            set
            {
                if (_properties.ContainsKey(key))
                {
                    var currentValue = _properties[key];
                    if (value == currentValue)
                        return;

                    _properties[key] = value;
                    RaisePropertyChanged(key);
                }
                else
                {
                    _properties.Add(key, value);
                    RaisePropertyChanged(key);
                }
            }
        }

        protected T GetValue<T>(string key)
        {
            if (_properties.ContainsKey(key))
                return (T)_properties[key];

            return default(T);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null || string.IsNullOrEmpty(propertyName))
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
