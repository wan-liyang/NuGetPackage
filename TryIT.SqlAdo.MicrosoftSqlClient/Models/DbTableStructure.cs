using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    public class DbTableStructure
    {
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public string DATA_TYPE { get; set; }
        public string CHARACTER_MAXIMUM_LENGTH { get; set; }
    }
}
