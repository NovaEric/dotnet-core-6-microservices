using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                ProductName = "PolloPica",
                ProductPrice = 12,
                ProductDescription = "Fry chicken Dominican style",
                ImageUrl = "https://res.cloudinary.com/dyej4hpgt/image/upload/v1646843779/heroes/marvel-wolverine.jpg",
                ProductCategoryName = "Entree"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 2,
                ProductName = "MangoChimi",
                ProductPrice = 6,
                ProductDescription = "Mango Dominican style",
                ImageUrl = "https://res.cloudinary.com/dyej4hpgt/image/upload/v1646843779/heroes/marvel-thor.jpg",
                ProductCategoryName = "Appetizer"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 3,
                ProductName = "Mango Shake",
                ProductPrice = 5,
                ProductDescription = "Mango shake Dominican style",
                ImageUrl = "https://res.cloudinary.com/dyej4hpgt/image/upload/v1646843778/heroes/marvel-spider.jpg",
                ProductCategoryName = "Appetizer"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 4,
                ProductName = "Mango & friends",
                ProductPrice = 12,
                ProductDescription = "Mixed Fruits",
                ImageUrl = "https://res.cloudinary.com/dyej4hpgt/image/upload/v1646843777/heroes/marvel-iron.jpg",
                ProductCategoryName = "Appetizer"
            });
        }
    }
}
