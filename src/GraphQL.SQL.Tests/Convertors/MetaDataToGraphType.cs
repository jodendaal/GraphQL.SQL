using GraphQL.SQL.Convertors;
using GraphQL.SQL.MetaData;
using GraphQL.Types;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests.Convertors
{
    [TestClass]
    public class MetaDataToGraphType : BaseDatabaseTest
    {
        [TestMethod]
        public void TableToGraphQLObject()
        {
            var result = new List<GraphQLTableMetaData>();
            var usersTable = new GraphQLTableMetaData()
            {
                TableName = "Users",
                TableAlias = "U",
                Schema = "dbo",
                IdentityColumn = "UserId"
            };

            usersTable.AddField(new Column() { Name = "userId", SqlType = "int", IsKeyField = true });
            usersTable.AddField(new Column() { Name = "username", SqlType = "nvarchar", IsKeyField = false });

            var graphQLType = usersTable.ToObjectGraph();

            Assert.AreEqual(graphQLType.Fields.Count, usersTable.Fields.Count);
            Assert.AreEqual(graphQLType.Name, usersTable.NameAs);
            Assert.AreEqual(graphQLType.Fields.FirstOrDefault(i=>i.Name == "userId").Type, typeof(IntGraphType));
            Assert.AreEqual(graphQLType.Fields.FirstOrDefault(i => i.Name == "username").Type, typeof(StringGraphType));
        }
    }
}
