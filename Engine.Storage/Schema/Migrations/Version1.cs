namespace Engine.Storage.Schema.Migrations
{
    public class Version1 : BaseMigration
    {
        public Version1()
        {
            Queries = new[]
            {
                "CREATE TABLE VersionInformation ( Version INTEGER NOT NULL )",
                "INSERT INTO VersionInformation (Version) VALUES (0)",
            };
        }
    }
}
