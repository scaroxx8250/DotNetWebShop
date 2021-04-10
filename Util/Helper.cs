using ASPDotNetShoppingCart.Data;
using ASPDotNetShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Util
{
    public class Helper
    {
        public Helper() { }

        // Called in Startup
        public static AppData InitAppData()
        {
            // Instantiation
            AppData appData = new AppData();

            // Fill up data by calling the methods "AddUsers" and "AddProducts"
            AddUsers(appData.Users);
            AddProducts(appData.Products, "ProductDetails.data");

            return appData;
        }

        public static void AddUsers(List<User> users)
        {
            string[] names = { "john", "mary" };

            if (users == null)
                return;

            foreach (string name in names)
            {
                User user = new User()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Username = name,
                    Password = name
                };
                users.Add(user);
            }
        }

        public static void AddProducts(List<Product> products, string filename)
        {
            if (products == null)
                return;

            string[] lines = File.ReadAllLines("SeedData" + "/" + filename);

            foreach (string line in lines)
            {
                string[] quartet = line.Split(";");
                if (quartet.Length != 4)
                    continue; // not what we expected; skip

                Product item = new Product()
                {
                    productName = quartet[0],
                    price = Convert.ToDouble(quartet[1]),
                    description = quartet[2],
                    imagePath = quartet[3]
                };

                products.Add(item);
            }
        }
    }
}
