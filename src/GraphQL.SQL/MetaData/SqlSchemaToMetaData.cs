using GraphQL.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQL.SQL.MetaData
{
    public class SqlSchemaToMetaData : ISqlSchemaToMetaData
    {
        private readonly string _connectionString;

        public SqlSchemaToMetaData(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DatabaseSchema SchemaToMetaData()
        {
            var result = new DatabaseSchema();
            var tables = new List<Table>();
            var command = @"SELECT distinct
                            s.name as TableSchema,
                            t.name as TableName,
                            STUFF((SELECT  ',' +  COL_NAME(iic.OBJECT_ID,iic.column_id) FROM  sys.indexes AS ii INNER JOIN 
                                        sys.index_columns AS iic ON  ii.OBJECT_ID = iic.OBJECT_ID
                                                                AND ii.index_id = iic.index_id
                                WHERE   ii.is_primary_key = 1
                                         and ii.object_id = t.object_id
                                        FOR XML PATH('')), 1, 1, '') AS PrimaryKeys,
                            ( select name from sys.columns
                            where is_identity = 1
                            and object_id = t.object_id) as IdentityColumn
                            FROM
                            sys.schemas AS s
                            INNER JOIN sys.tables AS t ON s.schema_id = t.schema_id";

            using (var con = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                con.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand(command, con))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            tables.Add(new Table()
                            {
                                TableName = rdr["TableName"].ToString(),
                                PrimaryKeys = rdr["PrimaryKeys"].ToString().Split(',').ToList(),
                                IdentityColumn = rdr["IdentityColumn"].ToString(),
                                Schema = rdr["TableSchema"].ToString(),
                                TableAlias = rdr["TableName"].ToString()
                            });
                        }
                    }
                }

                var columnsSql = @"SELECT 
                                    C.name as ColumnName,
                                    s.name as tableSchema,
                                        t.name as TableName,
	                                    case when ic.name is null then 0 else 1 end as IsIdentity,
	                                    c.is_nullable,
	                                    TYPE_NAME(c.system_type_id) as DataType,
	                                    c.precision as DataTypePrecision,
	                                    c.scale as DataTypeScale
                                    FROM
                                        sys.schemas AS s
                                        INNER JOIN sys.tables AS t ON s.schema_id = t.schema_id
                                        INNER JOIN sys.columns AS c ON t.object_id = c.object_id
                                        LEFT  JOIN sys.identity_columns AS ic on c.object_id = ic.object_id AND c.column_id = ic.column_id";

                using (var cmd = new System.Data.SqlClient.SqlCommand(columnsSql, con))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var tableName = rdr["TableName"].ToString();
                            var table = tables.FirstOrDefault(i => i.TableName == tableName);
                            if (table != null)
                            {
                                table.Fields.Add(new Column()
                                {
                                    Name = rdr["ColumnName"].ToString(),
                                    SqlType = rdr["DataType"].ToString(),
                                    IsKeyField = Convert.ToBoolean(rdr["IsIdentity"])
                                });
                            }
                        }
                    }
                }

                var relSql = @"
                            SELECT
                                ParentTableSchema = FK.Table_Schema,
                                ParentTable =  FK.TABLE_NAME,
                                ParentColumn = CU.COLUMN_NAME,
                                ChildTableSchema =  PK.Table_Schema ,
                                ChildTable = PK.TABLE_NAME,
                                ChildColumn = PT.COLUMN_NAME,
                                RelationShipName = C.CONSTRAINT_NAME
                            FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
                                INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
                                INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
                                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
                                INNER JOIN (
                                SELECT i1.TABLE_NAME, i2.COLUMN_NAME
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
                                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
                            WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
                            ) PT ON PT.TABLE_NAME = PK.TABLE_NAME
";

                result.RelationShips = new List<TableRelationShip>();
                using (var cmd = new System.Data.SqlClient.SqlCommand(relSql, con))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var parentTableName = rdr["ParentTable"].ToString();
                            var childTableName = rdr["ChildTable"].ToString();
                            var parentTable = tables.FirstOrDefault(i => i.TableName == parentTableName);
                            var childTable = tables.FirstOrDefault(i => i.TableName == childTableName);
                            if (parentTable != null)
                            {
                                parentTable.Fields.FirstOrDefault(i => i.Name == rdr["ParentColumn"].ToString()).IsKeyField = true;
                                childTable.Fields.FirstOrDefault(i => i.Name == rdr["ChildColumn"].ToString()).IsKeyField = true;

                                var relationShip = new TableRelationShip()
                                {
                                    ChildTableSchema = rdr["ChildTableSchema"].ToString(),
                                    ChildKeyField = rdr["ChildColumn"].ToString(),
                                    ChildTable = rdr["ChildTable"].ToString(),
                                    FieldName = rdr["RelationShipName"].ToString(),
                                    IsList = true,
                                    ParentTableSchema = rdr["ParentTableSchema"].ToString(),
                                    ParentKeyField = rdr["ParentColumn"].ToString(),
                                    ParentTable = rdr["ParentTable"].ToString()
                                };
                                parentTable.Relationships.Add(relationShip);
                                relationShip.SetChildMetaData(childTable);
                                result.RelationShips.Add(relationShip);

                                // AddReverse
                                var reverseRelationShip = new TableRelationShip()
                                {
                                    ParentTableSchema = rdr["ChildTableSchema"].ToString(),
                                    ParentKeyField = rdr["ChildColumn"].ToString(),
                                    ParentTable = rdr["ChildTable"].ToString(),
                                    FieldName = rdr["RelationShipName"].ToString() + "_Reverse",
                                    IsList = true,
                                    ChildTableSchema = rdr["ParentTableSchema"].ToString(),
                                    ChildKeyField = rdr["ParentColumn"].ToString(),
                                    ChildTable = rdr["ParentTable"].ToString()
                                };
                                reverseRelationShip.SetChildMetaData(parentTable);
                                childTable.Relationships.Add(reverseRelationShip);
                                result.RelationShips.Add(reverseRelationShip);
                            }
                        }
                    }
                }
            }
            
            result.Tables = tables;
            return result;
        }
    }
}
