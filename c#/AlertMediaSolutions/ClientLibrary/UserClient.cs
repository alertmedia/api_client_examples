using System;
using System.Web;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Configuration;

namespace alertmedia
{
	public sealed class UserClient : BaseClient
	{
		private static readonly Lazy<UserClient> lazy =
			new Lazy<UserClient>(() => new UserClient());

		public static UserClient Instance { get { return lazy.Value; } }

		private UserClient(): base() {
		}

		public Hashtable get(string userId) {
			try {
				string id = userId.Trim();
				string userUrl = "users/"+id;
				return this.performGet(userUrl,new Hashtable());
			} catch (Exception ex) {
				throw ex;
			}
		}

		public Hashtable list(Hashtable args) {
			try {				
				string userUrl = "users?"+Utils.QueryString(args);
				return this.performGet(userUrl,new Hashtable());
			} catch (Exception ex) {
				throw ex;
			}
		}

		public  Hashtable create(Hashtable args) {
			try {
				if (!args.ContainsKey("first_name") ||  string.IsNullOrEmpty ((string)args["first_name"])){
					throw new System.InvalidOperationException("first_name is required field!");
				}
				if (!args.ContainsKey("last_login") ||  Utils.dateValidate (args["last_login"])){
					throw new System.InvalidOperationException("last_login is required field!");
				}
				if (!args.ContainsKey("date_joined") ||  Utils.dateValidate (args["date_joined"])){
					throw new System.InvalidOperationException("date_joined is required field!");
				}
				// Either mobile_phone or email is required for api
				if (!args.ContainsKey("mobile_phone")  && !args.ContainsKey("email")){
					throw new System.InvalidOperationException("Either of mobile_number or email field is required!");	
				}else if (args.ContainsKey("mobile_phone")  && args.ContainsKey("email")){
					if (string.IsNullOrEmpty ((string) args["mobile_phone"]) && string.IsNullOrEmpty ((string)args["email"])){
						throw new System.InvalidOperationException("Either of mobile_number or email field is required!");
					}	
				}else if(!args.ContainsKey("mobile_phone") && string.IsNullOrEmpty ((string)args["email"]) ){
					throw new System.InvalidOperationException("Email field is empty!");
				}else if(!args.ContainsKey("email") && string.IsNullOrEmpty ((string)args["mobile_phone"]) ){
					throw new System.InvalidOperationException("mobile_phone field is empty!");
				}
				string userUrl = "users";
				return this.performPost(userUrl,args);
			} catch (Exception ex) {
				throw ex;
			}
		}

		public  Hashtable update(string userId,Hashtable args) {
			try {
				if (string.IsNullOrEmpty (userId)){
					throw new System.InvalidOperationException("User Id is required!");
				}
				string userUrl = "users/"+userId;
				return this.performPost(userUrl,args,"PUT");
			} catch (Exception ex) {
				throw ex;
			}
		}

		public  Hashtable delete(string userId) {
			try {
				if (string.IsNullOrEmpty (userId)){
					throw new System.InvalidOperationException("User Id is required!");
				}
				string userUrl = "users/"+userId;
				return this.performPost(userUrl,null,"DELETE");
			} catch (Exception ex){
				throw ex;
			}
		}

		public Hashtable importUserData(string csvContent, string customer) {
			try {				
				string uploadUrl = "import/bulk_update/?customer="+customer;
                Hashtable args = new Hashtable();
                args["data"] = csvContent;
				return this.uploadCSVData(uploadUrl, csvContent);
			} catch(Exception ex) {
				throw ex;
			}
		}

        public Hashtable getUsersToBeDeleted(string customer, ArrayList retainedUserList)
        {
            try
            {
                string missingUrl = "users_missing/?customer=" + customer;
                Hashtable args = new Hashtable();
                args["users"] = retainedUserList;
                return this.performPost(missingUrl, args);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


	}

}

