using TryIT.ActiveDirectory.Constants;
using TryIT.ActiveDirectory.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

namespace TryIT.ActiveDirectory.Helper
{
    internal class AdUserHelper
	{
		internal AdUserHelper()
        {

        }

		/// <summary>
		/// find user by EmployeeId, UserLogin, EmailAddress
		/// </summary>
		/// <param name="keyword"></param>
		/// <returns></returns>
		public AdUserModel FindUserByKeyword(string keyword)
        {
			var strFilter = new StringBuilder();

			string attr = AttributeMap.User.DetailMap.TryGetValue(AttributeMap.User.EmployeeId);
			strFilter.Append($"({attr}={keyword})");

			attr = AttributeMap.User.DetailMap.TryGetValue(AttributeMap.User.UserLogin);
			strFilter.Append($"({attr}={keyword})");

			attr = AttributeMap.User.DetailMap.TryGetValue(AttributeMap.User.EmailAddress);
			strFilter.Append($"({attr}={keyword})");

			string filter = $"(&(objectClass=user)(objectCategory=person)(|{strFilter}))";

			return FindUser(filter);
		}

		public AdUserModel FindUserByLogin(string userLogin)
		{
			string attr = AttributeMap.User.DetailMap.TryGetValue(AttributeMap.User.UserLogin);
			string filter = $"(&(objectClass=user)(objectCategory=person)({attr}={userLogin}))";
			return FindUser(filter);
		}
		public AdUserModel FindUserByEmployeeId(string employeeId)
		{
			string attr = AttributeMap.User.DetailMap.TryGetValue(AttributeMap.User.EmployeeId);
			string filter = $"(&(objectClass=user)(objectCategory=person)({attr}={employeeId}))";
			return FindUser(filter);
		}
		public AdUserModel FindUserByEmailAddress(string emailaddress)
		{
			string attr = AttributeMap.User.DetailMap.TryGetValue(AttributeMap.User.EmailAddress);
			string filter = $"(&(objectClass=user)(objectCategory=person)({attr}={emailaddress}))";
			return FindUser(filter);
		}

		public AdUserModel FindUserManager(string userDistinguishedName)
		{
			var user = FindUserByDistinuishdName(userDistinguishedName, AttributeMap.User.UserManagerMap);

			return FindUserByDistinuishdName(user.Manager_DistinguishedName, AttributeMap.User.ManagerMap);			
		}

		public AdUserModel FindUserGroup(string userDistinguishedName)
		{
			return FindUserByDistinuishdName(userDistinguishedName, AttributeMap.User.UserGroupMap);
		}

		private AdUserModel FindManagerByDistinuishdName(string distinguishedName)
		{
			return FindUserByDistinuishdName(distinguishedName, AttributeMap.User.ManagerMap);
		}

		private AdUserModel FindUserByDistinuishdName(string distinguishedName, Dictionary<string, string> attributeMap = null)
		{
			string attr = AttributeMap.User.DetailMap.TryGetValue(AttributeMap.User.DistinguishedName);
			string filter = $"(&(objectClass=user)(objectCategory=person)({attr}={distinguishedName}))";
			return _FindUser(filter, attributeMap);
		}

		private AdUserModel FindUser(string filter)
		{
			AdUserModel user = _FindUser(filter);

			if (user != null && !string.IsNullOrEmpty(user.Manager_DistinguishedName))
            {
				AdUserModel manager = FindManagerByDistinuishdName(user.Manager_DistinguishedName);
                if (manager != null)
                {
					user.Manager = manager;
				}
			}
			return user;
		}

		private AdUserModel _FindUser(string filter, Dictionary<string, string> attributeMap = null)
		{
			var search = new DirectorySearcher(new DirectoryEntry())
			{
				PageSize = 1000,
				Filter = filter
			};

			//make sure only needed attribute is sent back
			if (attributeMap == null)
            {
				attributeMap = AttributeMap.User.DetailMap;
			}

			foreach (var item in attributeMap)
			{
				search.PropertiesToLoad.Add(item.Value);
			}

			AdUserModel model = null;

			using (var results = search.FindAll())
			{
				foreach (SearchResult result in results)
				{
					model = new AdUserModel();

					string attr = attributeMap.TryGetValue(AttributeMap.User.DistinguishedName);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						model.DistinguishedName = (string)result.Properties[attr][0];
					}

					attr = attributeMap.TryGetValue(AttributeMap.User.DisplayName);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						model.DisplayName = (string)result.Properties[attr][0];
					}

					attr = attributeMap.TryGetValue(AttributeMap.User.GivenName);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						model.GivenName = (string)result.Properties[attr][0];
					}

