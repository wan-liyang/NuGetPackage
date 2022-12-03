using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.Model
{
    public class SiteUser
    {
        public string Site { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string SiteRole { get; set; }
    }
}
