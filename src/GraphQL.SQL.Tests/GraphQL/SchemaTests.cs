using GraphQL.DataLoader;
using GraphQL.NewtonsoftJson;
using GraphQL.SQL.Tests.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests.GraphQL
{
    [TestClass]
    public class SchemaTests : BaseGraphQLTest
    {
        [TestMethod]
        public async Task GenerateSchemaFromDatabase()
        {
            var schema = new SqlSchema(await GetContainer());

            Assert.IsNotNull(schema);
            Assert.IsNotNull(schema.Query);
            Assert.IsTrue(schema.Query.Fields.Count > 0);
            Assert.AreEqual(schema.Query.Fields.Count, 13);
        }

        [TestMethod]
        public async Task SchemaQuery()
        {
            var query = "{orders {  orderID }}";
            var container = await GetContainer();
            var schema = new SqlSchema(container);

            var json = await schema.ExecuteAsync(_ =>
            {
                _.Query = query;
                _.RequestServices = container;
            });

            var jObject = JObject.Parse(json);
            var orders = jObject["data"]["orders"];

            Debug.WriteLine(json);

            Assert.IsNotNull(orders);
            Assert.AreEqual(orders.Children().Count(), 830);
            Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
            Assert.AreEqual(orders.First()["orderID"].ToString(), "10248", "column returned incorrect value");
        }

        [TestMethod]
        public async Task SchemaQueryRelationShips()
        {
            var query = "{orders {  orderID fK_Orders_Customers { companyName } } }";
            var container = await GetContainer();
            var schema = new SqlSchema(container);

            var json = await schema.ExecuteAsync(_ =>
            {
                _.UnhandledExceptionDelegate = (e) => {

                    Debug.WriteLine(e.ErrorMessage);
                };
                _.Query = query;
                _.RequestServices = container;
                _.Listeners.Add(container.GetService(typeof(DataLoaderDocumentListener)) as DataLoaderDocumentListener);
            });

            var jObject = JObject.Parse(json);
            var orders = jObject["data"]["orders"];

            Debug.WriteLine(json);

            Assert.IsNotNull(orders);
            Assert.AreEqual(orders.Children().Count(), 830);
            Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
            Assert.AreEqual(orders.First()["orderID"].ToString(), "10248", "column returned incorrect value");
            Assert.IsNotNull(orders.First()["fK_Orders_Customers"], "relationship column returned null value");
            Assert.AreEqual(orders.First()["fK_Orders_Customers"].First()["companyName"].ToString(), "Vins et alcools Chevalier", "relationship column returned incorrect value");
        }
    }
}
