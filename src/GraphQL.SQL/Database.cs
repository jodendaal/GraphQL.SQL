using GraphQL.SQL.Builder;
using GraphQL.SQL.MetaData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.SQL
{
    public class Database : IDatabase
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ILookup<string, DataRow>> Query(CancellationToken cancellationToken, GraphQLTableMetaData metaData, IResolveFieldContext context)
        {
            var sqlCommand = GenerateSqlCommand(context,metaData);
            var dt = await Execute(sqlCommand);
            return dt.Rows.Cast<DataRow>().ToLookup(i => i[metaData.IdentityColumn].ToString(), o => o);
        }

        public async Task<IDictionary<string, IGrouping<string, DataRow>>> GetRowsById(IEnumerable<string> ids, CancellationToken cancellationToken, GraphQLTableMetaData metaData, IResolveFieldContext context)
        {
            var sqlCommand = GenerateSqlCommand(context, metaData, ids);

            var dt = await Execute(sqlCommand);


            var lookupKey = metaData.IdentityColumn;
            
            //Relationships
            if (context.FieldDefinition.Metadata.ContainsKey("relationshipNameParentKey"))
            {
                lookupKey = metaData.Fields.FirstOrDefault(i => i.Name == context.FieldDefinition.Metadata["relationshipNameParentKey"].ToString()).Name;
            }

            var t = dt.Rows.Cast<DataRow>().ToLookup(i => i[lookupKey].ToString(), o => o);

            var result = new Dictionary<string, IGrouping<string, DataRow>>();

            foreach (var item in t)
            {
                result.Add(item.Key, item);
            }
            return result;
        }

        private async Task<DataTable> Execute(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            using (var connection = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                cmd.Connection = connection;

                Debug.WriteLine(cmd.CommandText);
                Debug.WriteLine(string.Join(",", cmd.Parameters.Cast<SqlParameter>().Select(i => $"{i.ParameterName}={i.Value}")));

                using (System.Data.SqlClient.SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        private SqlCommand GenerateSqlCommand(IResolveFieldContext context, GraphQLTableMetaData metaData, IEnumerable<string> ids = null)
        {
            var paging = GetPaging(context);
            var requestedFields = context.SubFields.Select(i => i.Value.Name).ToList();
            var fields = metaData.GetFields(requestedFields);
            var pageInfo = paging;

            var tableName = metaData.FQN();
            var tableAlias = $"[{metaData.TableAlias}]";
            var query = new SelectQueryBuilder(tableName, tableAlias);

            // Fields
            foreach (var field in fields)
            {
                query.Field($"{tableAlias}.[{field.Name}]", $"[{field.NameAs}]");
            }

            // Paging
            if (pageInfo != null && pageInfo.Page > 0)
            {
                query.Page(
                    query.AddParam(pageInfo.Page, "_PageNumber", "int"),
                    query.AddParam(pageInfo.PageSize, "_PageSize", "int"),
                    $"{tableAlias}.[{metaData.OrderByDefault}]");
            }

            // Where
            AddFilterConditions(context, metaData, query,ids);

            return query.ToCommand();
        }

        private Paging GetPaging(IResolveFieldContext context)
        {
            if (context.Arguments == null)
            {
                return null;
            }

            Paging paging = null;
            if (context.Arguments.ContainsKey("page") || context.Arguments.ContainsKey("pageSize"))
            {
                paging = new Paging() { Page = 1 };
                if (context.Arguments.ContainsKey("page"))
                {
                    paging.Page = context.Arguments["page"].Value == null ? 0 : (int)context.Arguments["page"].Value;
                }

                if (context.Arguments.ContainsKey("pageSize"))
                {
                    paging.PageSize = context.Arguments["pageSize"].Value == null ? 0 : (int)context.Arguments["pageSize"].Value;
                }
            }

            return paging;
        }

        private void AddFilterSetConditions(Dictionary<string, object> values, SelectConditionSet set, Table tableMetaData, SelectQueryBuilder sqlBuilder)
        {
            foreach (var filterkey in values)
            {
                var val = filterkey.Value as Dictionary<string, object>;
                switch (filterkey.Key)
                {
                    case "and":
                        set.And.AddRange(GetFilterSetConditions(val, tableMetaData, sqlBuilder));
                        break;
                    case "or":
                        set.Or.AddRange(GetFilterSetConditions(val, tableMetaData, sqlBuilder));

                        break;
                    case "andSet":
                        set.AndSet = new SelectConditionSet(SetOperator.And);

                        AddFilterSetConditions(val, set.AndSet, tableMetaData, sqlBuilder);
                        break;
                    case "orSet":

                        set.OrSet = new SelectConditionSet(SetOperator.Or);
                        AddFilterSetConditions(val, set.OrSet, tableMetaData, sqlBuilder);
                        break;
                }
            }
        }

        private string ToCSV(object value,string valueType,SelectQueryBuilder builder)
        {
            switch (valueType)
            {
                case "int":
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                case "bigint":
                case "tinyint":
                case "smallint":
                    if (value is IEnumerable<int>)
                    {
                        return $"({string.Join(",", (value as IEnumerable<int>))})";
                    }

                    var valuesInt = value as object[];
                    return $"({string.Join(",", valuesInt)})";
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "char":
                case "uniqueidentifier":

                    if (value is IEnumerable<string>)
                    {
                        return $"({string.Join(",", (value as IEnumerable<string>).Select(i => $"'{i}'"))})";
                    }

                    var valuesTring = value as object[];
                    return $"({string.Join(",", valuesTring.Select(i => $"'{i}'"))})";
                    //return $"({ToCSV(valuesTring)}";
                default:

                    var values = value as object[];
                    return $"({string.Join(",", values.Select(i=> builder.AddParam(i)))})";
            }
        }

        private List<SelectCondition> GetFilterSetConditions(Dictionary<string, object> values, Table tableMetaData, SelectQueryBuilder sqlBuilder)
        {
            var result = new List<SelectCondition>();
            foreach (var v in values)
            {
                var fieldParts = v.Key.Split('_');
                var fieldName = v.Key;

                if (fieldParts.Count() > 1)
                {
                    var operatorVal = fieldParts[fieldParts.Count() - 1];
                    fieldName = fieldName.Remove(fieldName.LastIndexOf($"_{operatorVal}"), $"_{operatorVal}".Length);
                }

                var tableField = tableMetaData.Fields.FirstOrDefault(i => i.NameAs.ToLowerInvariant() == fieldName.ToLowerInvariant());
                if (tableField != null)
                {
                    var @operator = GetFieldOperator(v.Key);
                    switch (@operator)
                    {
                        case ColumnOperator.IN:
                        case ColumnOperator.NOT_IN:
                            result.Add(new SelectCondition($"{sqlBuilder.TableAlias}.[{tableField.Name}]", GetFieldOperator(v.Key), ToCSV(v.Value, tableField.SqlType, sqlBuilder)));
                            break;
                        default:
                            result.Add(new SelectCondition($"{sqlBuilder.TableAlias}.[{tableField.Name}]", GetFieldOperator(v.Key), sqlBuilder.AddParam(v.Value, tableField.Name)));
                            break;
                    }

                    
                }
            }

            return result;
        }

        private void AddFilterConditions(IResolveFieldContext context, GraphQLTableMetaData tableMetaData, SelectQueryBuilder sqlBuilder, IEnumerable<string> ids)
        {
            int setNo = 1;
            if (context.Arguments != null)
            {
                foreach (var field in context.Arguments)
                {
                    if (field.Key == "page" || field.Key == "pageSize")
                    {
                        continue;
                    }

                    var fieldT = field.Value.Value as Dictionary<string, object>;

                    if (fieldT != null)
                    {
                        sqlBuilder.ConditionSet(setNo, SetOperator.And, (a) =>
                        {
                            AddFilterSetConditions(fieldT, a, tableMetaData, sqlBuilder);
                            setNo++;
                        });
                    }
                    else if (field.Value.Value != null)
                    {
                        var tableField = tableMetaData.Fields.FirstOrDefault(i => i.NameAs.ToLowerInvariant() == field.Key.ToLowerInvariant());
                        if (tableField != null)
                        {
                            var @operator = GetFieldOperator(field.Key);
                            switch (@operator)
                            {
                                case ColumnOperator.IN:
                                case ColumnOperator.NOT_IN:
                                    sqlBuilder.Condition($"{sqlBuilder.TableAlias}.[{tableField.Name}]", @operator, ToCSV(field.Value.Value, tableField.SqlType, sqlBuilder));
                                    break;
                                default:
                                    sqlBuilder.Condition($"{sqlBuilder.TableAlias}.[{tableField.Name}]", @operator, sqlBuilder.AddParam(field.Value.Value, tableField.Name));
                                    break;
                            }
                        }
                    }
                }
            }


            if (ids != null && ids.Count() > 0)
            {
                var parentKey = context.FieldDefinition.Metadata["relationshipNameParentKey"].ToString();

                sqlBuilder.ConditionSet(setNo + 1, SetOperator.And, (a) =>
                {
                    var tableField = tableMetaData.Fields.FirstOrDefault(i => i.Name == tableMetaData.IdentityColumn);
                    
                    //Relationships
                    if (context.FieldDefinition.Metadata.ContainsKey("relationshipNameParentKey"))
                    {
                        tableField = tableMetaData.Fields.FirstOrDefault(i => i.Name == context.FieldDefinition.Metadata["relationshipNameParentKey"].ToString());
                    }

                    if (tableField != null)
                    {
                        a.AndCondition($"{sqlBuilder.TableAlias}.[{tableField.Name}]", ColumnOperator.IN, ToCSV(ids, tableField.SqlType, sqlBuilder));
                    }
                });
            }
            //else
            //{
            //    // RelationShips - This could be removed 
            //    var tableRelationShip = tableMetaData.Relationships.FirstOrDefault(i => i.FieldName.Replace("_Reverse", "").ToLowerInvariant() == context.FieldDefinition.Name.ToLowerInvariant());
            //    if (tableRelationShip != null)
            //    {
            //        var t = context.Source as IGrouping<string, DataRow>;
            //        var parentRow = t.FirstOrDefault();

            //        if (parentRow != null &&
            //            parentRow.Table.Columns.Contains(tableRelationShip.ParentKeyField) &&
            //            parentRow[tableRelationShip.ParentKeyField] != DBNull.Value)
            //        {
            //            sqlBuilder.ConditionSet(setNo, SetOperator.And, (a) =>
            //            {
            //                a.AndCondition(tableRelationShip.ChildKeyField, ColumnOperator.Equals, sqlBuilder.AddParam(parentRow[tableRelationShip.ParentKeyField], tableRelationShip.ChildKeyField));
            //                setNo++;
            //            });
            //        }
            //        else if (parentRow != null &&
            //                parentRow.Table.Columns.Contains($"{tableRelationShip.FieldName}_{tableRelationShip.ParentKeyField}"))
            //        {
            //            sqlBuilder.ConditionSet(setNo, SetOperator.And, (a) =>
            //            {
            //                a.AndCondition(tableRelationShip.ChildKeyField, ColumnOperator.Equals, sqlBuilder.AddParam(parentRow[$"{tableRelationShip.FieldName}_{tableRelationShip.ParentKeyField}"], tableRelationShip.ChildKeyField));
            //                setNo++;
            //            });
            //        }
            //        else
            //        {
            //            throw new Exception($"Expected parent row to contain {tableRelationShip.ParentKeyField} for {tableRelationShip.ParentTable} to {tableRelationShip.ParentTable} relationship but was not found.");
            //        }
            //    }
            //}
        }

        private string GetFieldOperator(string fieldName)
        {
            var values = fieldName.Split('_');
            if (values.Count() > 1)
            {
                var operatorVal = values[values.Count() - 1];

                switch (operatorVal)
                {
                    case "gt":
                        return ColumnOperator.GreaterThan;
                    case "gte":
                        return ColumnOperator.GreaterThanOrEqualTo;
                    case "lt":
                        return ColumnOperator.LessThan;
                    case "lte":
                        return ColumnOperator.LessThanOrEqualTo;
                    case "ne":
                        return ColumnOperator.NotEquals;
                    case "in":
                        return ColumnOperator.IN;
                    case "ni":
                        return ColumnOperator.NOT_IN;
                }
            }

            return ColumnOperator.Equals;
        }
    }
}
