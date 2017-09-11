using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AlertMedia.Client;
using AlertMedia.CodeGen.Client;

namespace AlertMedia.Example
{
    class Program
    {
        static void Main(string[] args) {

            var client = new AlertMediaClient(
                clientId:"00000000-0000-0000-0000-000000000000", 
                clientSecretKey:"00000000000000000000"
            );

            foreach (var user in client.Users.List(client.ApiCustomer.Id))
                Console.WriteLine($"{user.FirstName} {user.LastName}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
