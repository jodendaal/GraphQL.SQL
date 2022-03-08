using GraphQL.SQL.Convertors;
using GraphQL.SQL.MetaData;
using GraphQL.Types;
using System.Linq;

namespace GraphQL.SQL.Types
{
    public class TableQueryArgsType : QueryArguments
    {
        public TableQueryArgsType(GraphQLTableMetaData table, bool isRelationShip = false)
        {
            if (!string.IsNullOrWhiteSpace(table.IdentityColumn) && !isRelationShip)
            {
                var field = table.Fields.FirstOrDefault(i => i.Name == table.IdentityColumn);
                this.Add(new QueryArgument(field.SqlType.ToGraphQLType()) { Name = table.IdentityColumn, Description = "id of record" });
            }

            if (table.PagingEnabled)
            {
                this.Add(new QueryArgument(typeof(IntGraphType)) { Name = "page", Description = "page" });
                this.Add(new QueryArgument(typeof(IntGraphType)) { Name = "pageSize", Description = "pageSize" });
            }

             if (!isRelationShip)
              {
                var filter = new TableFilterSetType(table) { Name = !isRelationShip ? $"{table.NameAs}Filters" : $"{table.NameAs}FiltersRel" };

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
}
