using System.Collections.Generic;

namespace GraphQL.SQL.MetaData
{
    public class Column
    {
        public Column(string name = null, string sqlType = null)
        {
            Name = name;
            SqlType = sqlType;
        }

        private string _generationName;

        /// <summary>
        /// Determines if the field is a identity, primary key or foreign key.
        /// Will always be inlcude in select queries for relational data to use for queries.
        /// </summary>
        public bool IsKeyField { get; set; }

        public string Name { get; set; }

        public string NameAs { get => string.IsNullOrWhiteSpace(_generationName) ? Name : _generationName; set => _generationName = value; }

        public string SqlType { get; set; }

        public List<Restriction> Restrictions { get; set; }

        /// <summary>
        /// Field will be generated for the specfied Operations.
        /// </summary>
        public List<string> AllowedOperations { get; set; } = new List<string>() { TableOperations.Select , TableOperations.Update , TableOperations.Insert, TableOperations.Delete };
    }
}
