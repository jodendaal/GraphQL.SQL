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
    public class QueryPerformanceTests : BaseGraphQLTest
    {
        [TestMethod]
        public async Task Performance()
        {
            var query = "{orders(filter: { and: { orderID_in: [10248, 10251, 10254] } }) {  orderID }}";
            var container = await GetContainer();
            var schema = new SqlSchema(container);

            Stopwatch globalTime = new();
            Stopwatch iterrationTime = new();

            globalTime.Start();
            var count = 10;
            for (int i = 0; i <= 10; i++)
            {
                iterrationTime.Reset();
                iterrationTime.Start();
                var json = await schema.ExecuteAsync(_ =>
                {
                    _.Query = query;
                    _.RequestServices = container;
                });
                iterrationTime.Stop();

                Debug.WriteLine($"({i}) Took {iterrationTime.Elapsed:ss} s, {iterrationTime.Elapsed:fff}ms  to execute query");
                
                var jObject = JObject.Parse(json);
                var orders = jObject["data"]["orders"];

                Debug.WriteLine(json);

                if(i != 0)
                {
                    if(iterrationTime.ElapsedMilliseconds > 6)
                    {
                        Assert.Fail("Query performance has decreased please check change");
                    }
                }

                Assert.IsNotNull(orders);
                Assert.AreEqual(3, orders.Children().Count());
                Assert.IsNotNull(orders.First()["orderID"], "column returned null value");
                Assert.AreEqual("10248", orders.First()["orderID"].ToString(), "column returned incorrect value");
                Assert.AreEqual("10251", orders[1]["orderID"].ToString(), "column returned incorrect value");
                Assert.AreEqual("10254", orders[2]["orderID"].ToString(), "column returned incorrect value");
            }


            globalTime.Stop();

            Debug.WriteLine($"Total Time {globalTime.Elapsed:ss} s, {globalTime.Elapsed:fff}ms  to execute {count} queries");
        }
    }
}
