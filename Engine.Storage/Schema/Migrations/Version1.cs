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
                "CREATE TABLE BaseDirectory ( BaseDirectoryId INT, Path TEXT NOT NULL )",
                "CREATE TABLE FileType ( FileTypeId INT, FileTypeDescription TEXT )",
                "INSERT INTO FileType ( FileTypeId , FileTypeDescription ) VALUES (1, 'Image'), (2, 'Video')",
                "CREATE TABLE FileInfo ( " +
                    "FileTypeId INT, BaseDirectoryId INT, Directory TEXT NOT NULL, File TEXT NOT NULL, " +
                    "Thumbnail BLOB, FileWidth INT, FileHeight INT, FileLength INT, FileSize INT )",
            };
        }
    }
}
