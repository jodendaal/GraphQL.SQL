using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace GraphQL.SQL.MetaData
{
    public class JsonMetaDataProvider : IMetaDataProvider
    {
        private GraphQLSqlMetaData _graphQlMetadata;

        public JsonMetaDataProvider(string fileName)
        {

        }

        public virtual GraphQLSqlMetaData GetMetaData()
        {
            if (_graphQlMetadata == null)
            {
                _graphQlMetadata = LoadJson("database-schema.json");
            }

            return _graphQlMetadata;
        }

        private GraphQLSqlMetaData LoadJson(string fileName)
        {
            return JsonConvert.DeserializeObject<GraphQLSqlMetaData>(File.ReadAllText(fileName));
        }
    }
}
