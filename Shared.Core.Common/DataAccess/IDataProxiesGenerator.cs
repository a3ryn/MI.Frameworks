using System;
using System.Collections.Generic;

namespace Shared.Core.Common.DataAccess
{
    public interface IDataProxiesGenerator
    {
        void GenerateProxiesFromDb(string targetPath, string @namespace, string connStr, string schema,
            string dbObjNamePrefix = null, bool includeTablesAndViews = true, bool includeUdtts = true);

        void GenerateProxiesFromDb(string targetPath, string @namespace, string connStr, string schema,
            IEnumerable<string> tableAndViewNames = null, IEnumerable<string> udttNames = null);
    }
}
