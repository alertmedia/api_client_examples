using System;
using AlertMedia.CodeGen.Api;
using AlertMedia.CodeGen.Client;
using AlertMedia.CodeGen.Model;

namespace AlertMedia.Client
{
    public class AlertMediaClient
    {
        private IUserApi _users;
        public IUserApi Users => _users ?? (_users = new UserApi(Configuration));
        
        private IGroupApi _groups;
        public IGroupApi Groups => _groups ?? (_groups = new GroupApi(Configuration));

        public Customer ApiCustomer;
        public User ApiUser;

        public readonly Configuration Configuration;

        public AlertMediaClient(string clientId = null, string clientSecretKey = null, string server = null) {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
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
            var info = new DefaultApi(Configuration).LoginInfo();
            ApiCustomer = info.Customer;
            ApiUser = info.User;
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
