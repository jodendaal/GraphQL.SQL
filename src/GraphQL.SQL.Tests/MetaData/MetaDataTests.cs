using GraphQL.SQL.Convertors;
using GraphQL.SQL.MetaData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests.MetaData
{
    [TestClass]
    public class MetaDataTests : BaseDatabaseTest
    {
        [TestMethod]
        public async Task SchemaToMetaData()
        {
            var database = await GetDatabase();
            var sc = new SqlSchemaToMetaData(database.ConnectionString);
            var result = sc.SchemaToMetaData();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Tables.Count > 0);
        }

        [TestMethod]
        public async Task MetaDataToGraphQLMetaData()
        {
            var database = await GetDatabase();
            var sc = new SqlSchemaToMetaData(database.ConnectionString);
            var metaDataProvider = new AutoGenerateMetaDataProvider(sc);
            var graphQlMetaData  = metaDataProvider.GetMetaData();

            Assert.IsNotNull(graphQlMetaData);
            Assert.IsTrue(graphQlMetaData.Tables.Count > 0);
        }

        [TestMethod]
        public async Task MetaDataToGraphQLTypes()
        {
            var database = await GetDatabase();
            var sc = new SqlSchemaToMetaData(database.ConnectionString);
            var metaDataProvider = new AutoGenerateMetaDataProvider(sc);
            var graphQlMetaData = metaDataProvider.GetMetaData();

            var graphQlTypes = graphQlMetaData.ToGraphQLTypes();

            Assert.IsNotNull(graphQlTypes);
            Assert.IsTrue(graphQlMetaData.Tables.Count > 0);
            Assert.AreEqual(graphQlTypes.Count, graphQlMetaData.Tables.Count);
        }
    }
}
