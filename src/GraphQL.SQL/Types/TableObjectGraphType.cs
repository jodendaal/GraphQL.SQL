using GraphQL.SQL.Convertors;
using GraphQL.SQL.MetaData;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.SQL.Types
{
    public class TableObjectGraphType : ObjectGraphType
    {
        public GraphQLTableMetaData SqlMetaData { get; }

        public TableObjectGraphType(GraphQLTableMetaData metaData)
        {
            Name = metaData.NameAs;

            foreach (var field in metaData.Fields)
            {
                FieldAsync(field.SqlType.ToGraphQLType(),field.NameAs, resolve: (context) =>
                {
                    DataRow row;
                    var data = context.Source as IGrouping<string, DataRow>;
                    if(data != null)
                    {
                        row = data.First();
                    }
                    else
                    {
                        row = context.Source as DataRow;
                    }

                    if (row != null)
                    {
                        return Task.FromResult(row[field.Name]);
                    }

                    return null;
                });
            }

            SqlMetaData = metaData;
        }
    }
}
