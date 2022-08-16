using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.ActiveDirectory.Entities
{
    public class AdGroup : AdGroupBaseInfo
    {
        public List<string> Member_DistinguishedName { get; set; }
    }
}
