using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace alertmedia
{
	public sealed class GroupClient : BaseClient
	{
		private static readonly Lazy<GroupClient> lazy =
			new Lazy<GroupClient>(() => new GroupClient());

		public static GroupClient Instance { get { return lazy.Value; } }

		private GroupClient(): base() {
		}

		public Hashtable get(string groupId) {
			try {
				groupId = groupId.Trim();
				string groupUrl =  "groups/" + groupId;
				return this.performGet(groupUrl, new Hashtable());
			} catch(Exception ex) {
				throw ex;
			}
		}

		public Hashtable list(Hashtable args) {
			try {
                string groupUrl = "";
                if (args.Count > 0)
                {
                    groupUrl = "groups?" + Utils.QueryString(args);
                }else
                {
                    groupUrl = "groups";
                }
				Hashtable response = this.performGet(groupUrl, new Hashtable());
                string jsonString = response["data"].ToString();
                JavaScriptSerializer js = new JavaScriptSerializer();
                Group[] groups = js.Deserialize<Group[]>(jsonString);
                response["data"] = groups;
                return response;
            } catch (Exception ex) {
				throw ex;
			}
		}

		public  Hashtable create(Hashtable args) {
			try {
				if (!args.ContainsKey("name") ||  string.IsNullOrEmpty ((string)args["name"])){
					throw new System.InvalidOperationException("Group name is a required field!");
				}
				if (!args.ContainsKey("customer") ||  string.IsNullOrEmpty ((string)args["customer"])){
					throw new System.InvalidOperationException("Customer is a required field!");
				}
				string groupUrl = "groups";
				return this.performPost(groupUrl, args);
			} catch (Exception ex) {
				throw ex;
			}
		}

		public  Hashtable update(String groupId, Hashtable args) {
			try {
				string groupUrl = "groups/" + groupId;
				return this.performPost(groupUrl, args, "PUT");
			} catch (Exception ex) {
				throw ex;
			}
		}

		public  Hashtable delete(String groupId) {
			try {
				string groupUrl =  "groups/" + groupId;
				return this.performPost(groupUrl, null, "DELETE");
			} catch (Exception ex) {
				throw ex;
			}
		}

		public Hashtable listGroupUsers(ArrayList groups) {
			string groupIdList = "";
			for (int i = 0; i < groups.Count; i++) {
				if (groupIdList == "") {
					groupIdList = groups [i].ToString();
				} else {					
					groupIdList = groupIdList + "," + groups [i].ToString();
				}
			}
			string groupUrl = "users?groups="+groupIdList;
			return this.performGet(groupUrl, new Hashtable());
		}

		public Hashtable addUsersToGroup(string groupId, ArrayList users) {
			string userIdList = "";
			for (int i = 0; i < users.Count; i++) {
				if (userIdList == "") {
					userIdList = users [i].ToString();
				} else {
					userIdList = userIdList + "," + users [i].ToString();
				}
			}
			string groupUrl = "groups/"+groupId+"/users?id="+userIdList;
			return this.performPost(groupUrl, new Hashtable());
		}

		public Hashtable deleteUsersFromGroup(string groupId, ArrayList users) {
			string userIdList = "";
			for (int i = 0; i < users.Count; i++) {
				if (userIdList == "") {
					userIdList = users [i].ToString();
				} else {
					userIdList = userIdList + "," + users [i].ToString();
				}
			}
			string groupUrl = "groups/"+groupId+"/users?id="+userIdList;
			return this.performPost(groupUrl, new Hashtable(), "DELETE");
		}
	}

    public class Group
    {
        public string id { get; set; }
        public string name { get; set; }
    }

}

