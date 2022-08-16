using System;
using System.Collections.Generic;

namespace ActiveDirectory.Models
{
    internal class AdUserModel
	{
		public string DistinguishedName { get; set; }
		public string DisplayName { get; set; }
		public string GivenName { get; set; }
		public string LastName { get; set; }
		public string UserLogin { get; set; }
		public string EmployeeId { get; set; }
		public string EmailAddress { get; set; }
		public bool AccountActive { get; set; }
		public string Manager_DistinguishedName { get; set; }
		public AdUserModel Manager { get; set; }
		public List<string> MemberOf_DistinguishedName { get; set; }
		public List<AdGroupModel> MemberOf_Group { get; set; }
		public DateTime? ExpirationDate { get; set; }
		public DateTime? AccountLockoutTime { get; set; }
		public DateTime? LastPasswordSet { get; set; }
		public DateTime? PasswordExpiryOn { get; set; }
		public DateTime? CreationDate { get; set; }	
	}
}
