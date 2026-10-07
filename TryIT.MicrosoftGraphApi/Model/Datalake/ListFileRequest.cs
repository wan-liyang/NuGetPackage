using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Datalake
{
    /// <summary>
    /// Represents a request to list files within a specified directory of a storage account, with optional filtering by
    /// regular expression.
    /// </summary>
    /// <remarks>Use this class to specify the file system, directory, and an optional regular expression
    /// pattern when retrieving a list of files. Only files matching the provided pattern will be included in the
    /// results if a regex is specified.</remarks>
    public class ListFileRequest : BaseRequest
    {
        /// <summary>
        /// Gets or sets the root directory (container) of the storage account.
        /// </summary>
        public string FileSystem { get; set; }

        /// <summary>
        /// Gets or sets the path of the directory to be used.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Regex pattern to filter files. Only files matching this regex will be returned. If not specified, all files will be returned.
        /// </summary>
        public string Regex { get; set; }
    }
}
