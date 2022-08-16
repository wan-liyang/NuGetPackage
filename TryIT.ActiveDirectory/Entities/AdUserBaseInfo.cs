using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.ActiveDirectory.Entities
{
	public class AdUserBaseInfo
	{
		public string DistinguishedName { get; set; }
		public string DisplayName { get; set; }
		public string UserLogin { get; set; }
		public string EmployeeId { get; set; }
		public string EmailAddress { get; set; }
	}
}
