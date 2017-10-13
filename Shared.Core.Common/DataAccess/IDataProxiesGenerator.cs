/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;

namespace Shared.Core.Common.DataAccess
{
    /// <summary>
    /// Generator of types from tabular database models - tables, views, UDTTs 
    /// (Type generation from TVFs not supported yet)
    /// </summary>
    public interface IDataProxiesGenerator
    {
        void GenerateProxiesFromDb(string targetPath, string @namespace, string connStr, string schema,
            string dbObjNamePrefix = null, bool includeTablesAndViews = true, bool includeUdtts = true);

        void GenerateProxiesFromDb(string targetPath, string @namespace, string connStr, string schema,
            IEnumerable<string> tableAndViewNames = null, IEnumerable<string> udttNames = null);
    }
}
