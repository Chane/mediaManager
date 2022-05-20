using System.Collections.Generic;
using System.Data.SQLite;

namespace Engine.Storage.Schema.Migrations
{
    public abstract class BaseMigration
    {
        protected IEnumerable<string> Queries;

        public void Run(SQLiteCommand command)
        {
            if (Queries != null)
            {
                foreach (var query in Queries)
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
