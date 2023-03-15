using Entities.Identity.Models;
using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entities;

public class MasterDbContext : IdentityDbContext<AppUser>
{
    
    
    public MasterDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName != null && tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName[6..]);
            }
        }

        builder.Entity<AppUser>().HasOne(u => u.CardMember)
            .WithOne(c => c.User)
            .HasForeignKey<CardMember>(c => c.UserId);
    }

    public DbSet<AuctionInformation> AuctionInformationS { get; set; }

    public DbSet<CardMember> CardMembers { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Contact> Contacts { get; set; }

    public DbSet<LotProduct> LotProducts { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ProductPhoto> ProductPhotos { get; set; }

}