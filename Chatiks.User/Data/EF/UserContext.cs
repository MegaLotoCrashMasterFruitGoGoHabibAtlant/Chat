using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.User.Data.EF;

public class UserContext : IdentityDbContext<Domain.User, IdentityRole<long>, long>
{
    public UserContext()
    {
    }

    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Domain.User>(u =>
        {
            u.OwnsOne(x => x.FullName, fn =>
            {
                fn.OwnsOne(x => x.FirstName, fnf =>
                {
                    fnf.Property(x => x.Value).HasColumnName("FirstName");
                });

                fn.OwnsOne(x => x.LastName, fnl =>
                {
                    fnl.Property(x => x.Value).HasColumnName("LastName");
                });
                
                fn.ToTable("UserFullNames");
            });
        });

        base.OnModelCreating(builder);
    }
}