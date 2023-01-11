using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.Model
{
    /// <summary>
    /// project (P_), Workbooks (W_), Datasources (DS_), Flow (F_), Legen (L_), Metrics (M_)
    /// </summary>
    public class SiteProjectPermission
    {
        public string ProjectName { get; set; }
        public string GroupName { get; set; }
        public string P_View { get; set; }
        public string P_Publish { get; set; }
        public string W_View { get; set; }
        public string W_Filter { get; set; }
        public string W_ViewComments { get; set; }
        public string W_AddComments { get; set; }
        public string W_DownloadImage { get; set; }
        public string W_DownloadSummaryData { get; set; }
        public string W_RunExplainData { get; set; }
        public string W_ShareCustomized { get; set; }
        public string W_DownloadFullData { get; set; }
        public string W_WebEdit { get; set; }
        public string W_Downlaod { get; set; }
        public string W_Overwrite { get; set; }
        public string W_CreateMetrics { get; set; }
        public string W_Move { get; set; }
        public string W_Delete { get; set; }
        public string W_SetPermission { get; set; }
        public string DS_View { get; set; }
        public string DS_Connect { get; set; }
        public string DS_Download { get; set; }
        public string DS_Overwrite { get; set; }
        public string DS_SaveAs { get; set; }
        public string DS_Move { get; set; }
        public string DS_Delete { get; set; }
        public string DS_SetPermission { get; set; }
        public string DR_View { get; set; }
        public string DR_Overwrite { get; set; }
        public string DR_Move { get; set; }
        public string DR_Delete { get; set; }
        public string DR_SetPermission { get; set; }
        public string F_View { get; set; }
        public string F_Download { get; set; }
        public string F_Run { get; set; }
        public string F_Overwrite { get; set; }
        public string F_WebEdit { get; set; }
        public string F_Move { get; set; }
        public string F_Delete { get; set; }
        public string F_SetPermission { get; set; }
        public string L_View { get; set; }
        public string L_Overwrite { get; set; }
        public string L_Move { get; set; }
        public string L_Delete { get; set; }
        public string L_SetPermission { get; set; }
        public string M_View { get; set; }
        public string M_Overwrite { get; set; }
        public string M_Move { get; set; }
        public string M_Delete { get; set; }
        public string M_SetPermission { get; set; }
    }
    public static class SiteProjectPermissionExtension
    {
        public static List<Capability> GetProjectCapability(this SiteProjectPermission permission)
        {
            List<Capability> capabilities = new List<Capability>();

            if (!string.IsNullOrEmpty(permission.P_View))
            {
                capabilities.Add(new Capability
                {
                    Type = CapabilityType_Project.Read.ToString(),
                    Mode = GetCapabilityMode(permission.P_View)
                });
            }
            if (!string.IsNullOrEmpty(permission.P_Publish))
            {
                capabilities.Add(new Capability
                {
                    Type = CapabilityType_Project.Write.ToString(),
                    Mode = GetCapabilityMode(permission.P_Publish)
                });
            }

            return capabilities;
        }
        public static List<Capability> GetDefaultCapability(this SiteProjectPermission permission, DefaultPermissionTypeEnum defaultPermissionType)
        {
            List<Capability> capabilities = new List<Capability>();

            switch (defaultPermissionType)
            {
                case DefaultPermissionTypeEnum.workbooks:
                    if (!string.IsNullOrEmpty(permission.W_View))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.Read.ToString(),
                            Mode = GetCapabilityMode(permission.W_View)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_Filter))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.Filter.ToString(),
                            Mode = GetCapabilityMode(permission.W_Filter)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_ViewComments))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.ViewComments.ToString(),
                            Mode = GetCapabilityMode(permission.W_ViewComments)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_AddComments))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.AddComment.ToString(),
                            Mode = GetCapabilityMode(permission.W_AddComments)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_DownloadImage))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.ExportImage.ToString(),
                            Mode = GetCapabilityMode(permission.W_DownloadImage)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_DownloadSummaryData))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.ExportData.ToString(),
                            Mode = GetCapabilityMode(permission.W_DownloadSummaryData)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_RunExplainData))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.RunExplainData.ToString(),
                            Mode = GetCapabilityMode(permission.W_RunExplainData)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_ShareCustomized))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.ShareView.ToString(),
                            Mode = GetCapabilityMode(permission.W_ShareCustomized)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_DownloadFullData))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.ViewUnderlyingData.ToString(),
                            Mode = GetCapabilityMode(permission.W_DownloadFullData)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_WebEdit))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.WebAuthoring.ToString(),
                            Mode = GetCapabilityMode(permission.W_WebEdit)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_Downlaod))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.ExportXml.ToString(),
                            Mode = GetCapabilityMode(permission.W_Downlaod)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_Overwrite))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.Write.ToString(),
                            Mode = GetCapabilityMode(permission.W_Overwrite)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_CreateMetrics))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.CreateRefreshMetrics.ToString(),
                            Mode = GetCapabilityMode(permission.W_CreateMetrics)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_Move))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.ChangeHierarchy.ToString(),
                            Mode = GetCapabilityMode(permission.W_Move)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_Delete))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.Delete.ToString(),
                            Mode = GetCapabilityMode(permission.W_Delete)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.W_SetPermission))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_Workbook.ChangePermissions.ToString(),
                            Mode = GetCapabilityMode(permission.W_SetPermission)
                        });
                    }
                    break;
                case DefaultPermissionTypeEnum.datasources:
                    if (!string.IsNullOrEmpty(permission.DS_View))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_DataSource.Read.ToString(),
                            Mode = GetCapabilityMode(permission.DS_View)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.DS_Connect))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_DataSource.Connect.ToString(),
                            Mode = GetCapabilityMode(permission.DS_Connect)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.DS_Download))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_DataSource.ExportXml.ToString(),
                            Mode = GetCapabilityMode(permission.DS_Download)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.DS_Overwrite))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_DataSource.Write.ToString(),
                            Mode = GetCapabilityMode(permission.DS_Overwrite)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.DS_SaveAs))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_DataSource.SaveAs.ToString(),
                            Mode = GetCapabilityMode(permission.DS_SaveAs)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.DS_Move))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_DataSource.ChangeHierarchy.ToString(),
                            Mode = GetCapabilityMode(permission.DS_Move)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.DS_Delete))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_DataSource.Delete.ToString(),
                            Mode = GetCapabilityMode(permission.DS_Delete)
                        });
                    }
                    if (!string.IsNullOrEmpty(permission.DS_SetPermission))
                    {
                        capabilities.Add(new Capability
                        {
                            Type = CapabilityType_DataSource.ChangePermissions.ToString(),
                            Mode = GetCapabilityMode(permission.DS_SetPermission)
                        });
                    }
                    break;
                case DefaultPermissionTypeEnum.dataroles:
                    break;
                case DefaultPermissionTypeEnum.lenses:
                    break;
                case DefaultPermissionTypeEnum.flows:
                    break;
                case DefaultPermissionTypeEnum.metrics:
                    break;
                case DefaultPermissionTypeEnum.databases:
                    break;
                case DefaultPermissionTypeEnum.tables:
                    break;
                default:
                    break;
            }

            return capabilities;
        }

        private static CapabilityMode GetCapabilityMode(string value)
        {
            bool result = value.ToUpper() == "YES" || value.ToUpper() == "TRUE" || value.ToUpper() == "Y";
            return result ? CapabilityMode.Allow : CapabilityMode.Deny;
        }
    }

}
