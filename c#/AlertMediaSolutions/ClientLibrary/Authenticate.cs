using System;

namespace alertmedia
{
	public class Authenticate {

		private string baseUrl = null;
		private string clientId = null;
		private string clientKey = null;

		public Authenticate (string _baseUrl, string _clientId, string _clientKey) {
			this.baseUrl = _baseUrl;
			if (string.IsNullOrEmpty(this.baseUrl)) {
				throw new System.InvalidOperationException("Base Url must be included");
			}
			if(!this.baseUrl.EndsWith("/")) {
				this.baseUrl += "/";
			}

			this.clientId = _clientId;
			if (string.IsNullOrEmpty(this.clientId)) {
				throw new System.InvalidOperationException("ClientId must be included");
			}

			this.clientKey = _clientKey;
			if(string.IsNullOrEmpty(this.clientKey)) {
				throw new System.InvalidOperationException("clientKey must be included");
			}				
		}

		public string getBaseUrl() {
			return baseUrl;
		}

		public string getClientId() {
			return clientId;
		}

		public string getClientKey() {
			return clientKey;
		}

	}
}

