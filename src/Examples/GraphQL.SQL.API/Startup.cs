using GraphQL.DataLoader;
using GraphQL.SQL.Extensions;
using GraphQL.SQL.MetaData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.SQL.API
{

    public class Startup
    {
        private string _connectionString = @"Data Source=(localdb)\GraphQLTest;Initial Catalog=NorthWind;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //private string _connectionString = @"Data Source=(localdb)\GraphQLTest;Initial Catalog=GraphQL;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.UseGraphQL(_connectionString);

            //Manual Config Example
            //services.AddSingleton<ISqlSchemaToMetaData, SqlSchemaToMetaData>((a) => {
            //   return new SqlSchemaToMetaData(_connectionString);
            //});

            //services.AddSingleton<IDatabase, Database>((a) => {
            //    return new Database(_connectionString);
            //});

            //services.AddSingleton<IMetaDataProvider>((a)=> {
            //    if (File.Exists("database-schema.json"))
            //    {
            //        return new JsonMetaDataProvider("database-schema.json");
            //    }

            //    return new AutoGenerateMetaDataProvider(a.GetRequiredService<ISqlSchemaToMetaData>());
            //});
            //services.AddSingleton<SqlQuery>();
            //services.AddSingleton<SqlSchema>();

            //services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            //services.AddSingleton<DataLoaderDocumentListener>();


            //services.AddGraphQLSQL(_connectionString);
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
