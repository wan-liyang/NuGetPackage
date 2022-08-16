using TryIT.ActiveDirectory.Constants;
using TryIT.ActiveDirectory.Models;
using System.Collections.Generic;
using System.DirectoryServices;

namespace TryIT.ActiveDirectory.Helper
{
    internal class AdGroupHelper
    {
		public AdGroupModel FindGroup(string distinguishedName)
        {
			string filter = $"(&(objectClass=group)((distinguishedName={distinguishedName})))";
			return _FindGroup(filter);
		}
		public List<string> FindGroupMember(string distinguishedName)
		{
			string filter = $"(&(objectClass=group)((distinguishedName={distinguishedName})))";

			var search = new DirectorySearcher(new DirectoryEntry())
			{
				PageSize = 1000,
				Filter = filter
			};

			//make sure only needed attribute is sent back
			string attr = AttributeMap.Group.Map.TryGetValue(AttributeMap.Group.Member_DistinguishedName);
			search.PropertiesToLoad.Add(attr);

			List<string> members = new List<string>();

			using (var results = search.FindAll())
			{
				foreach (SearchResult result in results)
				{
                    attr = AttributeMap.Group.Map.TryGetValue(AttributeMap.Group.Member_DistinguishedName);
                    if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
                    {                        
                        var values = result.Properties[attr];
                        if (values.Count > 0)
                        {
                            var valueItem = values.GetEnumerator();
                            bool hasNext = valueItem.MoveNext();

                            while (hasNext)
                            {
                                members.Add(valueItem.Current.ToString());
                                hasNext = valueItem.MoveNext();
                            }
                        }
                    }
                }
			}
			return members;
		}
		private AdGroupModel _FindGroup(string filter)
		{
			var search = new DirectorySearcher(new DirectoryEntry())
			{
				PageSize = 1000,
				Filter = filter
			};

			//make sure only needed attribute is sent back
			foreach (var item in AttributeMap.Group.Map)
			{
				search.PropertiesToLoad.Add(item.Value);
			}
			string attr_member = AttributeMap.Group.Map.TryGetValue(AttributeMap.Group.Member_DistinguishedName);
			search.PropertiesToLoad.Remove(attr_member);

			AdGroupModel model = null;

			using (var results = search.FindAll())
			{
				foreach (SearchResult result in results)
				{
					model = new AdGroupModel();

					string attr = AttributeMap.Group.Map.TryGetValue(AttributeMap.Group.DistinguishedName);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						model.DistinguishedName = (string)result.Properties[attr][0];
					}

					attr = AttributeMap.Group.Map.TryGetValue(AttributeMap.Group.Name);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						model.Name = (string)result.Properties[attr][0];
					}

					attr = AttributeMap.Group.Map.TryGetValue(AttributeMap.Group.Mail);
					if (!string.IsNullOrEmpty(attr) && result.Properties.Contains(attr))
					{
						model.Mail = (string)result.Properties[attr][0];
					}
				}
			}
			return model;
		}
	}
}
