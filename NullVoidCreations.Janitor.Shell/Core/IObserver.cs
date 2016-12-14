
namespace NullVoidCreations.Janitor.Shell.Core
{
    public interface IObserver
    {
        void Update(IObserver sender, MessageCode code, params object[] data);
    }
}