					attr = attributeMap.TryGetValue(AttributeMap.User.LastName);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						model.LastName = (string)result.Properties[attr][0];
					}

					attr = attributeMap.TryGetValue(AttributeMap.User.UserLogin);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						model.UserLogin = (string)result.Properties[attr][0];
					}

					attr = attributeMap.TryGetValue(AttributeMap.User.EmployeeId);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						model.EmployeeId = (string)result.Properties[attr][0];
					}

					attr = attributeMap.TryGetValue(AttributeMap.User.EmailAddress);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						model.EmailAddress = (string)result.Properties[attr][0];
					}

					attr = attributeMap.TryGetValue(AttributeMap.User.UserAccountControl);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						int userAccountControlValue = (int)result.Properties[attr][0];
						model.AccountActive = !Convert.ToBoolean(userAccountControlValue & 0x0002);
					}

                    attr = attributeMap.TryGetValue(AttributeMap.User.Manager_DistinguishedName);
                    if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
                    {
                        model.Manager_DistinguishedName = (string)result.Properties[attr][0];
                    }

                    attr = attributeMap.TryGetValue(AttributeMap.User.MemberOf_DistinguishedName);
                    if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
                    {
                        List<AdGroupModel> groups = new List<AdGroupModel>();
                        List<string> groupDistinguishedName = new List<string>();
                        var values = result.Properties[attr];
                        if (values.Count > 0)
                        {
                            AdGroupHelper adGroup = new AdGroupHelper();

                            var valueItem = values.GetEnumerator();
                            bool hasNext = valueItem.MoveNext();

                            while (hasNext)
                            {
                                AdGroupModel group = adGroup.FindGroup(valueItem.Current.ToString());

                                groupDistinguishedName.Add(group.DistinguishedName);
                                groups.Add(group);

                                hasNext = valueItem.MoveNext();
                            }
                        }

                        model.MemberOf_DistinguishedName = groupDistinguishedName;
                        model.MemberOf_Group = groups;
                    }

                    attr = attributeMap.TryGetValue(AttributeMap.User.ExpirationDate);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						var value = (Int64)result.Properties[attr][0];
                        if (value != 0 && value != Int64.MaxValue)
                        {
							model.ExpirationDate = DateTime.FromFileTime(value);
						}
					}

					attr = attributeMap.TryGetValue(AttributeMap.User.AccountLockoutTime);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						var value = (Int64)result.Properties[attr][0];
						if (value != 0 && value != Int64.MaxValue)
						{
							model.AccountLockoutTime = DateTime.FromFileTime(value);
						}
					}

					attr = attributeMap.TryGetValue(AttributeMap.User.LastPasswordSet);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						var value = (Int64)result.Properties[attr][0];
						if (value != 0 && value != Int64.MaxValue)
						{
							model.LastPasswordSet = DateTime.FromFileTime(value);
						}
					}

					TimeSpan maxAge = GetMaxPasswordAge();
                    if (model.LastPasswordSet.HasValue)
                    {
						model.PasswordExpiryOn = model.LastPasswordSet.Value.AddDays(maxAge.Days);
					}

					attr = attributeMap.TryGetValue(AttributeMap.User.CreationDate);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						var value = (DateTime)result.Properties[attr][0];
						model.CreationDate = value.AddHours(8);
					}
				}
			}
			return model;
		}

		private TimeSpan GetMaxPasswordAge()
		{
			var maxPwdAge = TimeSpan.FromTicks(0);

			var search = new DirectorySearcher(new DirectoryEntry())
			{
				PageSize = 1000,
				Filter = "(&(objectClass=*))",
				SearchScope = SearchScope.Base
			};

			search.PropertiesToLoad.Add("maxPwdAge");

			using (var results = search.FindAll())
			{
				foreach (SearchResult result in results)
				{
					if (result.Properties.Contains("maxPwdAge"))
                    {
						maxPwdAge = TimeSpan.FromTicks((long)result.Properties["maxPwdAge"][0]);
					}
				}
			}

			return maxPwdAge.Duration();
		}
	}
}
