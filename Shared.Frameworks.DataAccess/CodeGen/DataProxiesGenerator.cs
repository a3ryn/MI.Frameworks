using System.Collections.Generic;
using System.ComponentModel.Composition;
using Shared.Core.Common.DataAccess;

namespace Shared.Frameworks.DataAccess.CodeGen
{
    [Export(typeof(IDataProxiesGenerator))]
    public class DataProxiesGenerator : IDataProxiesGenerator
    {
        public void GenerateProxiesFromDb(string targetPath, string @namespace,
            string connStr, string schema = "dbo", string dbObjNamePrefix = null,
            bool includeTablesAndViews = true, bool includeUdtts = true)
            => Helpers.CodeGenerator.GenerateProxiesFromDb(targetPath, @namespace,
                connStr, dbObjNamePrefix, includeTablesAndViews, includeUdtts);


        public void GenerateProxiesFromDb(string targetPath, string @namespace,
            string connStr, string schema = "dbo", IEnumerable<string> tableAndViewNames = null,
            IEnumerable<string> udttNames = null)
            => Helpers.CodeGenerator.GenerateProxiesFromDb(targetPath, @namespace,
                connStr, tableAndViewNames, udttNames);
    }
}
