using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using NLog;
using Telerik.JustMock;
using Xunit;

namespace DBTest
{
    public class DBManagerTest
    {
        const string goodProvider = "System.Data.SqlClient";
        const string badProvider = "SqlServer";
        // const string goodConnection = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True";
        const string goodConnection = @"Data Source = psicorp.the-taylor-family.org; Initial Catalog = master; Integrated Security = True";
        const string badConnection = "foo";

        [Fact]
        public void CTOR_Correct_NotNull()
        {
            var log = Mock.Create<ILogger>();
            var dbManager = new DBManager(log, goodProvider, goodConnection);
            Assert.NotNull(dbManager);
        }

        [Fact]
        public void CTOR_NullLog_ArgumentNullExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() => new DBManager(null, goodProvider, goodConnection));
        }

        [Fact]
        public void CTOR_NullProvider_ArgumentNullExceptionThrown()
        {
            var log = Mock.Create<ILogger>();
            Assert.Throws<ArgumentNullException>(() => new DBManager(log,string.Empty , goodConnection));
        }

        [Fact]
        public void CTOR_NullConnectionString_ArgumentNullExceptionThrown()
        {
            var log = Mock.Create<ILogger>();
            Assert.Throws<ArgumentNullException>(() => new DBManager(log,goodProvider, string.Empty));
        }

        [Fact]
        public void ExecuteSelectStatementScalar_Correct_Equals1()
        {
            var log = Mock.Create<ILogger>();
            var dbManager = new DBManager(log, goodProvider,goodConnection);
            Assert.NotNull(dbManager);

            var result = dbManager.ExecuteSelectStatementScalar("SELECT 1");
            Assert.NotNull(result);
            Assert.Equal(1,(int)result);
        }

        [Fact]
        public void ExecuteSelectStatementScalar_InvalidSelectStatement_ExceptionThrown()
        {
            var log = Mock.Create<ILogger>();
            var dbManager = new DBManager(log, goodProvider,goodConnection);
            Assert.NotNull(dbManager);

            Assert.Throws<ApplicationException>(()=>dbManager.ExecuteSelectStatementScalar("Foo"));
        }

        [Fact]
        public async Task ExecuteSelectStatementScalarAsync_Correct_Equals1()
        {
            var log = Mock.Create<ILogger>();
            var dbManager = new DBManager(log, goodProvider,goodConnection);
            Assert.NotNull(dbManager);

            var result = await dbManager.ExecuteSelectStatementScalarAsync("SELECT 1");
            Assert.NotNull(result);
            Assert.Equal(1,(int)result);
        }

        [Fact]
        public void ExecuteSelectStatementScalarAsync_InvalidSelectStatement_ExceptionThrown()
        {
            var log = Mock.Create<ILogger>();
            var dbManager = new DBManager(log, goodProvider,goodConnection);
            Assert.NotNull(dbManager);

            Assert.ThrowsAsync<ApplicationException>(async()=> await dbManager.ExecuteSelectStatementScalarAsync("Foo"));
        }

        [Fact]
        public void ExecuteStoredProcedure_ServerInfo_Equals()
        {
            var expected = "Microsoft SQL Server";

            var log = Mock.Create<ILogger>();
            var dbManager = new DBManager(log, goodProvider,goodConnection);
            Assert.NotNull(dbManager);

            var selectStatement = "sp_server_info";
            var paramList = new Dictionary<string, object>();
            paramList.Add("attribute_id",1);

            var result = dbManager.ExecuteStoredProcedure(paramList, selectStatement);
            Assert.NotNull(result);

            Assert.Equal(1, result.Rows.Count);
            Assert.Equal(expected, result.Rows[0]["attribute_value"].ToString());

        }
        [Fact]
        public async Task ExecuteStoredProcedureAsync_ServerInfo_Equals()
        {
            var expected = "Microsoft SQL Server";

            var log = Mock.Create<ILogger>();
            var dbManager = new DBManager(log, goodProvider,goodConnection);
            Assert.NotNull(dbManager);

            var selectStatement = "sp_server_info";
            var paramList = new Dictionary<string, object>();
            paramList.Add("attribute_id",1);

            var result = await dbManager.ExecuteStoredProcedureAsync(paramList, selectStatement);
            Assert.NotNull(result);

            Assert.Equal(1, result.Rows.Count);
            Assert.Equal(expected, result.Rows[0]["attribute_value"].ToString());

        }

        [Fact]
        public void ExecuteNonQuery_DoNothing_NoError()
        {
            var log = Mock.Create<ILogger>();
            var dbManager = new DBManager(log, goodProvider,goodConnection);
            Assert.NotNull(dbManager);

            var selectStatement = "IF 1=0 SELECT NULL";
            var result = dbManager.ExecuteNonQuery(selectStatement);
            Assert.Equal(-1,result);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_DoNothing_NoError()
        {
            var log = Mock.Create<ILogger>();
            var dbManager = new DBManager(log, goodProvider,goodConnection);
            Assert.NotNull(dbManager);

            var selectStatement = "IF 1=0 SELECT NULL";
            var result = await dbManager.ExecuteNonQueryAsync(selectStatement);
            Assert.Equal(-1,result);
        }
        
    }
}
