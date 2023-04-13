using System;
using System.Linq;
using System.Threading.Tasks;
using Chatiks.Core.Data.EF;
using Chatiks.Tools.DI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatiks.Core;

public class DiManager: IDiManager
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("EF_CORE_CORE_DB") ?? configuration.GetConnectionString("CoreContext");
        
        services.AddDbContext<CoreContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
    }

    public async Task OnAppStartedActions(IServiceCollection services, IConfiguration configuration)
    {
        var provider = services.BuildServiceProvider();
        var scope = provider.CreateScope();
        var coreContext = scope.ServiceProvider.GetService<CoreContext>();

        if ((await coreContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await coreContext.Database.MigrateAsync();
        }
    }
}