using System;

namespace TryIT.MicrosoftGraphApi.Model.Sharepoint
{
    /// <summary>
    /// file upload result information
    /// </summary>
    public class FileUploadResult
    {
        /// <summary>
        /// file name without path
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// file name with full path
        /// </summary>
        public string FileFullName { get; set; }
        /// <summary>
        /// indicator whether upload success
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// exception detail if upload failed
        /// </summary>
        public Exception Exception { get; set; }
    }
}
