using LocalDb;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests
{
    [TestClass]
    public class DatabaseConnectionTest : BaseDatabaseTest
    {

        [TestMethod]
        public void TestConnection()
        {
           using(var cmd = new SqlCommand("SELECT COUNT(*) FROM DBO.ORDERS",Database.Connection))
            {
                var result =(int)cmd.ExecuteScalar();
                Assert.AreEqual(830, result);
            }
        }
    }
}
