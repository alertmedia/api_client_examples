using System;
using System.IO;
using AlertMedia.Client;

namespace AlertMedia.Example
{
    class Program
    {
        private static AlertMediaClient _client;

        static void Main(string[] args) {
 
            _client = new AlertMediaClient(
                clientId:"00000000-0000-0000-0000-000000000000", 
                clientSecretKey:"00000000000000000000"
            );
            
            /* 
             * Uncomment any the following examples to try out the client
             */
            #region Examples
            // ListUsers();
            // SetApiUserPhoto("sample.png");
            #endregion

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void ListUsers() {
            foreach (var user in _client.Users.List(_client.ApiCustomer.Id))
                Console.WriteLine($"{user.Id}: {user.FirstName} {user.LastName}");
        }

        static void SetApiUserPhoto(string path) {
            using (var stream = new FileStream(path, FileMode.Open))
                _client.Users.SetPhoto(_client.ApiUser.Id, stream);
        }

    }
}
