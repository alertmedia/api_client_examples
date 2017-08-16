using System;
using AlertMedia.Client;

namespace AlertMedia.Example
{
    class Program
    {
        static void Main(string[] args) {

            var client = new AlertMediaClient(
                clientId:"00000000-0000-0000-0000-000000000000", 
                clientSecretKey:"00000000000000000000",
                server:"staging.alertmedia.com"
            );
            var customerId = 99;

            foreach (var user in client.Users.List(customerId: customerId)) {
                Console.WriteLine($"{user.FirstName} {user.LastName}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
