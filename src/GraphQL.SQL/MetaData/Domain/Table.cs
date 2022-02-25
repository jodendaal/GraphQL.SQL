using System.Collections.Generic;
using System.Linq;

namespace GraphQL.SQL.MetaData
{
    public class Table
    {
        private string _tableAlias;
        private string _tableName;
        private string orderBy;
        private string identityColumn;

        public bool PagingEnabled { get; set; } = true;

        public string PageSizeParameterName { get; set; } = "_PageSize";

        public string PageNumberParameterName { get; set; } = "_PageNumber";

        public string Schema { get; set; } = "dbo";

        public string TableAlias
        {
            get
            {
                return _tableAlias;
            }
            set => _tableAlias = value;
        }

        public string TableName
        {
            get
            {
                // if (!string.IsNullOrWhiteSpace(_tableAlias))
                // {
                //    return $"{_tableName} {_tableAlias}";
                // }
                return $"{_tableName}";
            }
            set => _tableName = value;
        }

        public string IdentityColumn { get => string.IsNullOrWhiteSpace(identityColumn)?PrimaryKeys.FirstOrDefault():identityColumn; set => identityColumn = value; }

        public List<string> PrimaryKeys { get; set; }

        public string OrderByDefault
        {
            get
            {
                if (orderBy != null)
                {
                    return orderBy;
                }

                if (!string.IsNullOrWhiteSpace(IdentityColumn))
                {
                    return IdentityColumn;
                }

                if (PrimaryKeys != null && PrimaryKeys.Count > 0)
                {
                    return string.Join(",", PrimaryKeys);
                }

                if (Fields.Count > 0)
                {
                    return Fields.FirstOrDefault(i => i.IsKeyField == true).Name;
                }

                if (Fields.Count > 0)
                {
                    return Fields.FirstOrDefault().Name;
                }

                return orderBy;
            }

            set => orderBy = value;
        }

        public void AddField(Column field)
        {
            this.Fields.Add(field);
        }

        public List<TableRelationShip> Relationships { get; set; } = new List<TableRelationShip>();
        public List<Column> Fields { get; set; } = new List<Column>();



        public IEnumerable<Column> GetFields(List<string> requestedFields)
        {
            var fields = Fields.Where(c => c.IsKeyField || requestedFields.Any(i => c.NameAs.ToLowerInvariant() == i.ToLowerInvariant()));
            return fields;
        }

        public string FQN()
        {
            return $"[{Schema}].[{TableName}]";
        }

        public string FQNA()
        {
            return $"[{Schema}].[{TableName}] [{TableAlias}]";
        }
    }
}
