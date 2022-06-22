using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Engine.Storage.Tests
{
    [TestFixture]
    [Explicit("Entry Point Tests")]
    public class EntryPoint
    {
        [Test]
        public void DataStoreInitialization_SchemaMigrations()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(assemblyLocation) ?? string.Empty;

            var dbPath = Path.Combine(directory, "media.db");

            var store = new DataStore();
            store.Initialize(dbPath);
        }
    }
}