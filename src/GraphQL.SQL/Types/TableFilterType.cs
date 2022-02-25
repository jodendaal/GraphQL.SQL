using GraphQL.SQL.Builder;
using GraphQL.SQL.MetaData;
using GraphQL.Types;
using System;
using System.Collections.Generic;

namespace GraphQL.SQL.Types
{
    public class TableFilterType : InputObjectGraphType
    {
        public TableFilterType(GraphQLTableMetaData table)
        {
            Name = $"{table.NameAs}Filter";

            table.Fields.ForEach(field =>
            {
                switch (field.SqlType)
                {
                    case "varbinary":
                    case "binary":
                    case "timestamp":
                    case "image":
                        AddFilter(typeof(ByteGraphType), field.NameAs, new List<string>()
                        {
                              ColumnOperator.Equals
                        });
                        break;
                    case "date":
                        AddFilter(typeof(DateGraphType), field.NameAs, new List<string>()
                        {
                              ColumnOperator.Equals,
                              ColumnOperator.NotEquals,
                              ColumnOperator.GreaterThan,
                              ColumnOperator.GreaterThanOrEqualTo,
                              ColumnOperator.LessThan,
                              ColumnOperator.LessThanOrEqualTo
                        });
                        break;
                    case "time":
                    case "datetime":
                    case "datetime2":
                    case "smalldatetime":
                        AddFilter(typeof(DateTimeGraphType), field.NameAs, new List<string>()
                        {
                              ColumnOperator.Equals,
                              ColumnOperator.NotEquals,
                              ColumnOperator.GreaterThan,
                              ColumnOperator.GreaterThanOrEqualTo,
                              ColumnOperator.LessThan,
                              ColumnOperator.LessThanOrEqualTo
                        });
                        break;
                    case "datetimeoffset":
                        AddFilter(typeof(DateTimeOffsetGraphType), field.NameAs, new List<string>()
                        {
                              ColumnOperator.Equals,
                              ColumnOperator.NotEquals,
                              ColumnOperator.GreaterThan,
                              ColumnOperator.GreaterThanOrEqualTo,
                              ColumnOperator.LessThan,
                              ColumnOperator.LessThanOrEqualTo
                        });
                        break;
                    case "bit":
                        AddField(new FieldType()
                        {
                            Name = field.NameAs,
                            Type = typeof(BooleanGraphType)
                        });
                        break;
                    case "uniqueidentifier":
                        AddField(new FieldType()
                        {
                            Name = field.NameAs,
                            Type = typeof(GuidGraphType)
                        });

                        AddField(new FieldType()
                        {
                            Name = $"{field.NameAs}_ne",
                            Type = typeof(StringGraphType)
                        });

                        break;
                    case "varchar":
                    case "nchar":
                    case "nvarchar":
                    case "text":
                    case "ntext":
                    case "char":

                        AddFilter(typeof(StringGraphType), field.NameAs, new List<string>()
                        {
                              ColumnOperator.Equals,
                              ColumnOperator.NotEquals
                        });

                        // AddField(new FieldType()
                        // {
                        //    Name = $"{field.GeneratedName}_contains",
                        //    Type = typeof(StringGraphType)
                        // });

                        // AddField(new FieldType()
                        // {
                        //    Name = $"{field.GeneratedName}_not_contains",
                        //    Type = typeof(StringGraphType)
                        // });

                        // AddField(new FieldType()
                        // {
                        //    Name = $"{field.GeneratedName}_in",
                        //    Type = typeof(StringGraphType)
                        // });

                        // AddField(new FieldType()
                        // {
                        //    Name = $"{field.GeneratedName}_not_in",
                        //    Type = typeof(StringGraphType)
                        // });
                        break;
                    case "float":

                        AddFilter(typeof(FloatGraphType), field.NameAs, new List<string>()
                        {
                              ColumnOperator.Equals,
                              ColumnOperator.NotEquals,
                              ColumnOperator.GreaterThan,
                              ColumnOperator.GreaterThanOrEqualTo,
                              ColumnOperator.LessThan,
                              ColumnOperator.LessThanOrEqualTo
                        });
                        break;
                    case "decimal":
                    case "numeric":
                    case "money":
                    case "smallmoney":

                        AddFilter(typeof(DecimalGraphType), field.NameAs, new List<string>()
                        {
                              ColumnOperator.Equals,
                              ColumnOperator.NotEquals,
                              ColumnOperator.GreaterThan,
                              ColumnOperator.GreaterThanOrEqualTo,
                              ColumnOperator.LessThan,
                              ColumnOperator.LessThanOrEqualTo
                        });

                        // AddField(new FieldType()
                        // {
                        //    Name = $"{field.GeneratedName}_in",
                        //    Type = typeof(StringGraphType)
                        // });

                        // AddField(new FieldType()
                        // {
                        //    Name = $"{field.GeneratedName}_not_in",
                        //    Type = typeof(StringGraphType)
                        // });
                        break;
                    case "bigint":
                        AddFilter(typeof(BigIntGraphType), field.NameAs, new List<string>()
                        {
                              ColumnOperator.Equals,
                              ColumnOperator.NotEquals,
                              ColumnOperator.GreaterThan,
                              ColumnOperator.GreaterThanOrEqualTo,
                              ColumnOperator.LessThan,
                              ColumnOperator.LessThanOrEqualTo
                        });
                        break;
                    case "int":
                    case "tinyint":
                    case "smallint":
                        AddFilter(typeof(IntGraphType), field.NameAs, new List<string>()
                        {
                              ColumnOperator.Equals,
                              ColumnOperator.NotEquals,
                              ColumnOperator.GreaterThan,
                              ColumnOperator.GreaterThanOrEqualTo,
                              ColumnOperator.LessThan,
                              ColumnOperator.LessThanOrEqualTo
                        });

                        AddFilter(typeof(ListGraphType<IntGraphType>), field.NameAs, new List<string>()
                        {
                              ColumnOperator.IN,
                              ColumnOperator.NOT_IN
                        });

                        //AddField(new FieldType()
                        //{
                        //    Name = $"{field.GeneratedName}_in",
                        //    Type = typeof(StringGraphType)
                        //});

                        // AddField(new FieldType()
                        // {
                        //    Name = $"{field.GeneratedName}_not_in",
                        //    Type = typeof(StringGraphType)
                        // });
                        break;
                    default:
                        AddFilter(typeof(StringGraphType), field.NameAs, new List<string>()
                        {
                              ColumnOperator.Equals
                        });
                        break;
                }
            });
        }

