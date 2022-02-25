using GraphQL.DataLoader;
using GraphQL.SQL.MetaData;
using GraphQL.SQL.Tests.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests.GraphQL
{
    public class BaseGraphQLTest : BaseDatabaseTest
    {
        public async Task<IServiceProvider> GetContainer()
        {
            var database = await GetDatabase();
            var container = new MockServiceProvider();
            var metaDataProvider = new AutoGenerateMetaDataProvider(new SqlSchemaToMetaData(database.ConnectionString));
            var dataContextAccessor = new DataLoaderContextAccessor();
            container.AddService(typeof(IDatabase), new Database(database.ConnectionString));
            container.AddService(typeof(IMetaDataProvider), metaDataProvider);
            container.AddService(typeof(SqlQuery), new SqlQuery(metaDataProvider));
            container.AddService(typeof(IDataLoaderContextAccessor), dataContextAccessor);
            container.AddService(typeof(DataLoaderDocumentListener), new DataLoaderDocumentListener(dataContextAccessor));
            return container;
        }
    }
}
