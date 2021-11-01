using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GraphQL.SQL.MetaData
{
    public class GraphQLTableMetaData : Table
    {
        public GraphQLTableMetaData()
        {
        }

        public GraphQLTableMetaData(Table tableMetaData)
        {
            this.Fields = tableMetaData.Fields;
            this.TableAlias = tableMetaData.TableAlias;
            this.TableName = tableMetaData.TableName;
            this.IdentityColumn = tableMetaData.IdentityColumn;
            this.Schema = tableMetaData.Schema;
            this.Relationships = tableMetaData.Relationships;
        }

        private string _nameAs;

        public string NameAs { get => string.IsNullOrWhiteSpace(_nameAs) ? this.EnsureNameIsCorrect(TableName) : _nameAs; set => _nameAs = this.EnsureNameIsCorrect(value); }

        public List<string> TableGraphTypes { get; set; } = new List<string>() { TableGraphType.Mulitple , TableGraphType.Single };

        public List<string> AllowedTableOperations { get; set; }  = new List<string>() { TableOperations.Select , TableOperations.Update , TableOperations.Insert, TableOperations.Delete };

        public List<Restriction> Restrictions { get; set; } = new List<Restriction>();

        public void AddRestriction(Restriction restriction)
        {
            this.Restrictions.Add(restriction);
        }

        private string EnsureNameIsCorrect(string name)
        {
            var match = Regex.Match(name, "^[_a-zA-Z][_a-zA-Z0-9]*$/");
            if (!match.Success)
            {
                name = name.Replace(" ", string.Empty).ToCamelCase(); 
            }

            return name;
        }
    }
}