        private void AddFilter(Type graphType, string fieldName, List<string> operators)
        {
            foreach (var op in operators)
            {
                if (op == ColumnOperator.Equals)
                {
                    AddField(new FieldType()
                    {
                        Name = fieldName,
                        Type = graphType
                    });
                }
                else if (op == ColumnOperator.NotEquals)
                {
                    AddField(new FieldType()
                    {
                        Name = $"{fieldName}_ne",
                        Type = graphType
                    });
                }
                else if (op == ColumnOperator.GreaterThan)
                {
                    AddField(new FieldType()
                    {
                        Name = $"{fieldName}_gt",
                        Type = graphType
                    });
                }
                else if (op == ColumnOperator.GreaterThanOrEqualTo)
                {
                    AddField(new FieldType()
                    {
                        Name = $"{fieldName}_gte",
                        Type = graphType
                    });
                }
                else if (op == ColumnOperator.LessThan)
                {
                    AddField(new FieldType()
                    {
                        Name = $"{fieldName}_lt",
                        Type = graphType
                    });
                }
                else if (op == ColumnOperator.LessThanOrEqualTo)
                {
                    AddField(new FieldType()
                    {
                        Name = $"{fieldName}_lte",
                        Type = graphType
                    });
                }
                else if (op == ColumnOperator.IN)
                {
                    AddField(new FieldType()
                    {
                        Name = $"{fieldName}_in",
                        Type =   graphType,
                        
                    });
                }
                else if (op == ColumnOperator.NOT_IN)
                {
                    AddField(new FieldType()
                    {
                        Name = $"{fieldName}_ni",
                        Type = graphType,

                    });
                }
            }
        }


    }
}
