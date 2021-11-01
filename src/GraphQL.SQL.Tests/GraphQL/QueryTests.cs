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
    public class QueryTests : BaseGraphQLTest
    {
        [TestMethod]
        public async Task Select_ById()
        {
            var query = "{orders(orderID:10248) {  orderID }}";
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
            Assert.AreEqual(1,orders.Children().Count());
            Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
            Assert.AreEqual("10248",orders.First()["orderID"].ToString() , "column returned incorrect value");
        }

        [TestMethod]
        public async Task Select_Paging()
        {
            var query = "{orders(page:1,pageSize:10) {  orderID }}";
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
            Assert.AreEqual(10,orders.Children().Count());
            Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
            Assert.AreEqual("10248",orders.First()["orderID"].ToString(), "column returned incorrect value");

            query = "{orders(page:2,pageSize:10) {  orderID }}";
            json = await schema.ExecuteAsync(_ =>
            {
                _.Query = query;
                _.RequestServices = container;
            });

            var jObjectPage2 = JObject.Parse(json);
            var ordersPage2 = jObjectPage2["data"]["orders"];

            Debug.WriteLine(json);

            Assert.IsNotNull(ordersPage2);
            Assert.AreEqual(10,ordersPage2.Children().Count());
            Assert.IsNotNull(ordersPage2.First()["orderID"], "column returned null value");
            Assert.AreEqual("10258",ordersPage2.First()["orderID"].ToString(), "column returned incorrect value");
        }


        [TestMethod]
        public async Task Select_Filter_ById()
        {
            var query = "{orders(filter: { and: { orderID: 10248 } }) {  orderID }}";
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
            Assert.AreEqual(orders.Children().Count(), 1);
            Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
            Assert.AreEqual(orders.First()["orderID"].ToString(), "10248", "column returned incorrect value");
        }

        [TestMethod]
        public async Task Select_Filter_Or()
        {
            var query = @"{ orders(filter: { or: { orderID: 10248, shipName: ""Hanari Carnes"" } }) { orderID shipName } }";
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
            Assert.AreEqual(15, orders.Children().Count());
            Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
            Assert.AreEqual("10248", orders.First()["orderID"].ToString(), "column returned incorrect value");
            Assert.AreEqual("10250", orders[1]["orderID"].ToString(), "column returned incorrect value");
            Assert.AreEqual("Hanari Carnes", orders[1]["shipName"].ToString(), "column returned incorrect value");
        }

        [TestMethod]
        public async Task Select_Filter_And()
        {
            var query = @"{ orders(filter: { and: { orderID_gt: 10248, shipName: ""Hanari Carnes"",freight_lte:68.65 } }) { orderID shipName freight } }";
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
            Assert.AreEqual(12, orders.Children().Count());
            Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
            Assert.AreEqual("10250", orders.First()["orderID"].ToString(), "column returned incorrect value");
            Assert.AreEqual("10253", orders[1]["orderID"].ToString(), "column returned incorrect value");
            Assert.AreEqual("Hanari Carnes", orders[1]["shipName"].ToString(), "column returned incorrect value");
            Assert.AreEqual("65.83", orders[0]["freight"].ToString(), "column returned incorrect value");
            Assert.AreEqual("58.17", orders[1]["freight"].ToString(), "column returned incorrect value");
        }

        [TestMethod]
        public async Task Select_Filter_Or_And()
        {
            var query = @"{ orders(filter: { or: { orderID: 10248, shipName: ""Hanari Carnes"" },and:{freight_gt:68.65} }) { orderID shipName freight } }";
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
            Assert.AreEqual(2,orders.Children().Count());
            Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
            Assert.AreEqual("10783", orders.First()["orderID"].ToString(), "column returned incorrect value");
            Assert.AreEqual("10981", orders[1]["orderID"].ToString(), "column returned incorrect value");
            Assert.AreEqual("124.98", orders[0]["freight"].ToString(), "column returned incorrect value");
            Assert.AreEqual("193.37", orders[1]["freight"].ToString(), "column returned incorrect value");
        }
    }
}
