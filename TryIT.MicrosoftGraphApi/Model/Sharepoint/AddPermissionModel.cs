using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Sharepoint
{
    public class AddPermissionModel
    {
        public string email { get; set; }
        public bool sendInvitation { get; set; }
        public PermissionRoleEnum role { get; set; }
    }

    public enum PermissionRoleEnum
    {
        read,
        write
    }
}
