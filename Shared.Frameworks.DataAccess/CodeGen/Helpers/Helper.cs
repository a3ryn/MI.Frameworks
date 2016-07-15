using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Shared.Core.Common.DataAccess;

namespace Shared.Frameworks.DataAccess.CodeGen.Helpers
{
    /// <summary>
    /// Represents a connection string to a SQL server/database
    /// </summary>
    public class SqlConnectionString : ValueObject<SqlConnectionString>, IConvertible
    {
        public static int DefultMaxPoolSize = 100;
        /// <summary>
        /// Implements utilities for constructing a connection string
        /// </summary>
        public class Builder
        {
            public static implicit operator string(Builder csb) => csb.ToString();

            public static implicit operator Builder(string cs) => new Builder(cs);

            public static implicit operator SqlConnectionString(Builder csb) => csb.Finish();

            private readonly SqlConnectionStringBuilder _csb;

            public Builder(string cs = "")
            {
                _csb = new SqlConnectionStringBuilder(cs);
            }

            public Builder ConnectTo(string server, string database = "")
            {
                _csb.DataSource = server;
                _csb.InitialCatalog = database;
                return this;
            }

            public Builder UsingCredentials(string user, string password)
            {
                _csb.IntegratedSecurity = false;
                _csb.UserID = user;
                _csb.Password = password;
                _csb.PersistSecurityInfo = true;
                return this;
            }

            public Builder UsingIntegratedSecurity()
            {
                _csb.IntegratedSecurity = true;
                _csb.UserID = String.Empty;
                _csb.Password = String.Empty;
                _csb.PersistSecurityInfo = true;
                return this;
            }

            public Builder Database(string dbname)
            {
                _csb.InitialCatalog = dbname;
                return this;
            }

            public override string ToString() => _csb.ToString();

            public SqlConnectionString Finish() => _csb.ToString();
        }

        public static Builder Build(string cs = "") => new Builder(cs);

        /// <summary>
        /// Converts a <see cref="SqlConnectionString"/> instance to text
        /// </summary>
        /// <param name="cs">The instance to convert</param>
        public static implicit operator string(SqlConnectionString cs) => cs.Text;

        /// <summary>
        /// Converts text representing a connection string to a <see cref="SqlConnectionString"/> instance
        /// </summary>
        /// <param name="Text"></param>
        public static implicit operator SqlConnectionString(string Text) => new SqlConnectionString(Text);

        /// <summary>
        /// The connection string text
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Initializes a new <see cref="SqlConnectionString"/> instance
        /// </summary>
        /// <param name="Text"></param>
        public SqlConnectionString(string Text)
        {
            this.Text = Text;
        }

        public Builder Rebuild() => new Builder(this.Text);

        public override string ToString() => Text;

        public string DatabaseName => (new SqlConnectionStringBuilder(this.Text)).InitialCatalog;

        public string ServerName => (new SqlConnectionStringBuilder(this.Text)).DataSource;

        public SqlConnectionString RemoveCatalog()
        {
            var csb = new SqlConnectionStringBuilder(this.Text);
            csb.InitialCatalog = String.Empty;
            return new SqlConnectionString(csb.ToString());
        }

