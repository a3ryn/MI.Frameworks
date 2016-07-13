using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Core.Common.DataAccess;
using Shared.Core.Common.DI;

namespace Shared.Frameworks.Tests
{
    [TestClass]
    public class DataAccessTests
    {
        [TestMethod]
        public void StProcWithOutputTest()
        {
            const string stProcName = "sys.sp_sequence_get_range";
            const string seqName = "[msg].[MessageId_Seq]";
            const string connStr = @"Data Source=TXRH-3FPXX52\SQLEXPRESS;initial catalog=imVision.Messaging;integrated security=True;MultipleActiveResultSets=True;App=abd";
            const int count = 10;
            var firstValue = 0m;

            var _dataAdapter = new DataAccess.DataAdapter();
            var result = _dataAdapter.ExecStProc<decimal>(stProcName, new Dictionary<string, object>
            {
                ["@sequence_name"] = seqName,
                ["@range_size"] = count
            }, new Dictionary<string, SqlDbType> { [@"range_first_value"] = SqlDbType.Variant }, connStr: connStr);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }
    }
}
