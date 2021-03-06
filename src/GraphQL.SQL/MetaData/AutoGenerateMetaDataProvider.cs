using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace GraphQL.SQL.MetaData
{
    public class AutoGenerateMetaDataProvider : IMetaDataProvider
    {
        private GraphQLSqlMetaData _graphQlMetadata;
        private readonly ISqlSchemaToMetaData _sqlSchemaToMetaData;

        public AutoGenerateMetaDataProvider(ISqlSchemaToMetaData sqlSchemaToMetaData)
        {
            _sqlSchemaToMetaData = sqlSchemaToMetaData;
        }

        public virtual GraphQLSqlMetaData GetMetaData()
        {
            if (_graphQlMetadata == null)
            {
                _graphQlMetadata = new GraphQLSqlMetaData();
                _graphQlMetadata.Tables = new List<GraphQLTableMetaData>();
                var metaData = _sqlSchemaToMetaData.SchemaToMetaData();
                metaData.Tables.ForEach(i =>
                {
                    _graphQlMetadata.Tables.Add(new GraphQLTableMetaData(i));
                });

                _graphQlMetadata.RelationShips = metaData.RelationShips;

                SaveJson(_graphQlMetadata);
            }

           

            return _graphQlMetadata;
        }

        private void SaveJson(GraphQLSqlMetaData graphQlMetadata)
        {
            File.WriteAllText("database-schema.json", JsonConvert.SerializeObject(graphQlMetadata));
        }
    }
}
