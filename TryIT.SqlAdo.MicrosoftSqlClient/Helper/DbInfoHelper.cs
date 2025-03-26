using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Helper
{
    /// <summary>
    /// helper class for get database information
    /// </summary>
    public static class DbInfoHelper
    {
        /// <summary>
        /// get table identity column name
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetIdentityColumnName(this DbConnector dbConnector, string tableName)
        {
            string sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA + '.' + TABLE_NAME = @tableName AND COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tableName", tableName)
            };

            return dbConnector.ExecuteScalar<string>(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// get table structure
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<DbTableStructure> GetDbTableStructure(this DbConnector dbConnector, string tableName)
        {
            string sql = $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA + '.' + TABLE_NAME = @tableName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tableName", tableName)
            };

            DataTable table = dbConnector.FetchDataTable(sql, CommandType.Text, parameters);

            if (table == null || table.Rows.Count == 0)
            {
                throw new Exception($"get target table structure failed, please ensure target table '{tableName}' exists and account has permission");
            }

            return table.Rows.OfType<DataRow>().Select(k =>
                  new DbTableStructure
                  {
                      TABLE_NAME = tableName,
                      COLUMN_NAME = k[0].ToString(),
                      DATA_TYPE = k[1].ToString(),
                      CHARACTER_MAXIMUM_LENGTH = k[2].ToString()
                  }).ToList();
        }

        /// <summary>
        /// get temp table structure
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<DbTableStructure> GetDbTableStructure_TempDb(this DbConnector dbConnector, string tableName)
        {
            string sql = $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM Tempdb.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA + '.' + TABLE_NAME = @tableName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tableName", tableName)
            };

            DataTable table = dbConnector.FetchDataTable(sql, CommandType.Text, parameters);

            if (table == null || table.Rows.Count == 0)
            {
                throw new Exception($"get target table structure failed, please ensure target table '{tableName}' exists and account has permission");
            }

            return table.Rows.OfType<DataRow>().Select(k =>
                  new DbTableStructure
                  {
                      TABLE_NAME = tableName,
                      COLUMN_NAME = k[0].ToString(),
                      DATA_TYPE = k[1].ToString(),
                      CHARACTER_MAXIMUM_LENGTH = k[2].ToString()
                  }).ToList();
        }

        /// <summary>
        /// get always encrypted columns
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <returns></returns>
        public static List<AlwaysEncryptedColumn> GetAlwaysEncryptedColumns(this DbConnector dbConnector)
        { 
            return GetAlwaysEncryptedColumns(dbConnector, string.Empty);
        }

        /// <summary>
        /// get always encrypted columns
        /// </summary>
        /// <param name="dbConnector"></param>
        /// <param name="tableName">table name with schema, e.g. dbo.table</param>
        /// <returns></returns>
        public static List<AlwaysEncryptedColumn> GetAlwaysEncryptedColumns(this DbConnector dbConnector, string tableName)
        {
            string cmdText = @"SELECT 
	                                sch.name + '.' + tbl.name AS TableName,
	                                col.name AS ColumnName,
	                                typ.name AS ColumnType,
                                    CASE
                                        WHEN typ.name = 'nvarchar' THEN col.max_length / 2
                                        ELSE col.max_length 
                                    END AS ColumnMaxLength,
	                                col.Precision,
	                                col.Scale,
	                                col.encryption_type_desc AS EncryptionType,
	                                cek.name AS ColumnEncryptKeyName,
	                                cmk.name AS MasterKeyName,
	                                cmk.key_store_provider_name AS KeyStoreProviderName,
	                                cmk.key_path AS KeyPath
                                FROM sys.tables tbl
                                JOIN sys.columns col ON col.object_id = tbl.object_id
                                JOIN sys.column_encryption_keys cek ON cek.column_encryption_key_id = col.column_encryption_key_id
                                JOIN sys.column_encryption_key_values cek_val ON cek_val.column_encryption_key_id = cek.column_encryption_key_id
                                JOIN sys.column_master_keys cmk ON cmk.column_master_key_id = cek_val.column_master_key_id
                                JOIN sys.schemas sch ON sch.schema_id = tbl.schema_id 
                                JOIN sys.types typ ON typ.system_type_id = col.system_type_id 
	                                AND typ.user_type_id = col.user_type_id
                                WHERE col.[encryption_type] IS NOT NULL
                                    AND (sch.name + '.' + tbl.name = @tableName OR @tableName = '')
                        ";

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@tableName", tableName)
            };

            var table = dbConnector.FetchDataTable(cmdText, CommandType.Text, sqlParameters);


            return table.Rows.OfType<DataRow>().Select(k =>
                  new AlwaysEncryptedColumn
                  {
                      TableName = k[0].ToString(),
                      ColumnName = k[1].ToString(),
                      ColumnType = k[2].ToString(),
                      ColumnMaxLength = int.Parse(k[3].ToString()),
                      Precision = int.Parse(k[4].ToString()),
                      Scale = int.Parse(k[5].ToString()),
                      EncryptionType = k[6].ToString(),
                      ColumnEncryptKeyName = k[7].ToString(),
                      MasterKeyName = k[8].ToString(),
                      KeyStoreProviderName = k[9].ToString(),
                      KeyPath = k[10].ToString()
                  }).ToList();
        }
    }
}
