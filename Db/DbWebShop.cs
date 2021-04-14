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

            model.Entity<CartItem>().HasKey(x => new { x.CartId, x.ProductId });
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchasedHistory> PurchasedHistories { get; set; }
        public DbSet<PurchasedItems> PurchasedItems { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }


    }
}
