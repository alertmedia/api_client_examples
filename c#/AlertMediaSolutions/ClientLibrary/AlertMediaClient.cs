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
	public class AlertMediaClient
	{
		private static string baseUrl = null;
		private static string clientId = null;
		private static string clientKey = null;

		public AlertMediaClient() {
		}

		public static void setBaseUrl(string _baseUrl) {
			baseUrl = _baseUrl;
		}

		public static void setClientId(string _clientId) {
			clientId = _clientId;
		}

		public static void setClientKey(string _clientKey) {
			clientKey = _clientKey;
		}
								
		public static UserClient User { 
			get { 
				UserClient client =  UserClient.Instance; 
				client.setAuthenticate(new Authenticate(baseUrl, clientId, clientKey));
				return client;
			} 
		}

		public static GroupClient Group {
			get {
				GroupClient client = GroupClient.Instance;
				client.setAuthenticate(new Authenticate(baseUrl, clientId, clientKey));
				return client;
			}
		}

        public static CustomerClient Customer
        {
            get
            {
                CustomerClient client = CustomerClient.Instance;
                client.setAuthenticate(new Authenticate(baseUrl, clientId, clientKey));
                return client;
            }
        }



    }//Class Ends here..
}