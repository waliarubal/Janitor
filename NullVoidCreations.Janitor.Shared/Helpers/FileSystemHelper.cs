
using System.IO;
namespace NullVoidCreations.Janitor.Shared.Helpers
{
    public class FileSystemHelper
    {
        volatile static FileSystemHelper _instance;

        private FileSystemHelper()
        {

        }

        #region properties

        public static FileSystemHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FileSystemHelper();

                return _instance;
            }
        }

        #endregion

        public bool DeleteFile(string path)
        {
            var isDeleted = true;
            try
            {
                File.Delete(path);
            }
            catch
            {
                isDeleted = false;
            }

            return isDeleted;
        }
    }
}
