using GraphQL.SQL.MetaData;
using GraphQL.Types;

namespace GraphQL.SQL.Types
{
    public class TableQueryArgsType : QueryArguments
    {
        public TableQueryArgsType(GraphQLTableMetaData table)
        {
            if (!string.IsNullOrWhiteSpace(table.IdentityColumn))
            {
                this.Add(new QueryArgument(typeof(IntGraphType)) { Name = table.IdentityColumn, Description = "id of record" });
            }

            if (table.PagingEnabled)
            {
                this.Add(new QueryArgument(typeof(IntGraphType)) { Name = "page", Description = "page" });
                this.Add(new QueryArgument(typeof(IntGraphType)) { Name = "pageSize", Description = "pageSize" });
            }

            var filter = new TableFilterSetType(table) { Name = $"{table.NameAs}Filters" };
            filter.AddField(new FieldType()
            {
                Name = "andSet",
                Type = filter.GetType(),
                ResolvedType = filter
            });

            filter.AddField(new FieldType()
            {
                Name = "orSet",
                Type = filter.GetType(),
                ResolvedType = filter
            });

            this.Add(new QueryArgument(filter) { Name = "filter" });
        }
    }
}
