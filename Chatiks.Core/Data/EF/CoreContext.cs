using Chatiks.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Core.Data.EF;

public class CoreContext : DbContext
{
    public CoreContext()
    {
        
    }
    
    public CoreContext(DbContextOptions<CoreContext> options): base(options)
    {
        
    }
    
    public DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Image>(i =>
        {
            i.HasKey(x => x.Id);
            
            i.Property(x => x.Base64String).IsRequired();
            
            i.Property(x => x.Width).IsRequired();
            
            i.Property(x => x.Height).IsRequired();
        });

        base.OnModelCreating(builder);
    }
}