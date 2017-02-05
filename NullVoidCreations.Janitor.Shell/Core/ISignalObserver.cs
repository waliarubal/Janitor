
namespace NullVoidCreations.Janitor.Shell.Core
{
    public interface ISignalObserver
    {
        void SignalReceived(Signal signal, params object[] data);
    }
}
