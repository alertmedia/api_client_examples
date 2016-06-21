using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Web.Script.Serialization;


namespace alertmedia
{
    public sealed class CustomerClient : BaseClient
    {
        private static readonly Lazy<CustomerClient> lazy =
            new Lazy<CustomerClient>(() => new CustomerClient());

        public static CustomerClient Instance { get { return lazy.Value; } }

        private CustomerClient(): base() {
        }

        public string getCustomerId()
        {
            try
            {
                string url = "customers2";                    
                Hashtable response= this.performGet(url, new Hashtable());
                string jsonString = response["data"].ToString();
                JavaScriptSerializer js = new JavaScriptSerializer();
                Customer[] customers = js.Deserialize<Customer[]>(jsonString);
                return customers[0].id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class Customer
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
