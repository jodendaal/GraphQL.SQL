using LocalDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests
{
    [TestClass]
    public class TestDatabaseCreation
    {
        static SqlInstance sqlInstance;
        static TestDatabaseCreation()
        {
            sqlInstance = new(
             name: "GraphQLTester1",
             buildTemplate: TestDatabase.CreateSchema);
        }

        [TestMethod]
        public async Task BuildDatabase()
        {
            await using var database = await sqlInstance.Build();
        }
    }
}
