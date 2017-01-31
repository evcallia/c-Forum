using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace forum.Models
{
    public class Context : IdentityDbContext<User>
    {
        public Context(DbContextOptions options) : base(options) {}

        new public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories {get; set;}
        public DbSet<User_Cateogry> User_Cateogries {get; set;}
        public DbSet<Topic> Topics {get; set;}
        public DbSet<Comment> Comments {get; set;}
        
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);

        //     //Can specify how we want relationshaps to look here

        //     // modelBuilder.Entity<AddedProduct>()
        //     //     .HasOne(u => u.user)
        //     //     .WithMany(ap => ap.addedProducts);

        //     // modelBuilder.Entity<AddedProduct>()
        //     //     .HasOne(p => p.product)
        //     //     .WithMany(ap => ap.addedProducts);

        // }
    }
}