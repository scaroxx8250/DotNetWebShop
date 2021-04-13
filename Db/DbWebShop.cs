using ASPDotNetShoppingCart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Db
{
    public class DbWebShop:DbContext
    {
        protected IConfiguration configuration;
        public DbWebShop(DbContextOptions<DbWebShop> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder model)
        {
            //set Name column in Product table as unique
            model.Entity<Product>().HasIndex(x => x.ProductName).IsUnique();

          
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseHistory> PurchasedHistory { get; set; }
        public DbSet<PurchasedItems> PurchasedItems { get; set; }
 
    }
}
