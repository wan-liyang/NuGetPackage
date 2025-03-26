namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>
    /// always encrypted column
    /// </summary>
    public class AlwaysEncryptedColumn
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public int ColumnMaxLength { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public string EncryptionType { get; set; }
        public string ColumnEncryptKeyName { get; set; }
        public string MasterKeyName { get; set; }
        public string KeyStoreProviderName { get; set; }
        public string KeyPath { get; set; }
    }
}
