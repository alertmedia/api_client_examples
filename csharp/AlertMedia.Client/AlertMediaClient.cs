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
            var username = clientId ?? Environment.GetEnvironmentVariable("AM_CLIENT_ID");
            if (username == null)
                throw new ArgumentNullException(nameof(clientId), "You must specify an AlertMedia API clientId and clientSecretKey.");

            var password = clientSecretKey ?? Environment.GetEnvironmentVariable("AM_CLIENT_SECRET_KEY");
            if (password == null)
                throw new ArgumentNullException(nameof(clientSecretKey), "You must specify an AlertMedia API clientId and clientSecretKey.");

            Configuration = new Configuration {
                ApiClient = GetApiClient(server),
                Username = username,
                Password = password,
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
