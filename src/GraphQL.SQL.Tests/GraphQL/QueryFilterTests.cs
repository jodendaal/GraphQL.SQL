using GraphQL.DataLoader;
using GraphQL.NewtonsoftJson;
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
    public class QueryFilterTests : BaseGraphQLTest
    {
        [TestMethod]
        public async Task IN()
        {
            var query = "{orders(filter: { and: { orderID_in: [10248, 10251, 10254] } }) {  orderID }}";
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
            Assert.AreEqual(3, orders.Children().Count());
            Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
            Assert.AreEqual("10248", orders.First()["orderID"].ToString(), "column returned incorrect value");
            Assert.AreEqual("10251", orders[1]["orderID"].ToString(), "column returned incorrect value");
            Assert.AreEqual("10254", orders[2]["orderID"].ToString(), "column returned incorrect value");
        }

        [TestMethod]
        public async Task NOT_IN()
        {
            var query = "{orders(page:1,pageSize:10,filter: { and: { orderID_ni: [10248, 10251, 10254] } }) {  orderID }}";
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
            Assert.AreEqual(10, orders.Children().Count());

            foreach(var child in orders.Children())
            {
                Assert.AreNotEqual("10248", child["orderID"].ToString(), "row returned incorrect value");
                Assert.AreNotEqual("10251", child["orderID"].ToString(), "row returned incorrect value");
                Assert.AreNotEqual("10254", child["orderID"].ToString(), "row returned incorrect value");
            }


            Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
            Assert.AreEqual("10249", orders.First()["orderID"].ToString(), "column returned incorrect value");
        }
    }
}
