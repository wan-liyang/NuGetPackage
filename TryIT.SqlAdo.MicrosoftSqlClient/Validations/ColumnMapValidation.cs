using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Validations
{
    internal static class ColumnMapValidation
    {

        /// <summary>
        /// Check if target table the <paramref name="primaryKeys"/> is not null or empty, and its exists in <paramref name="columnMappings"/> value field
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <param name="columnMappings"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void PrimaryKeyExistsInColumnMap(List<string> primaryKeys, Dictionary<string, string> columnMappings)
        {
            if (primaryKeys == null || primaryKeys.Count == 0)
            {
                throw new ArgumentException("Primary key is mandatory for update or upsert");
            }

            if (columnMappings == null || columnMappings.Count == 0)
            {
                throw new ArgumentException("Column mapping is mandatory");
            }

            foreach (var pKey in primaryKeys)
            {
                bool primarkExistsInMap = columnMappings.Values.Any(p => p.Equals(pKey, StringComparison.OrdinalIgnoreCase));

                if (!primarkExistsInMap)
                {
                    throw new ArgumentException($"Primary key '{pKey}' not found in column mappings");
                }
            }
        }


        /// <summary>
        /// validate column map against source DataTable and target Database Table Strucutre
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="targetTableStructure"></param>
        /// <param name="columnMap">expected source to target column map, if empty then will skip this validation</param>
        /// <exception cref="Exception"></exception>
        public static void ValidateColumnMap(DataTable sourceTable, List<DbTableStructure> targetTableStructure, Dictionary<string, string> columnMap)
        {
            if (columnMap != null && columnMap.Count > 0)
            {
                // validate column mapping appear in source table
                ValidateColumnMap_Source(sourceTable, columnMap);

                // validate column mapping appear in target table
                ValidateColumnMap_Target(targetTableStructure, columnMap);
            }
        }

        private static void ValidateColumnMap_Source(DataTable sourceTable, Dictionary<string, string> columnMap)
        {
            List<string> sourceColumns = new List<string>();
            foreach (DataColumn item in sourceTable.Columns)
            {
                sourceColumns.Add(item.ColumnName);
            }
            var notExists = columnMap.Where(map => !sourceColumns.Exists(s => s.Equals(map.Key, StringComparison.OrdinalIgnoreCase))).Select(p => p.Key).ToList();
            if (notExists != null && notExists.Count > 0)
            {
                throw new ArgumentException($"column map not found in source data table: {string.Join(", ", notExists)}");
            }
        }

        private static void ValidateColumnMap_Target(List<DbTableStructure> targetTableStructure, Dictionary<string, string> columnMap)
        {
            string targetTable = targetTableStructure[0].TABLE_NAME;

            var notExists = columnMap.Where(map => !targetTableStructure.Exists(t => t.COLUMN_NAME.Equals(map.Value, StringComparison.OrdinalIgnoreCase))).Select(p => p.Value).ToList();
            if (notExists != null && notExists.Count > 0)
            {
                throw new ArgumentException($"column map not found in target database, table: {targetTable}, column: {string.Join(", ", notExists)}");
            }
        }
    }
}
