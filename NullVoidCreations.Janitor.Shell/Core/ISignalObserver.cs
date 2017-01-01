
namespace NullVoidCreations.Janitor.Shell.Core
{
    public interface ISignalObserver
    {
        void Update(ISignalObserver sender, Signal code, params object[] data);
    }
}
