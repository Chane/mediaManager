using System.Data.SQLite;
using Engine.Storage.Schema;

namespace Engine.Storage
{
    public class DataStore
    {
        private SQLiteConnection connection;

        public void Initialize(string path)
        {
            this.connection = new SQLiteConnection($"Data Source={path};Version=3;New=False;Compress=True;") { DefaultTimeout = 60 };
            this.connection.Open();

            var upgrader = new Upgrader(this.connection);
            upgrader.UpgradeToLatestVersion();
        }
    }
}
