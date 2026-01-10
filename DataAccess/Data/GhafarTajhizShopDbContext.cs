using DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class GhafarTajhizShopDbContext : IdentityDbContext<User,Role,int>
    {
        public GhafarTajhizShopDbContext(DbContextOptions<GhafarTajhizShopDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Basket> Baskets { get; set; }

        public DbSet<BasketItem> BasketItems { get; set; }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // جلوگیری از تکرار یک محصول در یک سبد
            modelBuilder.Entity<BasketItem>()
                .HasIndex(bi => new { bi.BasketId, bi.ProductId })
                .IsUnique();
        }

    }
    
}