        #region IConvertible
        TypeCode IConvertible.GetTypeCode()
        {
            return Text.GetTypeCode();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToBoolean(provider);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToChar(provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToSByte(provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToByte(provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToInt16(provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToUInt16(provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToInt32(provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToUInt32(provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToInt64(provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToUInt64(provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToSingle(provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToDouble(provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToDecimal(provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)Text).ToDateTime(provider);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return Text.ToString(provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)Text).ToType(conversionType, provider);
        }
        #endregion
    }


    public class SchemaTableRecord
    {
        [DataMapping("ColumnName", 0)]
        public string ColumnName { get; set; }

        [DataMapping("ColumnOrdinal", 1)]
        public System.Int32 ColumnOrdinal { get; set; }

        [DataMapping("ColumnSize", 2)]
        public System.Int32 ColumnSize { get; set; }

        [DataMapping("NumericPrecision", 3)]
        public System.Int16 NumericPrecision { get; set; }

        [DataMapping("NumericScale", 4)]
        public System.Int16 NumericScale { get; set; }

        [DataMapping("IsUnique", 5)]
        public bool IsUnique { get; set; }

        [DataMapping("IsKey", 6)]
        public bool? IsKey { get; set; }

        [DataMapping("BaseServerName", 7)]
        public string BaseServerName { get; set; }

        [DataMapping("BaseCatalogName", 8)]
        public string BaseCatalogName { get; set; }

        [DataMapping("BaseColumnName", 9)]
        public string BaseColumnName { get; set; }

        [DataMapping("BaseSchemaName", 10)]
        public string BaseSchemaName { get; set; }

        [DataMapping("BaseTableName", 11)]
        public string BaseTableName { get; set; }

        [DataMapping("DataType", 12)]
        public Type DataType { get; set; }

        [DataMapping("AllowDBNull", 13)]
        public bool AllowDBNull { get; set; }

        [DataMapping("ProviderType", 14)]
        public System.Int32 ProviderType { get; set; }

        [DataMapping("IsAliased", 15)]
        public bool? IsAliased { get; set; }

        [DataMapping("IsExpression", 16)]
        public bool? IsExpression { get; set; }

        [DataMapping("IsIdentity", 17)]
        public bool IsIdentity { get; set; }

        [DataMapping("IsAutoIncrement", 18)]
        public bool IsAutoIncrement { get; set; }

        [DataMapping("IsRowVersion", 19)]
        public bool IsRowVersion { get; set; }

        [DataMapping("IsHidden", 20)]
        public bool? IsHidden { get; set; }

        [DataMapping("IsLong", 21)]
        public bool IsLong { get; set; }

        [DataMapping("IsReadOnly", 22)]
        public bool IsReadOnly { get; set; }

        [DataMapping("ProviderSpecificDataType", 23)]
        public Type ProviderSpecificDataType { get; set; }

        [DataMapping("DataTypeName", 24)]
        public string DataTypeName { get; set; }

        [DataMapping("XmlSchemaCollectionDatabase", 25)]
        public string XmlSchemaCollectionDatabase { get; set; }

        [DataMapping("XmlSchemaCollectionOwningSchema", 26)]
        public string XmlSchemaCollectionOwningSchema { get; set; }

        [DataMapping("XmlSchemaCollectionCollectionName", 27)]
        public string XmlSchemaCollectionCollectionName { get; set; }

        [DataMapping("UdtAssemblyQualifiedName", 28)]
        public string UdtAssemblyQualifiedName { get; set; }

        [DataMapping("NonVersionedProviderType", 29)]
        public System.Int32 NonVersionedProviderType { get; set; }

        [DataMapping("IsColumnSet", 30)]
        public bool IsColumnSet { get; set; }
    }


    /// <summary>
    /// Defines extensions for the <see cref="System.Data.SqlClient"/> API 
    /// involved
    /// </summary>
    public static class SqlClientExtensions
    {

        private static DataTable GetSchemaTable(this SqlCommand command)
        {
            var reader = command.ExecuteReader(CommandBehavior.SchemaOnly);
            try
            {
                return reader.GetSchemaTable();
            }
            finally
            {
                reader.Close();
            }
        }

        private static DataTable GetDataTable(this SqlConnectionString cs, string sql, DataTable dst = null)
        {
            var dataTable = dst ?? new DataTable();
            using (var connection = cs.OpenConnection())
            {
                using (var command = connection.CreateCommand(sql))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    return dataTable;
                }
            }
        }

        //private static DataTable GetDataTable(this SqlConnectionString cs, SqlTableQuery query)
        //{
        //    var dataTable = new DataTable(query.SchemaQualifiedTableName);
        //    var sql = query.ToSql();
        //    return cs.GetDataTable(sql, dataTable);
        //}

        /// <summary>
        /// Opens a connection to an identified data source
        /// </summary>
        /// <param name="cs">Provides access to the data source</param>
        /// <returns></returns>
        public static SqlConnection OpenConnection(this SqlConnectionString cs)
        {
            var connection = new SqlConnection(cs);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Creates a <see cref="SqlCommand"/> from supplied SQL on a <see cref="SqlConnection"/>
        /// </summary>
        /// <param name="connection">The connection</param>
        /// <param name="sql">The SQL that specifies the command</param>
        /// <returns></returns>
        public static SqlCommand CreateCommand(this SqlConnection connection, string sql) =>
            new SqlCommand(sql, connection);



        public static IEnumerable<SchemaTableRecord> GetResultSchema(this SqlConnectionString cs, string sql)
        {
            using (var connection = cs.OpenConnection())
            {
                using (var command = connection.CreateCommand(sql))
                {
                    var table = command.GetSchemaTable();
                    return table.ReadProxies<SchemaTableRecord>();
                }
            }
        }

        public static IEnumerable<TProxy> Get<TProxy>(this SqlConnectionString cs, string sql)
            where TProxy : new()
        {
            var table = cs.GetDataTable(sql);
            return table.ReadProxies<TProxy>();           
        }

        public static IEnumerable<TProxy> ReadProxies<TProxy>(this DataTable dt, Func<object,TProxy> translatorDelegate = null)
            where TProxy : new()
        {
            var result = new List<TProxy>();
            foreach (DataRow row in dt.Rows)
            {
                var p = translatorDelegate != null
                            ? translatorDelegate(row)
                            : row.MapObject<TProxy>();
                if (p != null)
                {
                    result.Add(p);
                }
            }
            return result;
        }
    }
}
