using GraphQL.Types;
using System;

namespace GraphQL.SQL
{
    public class SqlSchema : Schema
    {
        public SqlSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = provider.GetService(typeof(SqlQuery)) as SqlQuery;
        }
    }
}
