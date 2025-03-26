namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>
    /// table structure
    /// </summary>
    public class DbTableStructure
    {
        /// <summary>
        /// name of table
        /// </summary>
        public string TABLE_NAME { get; set; }

        /// <summary>
        /// name of column
        /// </summary>
        public string COLUMN_NAME { get; set; }

        /// <summary>
        /// data type of column
        /// </summary>
        public string DATA_TYPE { get; set; }

        /// <summary>
        /// max length of column
        /// </summary>
        public string CHARACTER_MAXIMUM_LENGTH { get; set; }
    }
}
