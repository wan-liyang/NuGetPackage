using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.ActiveDirectory.Models
{
    internal class AdGroupModel
    {
        public string DistinguishedName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Mail { get; set; }
        public List<string> Member_DistinguishedName { get; set; }
    }
}
