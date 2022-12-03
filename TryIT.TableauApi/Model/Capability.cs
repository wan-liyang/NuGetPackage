using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.Model
{
    public class Capability
    {
        public string Type { get; set; }
        public CapabilityMode Mode { get; set; }
    }

    public enum CapabilityType_Project
    {
        /// <summary>
        /// view
        /// </summary>
        Read,
        /// <summary>
        /// publish
        /// </summary>
        Write,
        ExportData,
        WebAuthoring
    }
    public enum CapabilityType_Workbook
    {
        /// <summary>
        /// add comments
        /// </summary>
        AddComment,
        /// <summary>
        /// move
        /// </summary>
        ChangeHierarchy,
        /// <summary>
        /// set permission
        /// </summary>
        ChangePermissions,
        /// <summary>
        /// delete
        /// </summary>
        Delete,
        /// <summary>
        /// download summary data
        /// </summary>
        ExportData,
        /// <summary>
        /// download image/pdf
        /// </summary>
        ExportImage,
        /// <summary>
        /// download
        /// </summary>
        ExportXml,
        /// <summary>
        /// filter
        /// </summary>
        Filter,
        /// <summary>
        /// view
        /// </summary>
        Read,
        /// <summary>
        /// share customized
        /// </summary>
        ShareView,
        /// <summary>
        /// view comments
        /// </summary>
        ViewComments,
        /// <summary>
        /// download all data
        /// </summary>
        ViewUnderlyingData,
        /// <summary>
        /// web edit
        /// </summary>
        WebAuthoring,
        /// <summary>
        /// overwrite
        /// </summary>
        Write,
        /// <summary>
        /// run explain data
        /// </summary>
        RunExplainData,
        /// <summary>
        /// create refresh metrics
        /// </summary>
        CreateRefreshMetrics
    }
    public enum CapabilityType_DataSource
    {
        /// <summary>
        /// move
        /// </summary>
        ChangeHierarchy,
        /// <summary>
        /// set permission
        /// </summary>
        ChangePermissions,
        /// <summary>
        /// connect
        /// </summary>
        Connect,
        /// <summary>
        /// delete
        /// </summary>
        Delete,
        /// <summary>
        /// download
        /// </summary>
        ExportXml,
        /// <summary>
        /// view
        /// </summary>
        Read,
        /// <summary>
        /// overwrite
        /// </summary>
        Write,
        /// <summary>
        /// save as
        /// </summary>
        SaveAs
    }

    public enum CapabilityMode
    {
        Allow,
        Deny
    }
}
