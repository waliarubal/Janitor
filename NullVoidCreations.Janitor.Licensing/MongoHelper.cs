using MongoDB.Driver;
using System.Runtime.InteropServices;

namespace NullVoidCreations.Janitor.Licensing
{
    class MongoHelper
    {
        [DllImport("wininet", SetLastError = true)]
        static extern bool InternetGetConnectedState(out int lpdwFlags, int dwReserved);

        static MongoHelper _instance;

        #region properties

        public static MongoHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MongoHelper();

                return _instance;
            }
        }

        public bool IsInternetAvailable
        {
            get
            {
                int flags;
                return InternetGetConnectedState(out flags, 0);
            }
        }

        #endregion

        public MongoDatabase GetConnection()
        {
            if (!IsInternetAvailable)
                return null;

            var connString = new MongoConnectionStringBuilder();
            connString.ConnectionMode = ConnectionMode.Automatic;
            connString.DatabaseName = "windoc";
            connString.Server = new MongoServerAddress("ds153709.mlab.com", 53709);
            connString.Username = "admin";
            connString.Password = "control*88";

            var client = new MongoClient(connString.ConnectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("windoc");
            return database;
        }
    }
}
