using System;
using AlertMedia.CodeGen.Api;
using AlertMedia.CodeGen.Client;

namespace AlertMedia.Client
{
    public class AlertMediaClient
    {
        private IUserApi _users;
        public IUserApi Users => _users ?? (_users = new UserApi(Configuration));
        
        private IGroupApi _groups;
        public IGroupApi Groups => _groups ?? (_groups = new GroupApi(Configuration));

        public readonly Configuration Configuration;

        public AlertMediaClient(string clientId = null, string clientSecretKey = null, string server = null) {
            Configuration = new Configuration {
                ApiClient = GetApiClient(server),
                Username = clientId
                           ?? System.Environment.GetEnvironmentVariable("AM_CLIENT_ID")
                           ?? throw new ArgumentNullException(nameof(clientId), "You must specify an AlertMedia API clientId and clientSecretKey."),
                Password = clientSecretKey
                           ?? System.Environment.GetEnvironmentVariable("AM_CLIENT_SECRET_KEY")
                           ?? throw new ArgumentNullException(nameof(clientSecretKey), "You must specify an AlertMedia API clientId and clientSecretKey.")
            };
        }

        private ApiClient GetApiClient(string address) {
            if (string.IsNullOrWhiteSpace(address))
                return new ApiClient();
            if (address.IndexOf("http", StringComparison.InvariantCultureIgnoreCase) < 0)
                address = $"https://{address}";
            address = $"{new Uri(address).AbsoluteUri}api";
            return new ApiClient(address);
        }
    }
}
