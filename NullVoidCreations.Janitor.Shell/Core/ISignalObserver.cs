
namespace NullVoidCreations.Janitor.Shell.Core
{
    public interface ISignalObserver
    {
        void SignalReceived(ISignalObserver sender, Signal signal, params object[] data);
    }
}
