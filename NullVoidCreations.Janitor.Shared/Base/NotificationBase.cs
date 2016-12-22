using System.ComponentModel;

namespace NullVoidCreations.Janitor.Shared.Base
{
    public abstract class NotificationBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null || string.IsNullOrEmpty(propertyName))
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
