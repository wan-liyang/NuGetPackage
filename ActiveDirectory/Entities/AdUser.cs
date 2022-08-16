using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveDirectory.Entities
{
	public class AdUser : AdUserBaseInfo
	{
		public string GivenName { get; set; }
		public string LastName { get; set; }
		/// <summary>
		/// indicator whether account is active or disabled, value could be: Yes / No
		/// </summary>
		public string AccountActive { get; set; }
		public DateTime? ExpirationDate { get; set; }
		public DateTime? AccountLockoutTime { get; set; }
		public DateTime? LastPasswordSet { get; set; }
		public DateTime? PasswordExpiryOn { get; set; }
		public DateTime? CreationDate { get; set; }
		//public AdUserManager Manager { get; set; }
		//public List<AdUserGroup> MemberOf_Group { get; set; }
	}
}
