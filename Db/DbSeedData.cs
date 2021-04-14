using ASPDotNetShoppingCart.Data;
using ASPDotNetShoppingCart.Db;
using ASPDotNetShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Util
{
    public class DbSeedData
    {
        private readonly DbWebShop db;
        public DbSeedData(DbWebShop db) {

            this.db = db;
        }

        // Called in Startup
        public void Init()
        {
            // Instantiation
            //AppData appData = new AppData();

            // Fill up data by calling the methods "AddUsers" and "AddProducts"
          
            AddUsers();
            AddProducts("ProductDetails.data");
           // AddProducts(appData.Products, "ProductDetails.data");
           

    }

        protected void AddUsers()
        {
            string[] names = { "john", "mary" };
            string[] pwd = { "cherwah", "tin" };
            for (int i = 0; i < names.Length; i++)
            {
                db.Users.Add(new User
                {
                    Username = names[i],
                    Password = pwd[i],
                }) ;
            }

            db.SaveChanges();
        }
     



        protected void AddProducts(string filename)
        {
            string[] lines = File.ReadAllLines("SeedData" + "/" + filename);

            foreach (string line in lines)
            {
                string[] sixth = line.Split(";");
                if (sixth.Length != 5)
                    continue; // not what we expected; skip

               db.Products.Add(new Product
                {
                    ProductName = sixth[0],
                    Price = Convert.ToDouble(sixth[1]),
                    Description = sixth[2],
                    ImagePath = sixth[3],
                    DownloadLink = sixth[4]
                });
            }
            db.SaveChanges();
        }

    }
}
