using GraphQL.SQL.MetaData;
using GraphQL.Types;

namespace GraphQL.SQL.Types
{
    public class TableFilterSetType : InputObjectGraphType
    {
        public TableFilterSetType(GraphQLTableMetaData table)
        {
            var t = new TableFilterType(table);
            AddField(new FieldType()
            {
                Name = "and",
                Type = t.GetType(),
                ResolvedType = t
            });

            AddField(new FieldType()
            {
                Name = "or",
                Type = t.GetType(),
                ResolvedType = t
            });
        }
    }
}
