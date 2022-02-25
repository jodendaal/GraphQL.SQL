using GraphQL.SQL.MetaData;
using GraphQL.Types;

namespace GraphQL.SQL.Types
{
    public class TableFieldListRelationshipType : TableFieldType
    {
        public TableFieldListRelationshipType(TableObjectGraphType tableType, GraphQLTableMetaData childMetaData,TableRelationShip relationShip) : base(tableType)
        {
            Name = relationShip.FieldName;
            ResolvedType = new ListGraphType(base.ResolvedType);
            Resolver = new TableFieldTypeResolver(childMetaData);
            Metadata.Add("relationshipName", relationShip.FieldName);
            Metadata.Add("relationshipNameParentKey", relationShip.ParentKeyField);
            Metadata.Add("relationshipNameChildKey", relationShip.ChildKeyField);
           // Arguments = new TableQueryArgsType(childMetaData);
        }
    }
}
