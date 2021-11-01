namespace GraphQL.SQL.MetaData
{
    public class TableRelationShip
    {
        public string ParentTableSchema { get; set; }

        public string ParentTable { get; set; }

        public string ChildTableSchema { get; set; }

        public string ChildTable { get; set; }

        public string ParentKeyField { get; set; }

        public string ChildKeyField { get; set; }

        public string FieldName { get; set; }

        public bool IsList { get; set; }

        private Table childMetaData;

        public Table GetChildMetaData()
        {
            return childMetaData;
        }

        public void SetChildMetaData(Table metaData)
        {
            childMetaData = metaData;
        }
    }
}
