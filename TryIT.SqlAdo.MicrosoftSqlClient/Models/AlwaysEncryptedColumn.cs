namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>
    /// always encrypted column
    /// </summary>
    public class AlwaysEncryptedColumn
    {
        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ColumnType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ColumnMaxLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Precision { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Scale { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EncryptionType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ColumnEncryptKeyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MasterKeyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string KeyStoreProviderName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string KeyPath { get; set; }
    }
}
