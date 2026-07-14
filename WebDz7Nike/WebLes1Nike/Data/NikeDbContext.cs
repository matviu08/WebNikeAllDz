using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebLes1Nike.Data.Entities;
using WebLes1Nike.Data.Entities.Identity;

namespace WebLes1Nike.Data;

public class NikeDbContext : IdentityDbContext<UserEntity, RoleEntity, int>
{
    public NikeDbContext(DbContextOptions<NikeDbContext> options)
        : base(options)
    { }

    public DbSet<CategoryEntitiy> Categories { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ProductImageEntity> ProductImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRoleEntity>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);
        
        modelBuilder.Entity<UserRoleEntity>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UsersRoles)
            .HasForeignKey(ur => ur.RoleId);
    }
}