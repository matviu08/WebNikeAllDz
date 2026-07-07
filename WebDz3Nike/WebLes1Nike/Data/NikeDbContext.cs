using Microsoft.EntityFrameworkCore;
using WebLes1Nike.Data.Entities;

namespace WebLes1Nike.Data;

public class NikeDbContext : DbContext
{
    public NikeDbContext(DbContextOptions<NikeDbContext> options) 
        : base(options)
    { }

    public DbSet<CategoryEntitiy> Categories { get; set; }
}
