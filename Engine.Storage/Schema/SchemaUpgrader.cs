using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Engine.Storage.Schema.Migrations;

namespace Engine.Storage.Schema
{
    public class Upgrader
    {
        private readonly SQLiteConnection connection;
        private const int CurrentSchemaVersion = 1;

        public Upgrader(SQLiteConnection connection) => this.connection = connection;

        public void UpgradeToLatestVersion()
        {
            var currentVersion = this.GetCurrentVersion();
            if (currentVersion != CurrentSchemaVersion)
            {
                using var transaction = this.connection.BeginTransaction(IsolationLevel.Serializable);
                this.RunSchemaMigration(currentVersion);
                this.UpdateVersionNumber();
                transaction.Commit();
            }
        }

        private int GetCurrentVersion()
        {
            using var command = this.connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";

            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    // blank database
                    return 0;
                }
            }

            command.CommandText = "SELECT Version FROM VersionInformation LIMIT 1";
            var versionNumber = Convert.ToInt32(command.ExecuteScalar());
            return versionNumber;
        }

        private void RunSchemaMigration(int fromVersion)
        {
            var migrations = GetRequiredMigrations(fromVersion);
            using var command = this.connection.CreateCommand();

            foreach (var migration in migrations)
            {
                migration.Run(command);
            }
        }

        private static IEnumerable<BaseMigration> GetRequiredMigrations(int fromVersion)
        {
            if (fromVersion == 0)
            {
                yield return new Version1();
            }
        }

        private void UpdateVersionNumber()
        {
            using var command = this.connection.CreateCommand();
            command.CommandText = $"UPDATE VersionInformation SET Version = {CurrentSchemaVersion}";
            command.ExecuteNonQuery();
        }
    }
}
