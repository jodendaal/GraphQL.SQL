using GraphQL.DataLoader;
using GraphQL.SQL.MetaData;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.SQL.Extensions
{
    public static class RegistrationExtensions
    {
        public static string UseGraphQL(this IServiceCollection services,string connectionString)
        {
            services.AddSingleton<ISqlSchemaToMetaData, SqlSchemaToMetaData>((a) => {
                return new SqlSchemaToMetaData(connectionString);
            });

            services.AddSingleton<IDatabase, Database>((a) => {
                return new Database(connectionString);
            });

            services.AddSingleton<IMetaDataProvider>((a) => {
                if (File.Exists("database-schema.json"))
                {
                    return new JsonMetaDataProvider("database-schema.json");
                }

                return new AutoGenerateMetaDataProvider(a.GetRequiredService<ISqlSchemaToMetaData>());
            });
            services.AddSingleton<SqlQuery>();
            services.AddSingleton<SqlSchema>();

            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<DataLoaderDocumentListener>();

            return "";
        }
    }
}
