using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SeidoDbWebApiConsumer.Models;
using SeidoDbWebApiConsumer.Services;

namespace SeidoDbWebApiConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new SeidoDbHttpService(new Uri("https://localhost:5001"));
//            var service = new SeidoDbHttpService(new Uri("http://localhost:5000"));

            QueryDatabaseAsync(service).Wait();
        }


        private static async Task QueryDatabaseAsync(SeidoDbHttpService service)
        {
            Console.WriteLine("Query Database");
            Console.WriteLine("--------------");

            Console.WriteLine("Testing GetCustomersAsync()");
            var customers = await service.GetCustomersAsync();
            Console.WriteLine($"Nr of Customers: {customers.Count()}");
            Console.WriteLine($"First 10 Customers:");
            customers.Take(10).ToList().ForEach(c => Console.WriteLine(c));

            Console.WriteLine("\nTesting GetCustomerAsync()");
            Console.WriteLine("Read the first customer using Guid");
            var cust1 = await service.GetCustomerAsync(customers.First().CustomerID);
            Console.WriteLine(cust1);

            Console.WriteLine("\nTesting CreateCustomerAsync()");
            Customer NewCust1 = new Customer();
            var NewCust2 = await service.CreateCustomerAsync(NewCust1);
            Console.WriteLine("Created Customer:");
            Console.WriteLine(NewCust1);

            var NewCust3 = await service.GetCustomerAsync(NewCust2.CustomerID);
            if(NewCust1 == (Customer) NewCust2 && NewCust1 == (Customer) NewCust3)
                Console.WriteLine("Readback customer Equal");
             else
                Console.WriteLine("ERROR: Readback customer not equal");


            Console.WriteLine("\nTesting UpdateCustomerAsync()");
            NewCust1.FirstName += "_Updated";
            NewCust1.LastName += "_Updated";

            var UpdatedCustomer1 = await service.UpdateCustomerAsync(NewCust1);
            Console.WriteLine($"Created Customer with updated names.\n{UpdatedCustomer1}");

            var UpdatedCustomer2 = await service.GetCustomerAsync(UpdatedCustomer1.CustomerID);
            Console.WriteLine($"Readback Customer with updated names.\n{UpdatedCustomer2}");


            Console.WriteLine("\nTesting DeleteCustomerAsync()");
            var DelCust1 = await service.DeleteCustomerAsync(NewCust1.CustomerID);

            Console.WriteLine($"Customer to delete.\n{NewCust1}");
            Console.WriteLine($"Deleted Customer.\n{DelCust1}");

            if (DelCust1 != null && (Customer) DelCust1 == NewCust1)
                Console.WriteLine("Customer Equal");
            else
                Console.WriteLine("ERROR: Customers not equal");

            var DelCust2 = await service.GetCustomerAsync(DelCust1.CustomerID);
            if (DelCust2 != null)
                Console.WriteLine("ERROR: Customer not removed");
            else
                Console.WriteLine("Customer confirmed removed from Db");

            Console.WriteLine();
        }
    }
}
