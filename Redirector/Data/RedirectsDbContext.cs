using Microsoft.EntityFrameworkCore;

namespace Redirector;

public class RedirectsDbContext(DbContextOptions<RedirectsDbContext> options) : DbContext(options)
{
    public DbSet<SmartLinkDescription> SmartLinkDescription { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SmartLinkDescription>()
            .HasKey(e => e.LinkPath);
    }
}