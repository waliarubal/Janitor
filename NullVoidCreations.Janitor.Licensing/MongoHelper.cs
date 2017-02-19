using MongoDB.Driver;

namespace NullVoidCreations.Janitor.Licensing
{
    class MongoHelper
    {
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

        #endregion

        public MongoDatabase GetConnection()
        {
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
