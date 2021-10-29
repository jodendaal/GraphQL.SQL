using LocalDb;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests
{
    [TestClass]
    public class DatabaseConnectionTest : BaseDatabaseTest
    {
        [TestMethod]
        public async Task TestConnection1()
        {
           var db = await GetDatabase("1");
           using(var cmd = new SqlCommand("SELECT COUNT(*) FROM DBO.ORDERS", db.Connection))
            {
                var result =(int)cmd.ExecuteScalar();
                Assert.AreEqual(830, result);
            }
        }

        [TestMethod]
        public async Task TestConnection2()
        {
            var db = await GetDatabase("2");
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM DBO.ORDERS", db.Connection))
            {
                var result = (int)cmd.ExecuteScalar();
                Assert.AreEqual(830, result);
            }
        }

        [TestMethod]
        public async Task TestConnection3()
        {
            var db = await GetDatabase("3");
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM DBO.ORDERS", db.Connection))
            {
                var result = (int)cmd.ExecuteScalar();
                Assert.AreEqual(830, result);
            }
        }
    }
}
