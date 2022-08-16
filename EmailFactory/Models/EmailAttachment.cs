namespace EmailFactory.Models
{
    public class EmailAttachment
    {
        /// <summary>
        /// optional, the name of attachment, if not defined will get name from <see cref="FileNameAndPath"/>
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// mandatory, the physical path of attachment, will attach this file as email attachment
        /// </summary>
        public string FileNameAndPath { get; set; }
    }
}
