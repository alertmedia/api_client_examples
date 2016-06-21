using System;
using alertmedia;
using System.Collections;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace TestClient
{
	public class Test {
		
		public static void Main (string[] args) {

			AlertMediaClient.setBaseUrl (ConfigurationManager.AppSettings["BaseUrl"]);
			AlertMediaClient.setClientId (ConfigurationManager.AppSettings["ClientID"]);
			AlertMediaClient.setClientKey (ConfigurationManager.AppSettings["ClientKey"]);

            string customerId = AlertMediaClient.Customer.getCustomerId();

            

            Console.ReadLine();

            // Create Group
            /*
                        Hashtable args2 = new Hashtable ();
                        args2.Add ("name","Nag group");
                        args2.Add ("customer","3");
                        args2.Add ("description","This is a group created by Nag");
                        Hashtable response = AlertMediaClient.Group.create (args2);
                        foreach(string key in response.Keys) {
                            Console.WriteLine(String.Format("{0}: {1}", key, response[key]));
                        }
            */
            // List all groups for customer 3

            

            // Update group

            //			Hashtable args2 = new Hashtable();
            //			args2.Add("customer","3");
            //			args2.Add ("name", "Nag group 12312312");
            //			args2.Add ("description", "Nag from enfini");
            //			Hashtable response = AlertMediaClient.Group.update ("635", args2);
            //			foreach(string key in response.Keys) {
            //				Console.WriteLine(String.Format("{0}: {1}", key, response[key]));
            //			}

            // Delete group

            //			Hashtable response = AlertMediaClient.Group.delete ("635");
            //			foreach(string key in response.Keys) {
            //				Console.WriteLine(String.Format("{0}: {1}", key, response[key]));
            //			}

            // Get Group Details

            //			Hashtable response = AlertMediaClient.Group.get ("635");
            //			foreach(string key in response.Keys) {
            //				Console.WriteLine(String.Format("{0}: {1}", key, response[key]));
            //			}

            // List users in group

            //			ArrayList args2 = new ArrayList ();
            //			args2.Add ("635");
            //			Hashtable response = AlertMediaClient.Group.listGroupUsers (args2);
            //			foreach(string key in response.Keys) {
            //				Console.WriteLine(String.Format("{0}: {1}", key, response[key]));
            //			}

            // Add user to group

            //			ArrayList args2 = new ArrayList ();
            //			args2.Add ("29041");
            //			Hashtable response = AlertMediaClient.Group.addUsersToGroup ("635", args2);
            //			foreach(string key in response.Keys) {
            //				Console.WriteLine(String.Format("{0}: {1}", key, response[key]));
            //			}

            // Delete users from group

            //			ArrayList args2 = new ArrayList ();
            //			args2.Add ("29041");
            //			Hashtable response = AlertMediaClient.Group.deleteUsersFromGroup ("635", args2);
            //			foreach(string key in response.Keys) {
            //				Console.WriteLine(String.Format("{0}: {1}", key, response[key]));
            //			}

        }
	}
}

