using System;
using System.Web;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Text;

namespace alertmedia
{
	public class BaseClient
	{
		private Authenticate auth = null;

		public BaseClient ()
		{
		}

		public void setAuthenticate(Authenticate auth) {
			this.auth = auth;
		}

		protected  WebClient getWebClient() {
			try {
				WebClient client = new WebClient();
				string client_id_key = auth.getClientId() + ":" + auth.getClientKey();
				string token = "Basic " + Utils.Base64Encode(client_id_key);
				client.Headers.Add("Authorization", token);
				client.Encoding = System.Text.Encoding.UTF8;
				return client;
			} catch (Exception ex) {
				throw ex;
			}
		}

		protected  Hashtable performGet(string concatUrl, Hashtable args) {
			try {
				string url = auth.getBaseUrl();
				url = url + concatUrl;
				using (var client = getWebClient()) {
                    client.Headers.Set("Content-Type", "application/json");
                    string responseString =client.DownloadString(url);
					Hashtable response = new Hashtable ();
					response.Add("status", (int)HttpStatusCode.OK);
					response.Add("data", responseString);
					return response;
				} 
			} catch (WebException ex) {
				var responseString = new StreamReader(((HttpWebResponse)ex.Response).GetResponseStream()).ReadToEnd();
				var statusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
				Hashtable response = new Hashtable ();
				response.Add("status", statusCode);
				response.Add("data", responseString);
				return response;
			} catch (Exception ex) {
				throw ex;
			}			 
		}

		protected  Hashtable performPost(string concatUrl,Hashtable args,string httpmethod = "POST") {
			try {
				string url = auth.getBaseUrl();
				url = url + concatUrl;
				using (var client = getWebClient()) {
                    client.Headers.Set("Content-Type", "application/json");
                    var serializer = new JavaScriptSerializer();
					var json = serializer.Serialize(args);
					string responseString = client.UploadString(url,httpmethod,json);
					Hashtable response = new Hashtable ();
					response.Add("status", (int)HttpStatusCode.OK);
					response.Add("data", responseString);
					return response;
				} 
			} catch (WebException ex) {
				var responseString = new StreamReader(((HttpWebResponse)ex.Response).GetResponseStream()).ReadToEnd();
				var statusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
				Hashtable response = new Hashtable ();
				response.Add("status", statusCode);
				response.Add("data", responseString);
				return response;
			} catch (Exception ex) {
				throw ex;
			}
		}

		protected Hashtable uploadCSVData(string concatUrl, string csvStr) {
			try {
				string url = auth.getBaseUrl();
				url = url + concatUrl;
				using (var client = getWebClient()) {
                    byte[] postArray = Encoding.ASCII.GetBytes(csvStr);
                    client.Headers.Add("Content-Type", "text/csv");
                    byte[] responseArray = client.UploadData(url,postArray);
					Hashtable response = new Hashtable ();
					response.Add("status", (int)HttpStatusCode.OK);
					response.Add("data", System.Text.Encoding.ASCII.GetString(responseArray));
					return response;
				} 
			} catch (WebException ex) {
				var responseString = new StreamReader(((HttpWebResponse)ex.Response).GetResponseStream()).ReadToEnd();
				var statusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
				Hashtable response = new Hashtable ();
				response.Add("status", statusCode);
				response.Add("data", responseString);
				return response;
			} catch (Exception ex) {
				throw ex;
			}

		}

	}
}

