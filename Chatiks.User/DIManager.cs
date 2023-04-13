using System;
using System.Linq;
using System.Threading.Tasks;
using Chatiks.Tools.DI;
using Chatiks.User.Data.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatiks.User;

public class DiManager : IDiManager
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var connStr = Environment.GetEnvironmentVariable("EF_CORE_USER_DB") ??
                      configuration.GetConnectionString("UserContext");

        if (string.IsNullOrEmpty(connStr))
        {
            throw new Exception(
                "Provide connection string for UserContext in appsettings.json or set EF_CORE_USER_DB environment variable");
        }

        services.AddDbContext<UserContext>(o =>
        {
            o.UseNpgsql(connStr);
        });

        services.AddIdentity<Domain.User, IdentityRole<long>>()
            .AddEntityFrameworkStores<UserContext>()
            .AddDefaultTokenProviders();
        
        services.Configure<IdentityOptions>(o =>
        {
            o.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/";
            o.Password.RequireDigit = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Lockout = new LockoutOptions
            {
                MaxFailedAccessAttempts = 100000
            };
            o.SignIn.RequireConfirmedEmail = false;
        });
    }

    public async Task OnAppStartedActions(IServiceCollection services, IConfiguration configuration)
    {
        var provider = services.BuildServiceProvider();
        var scope = provider.CreateScope();
        
        var userContext = scope.ServiceProvider.GetService<UserContext>();
        
        if((await userContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await userContext.Database.MigrateAsync();
        }
    }
}