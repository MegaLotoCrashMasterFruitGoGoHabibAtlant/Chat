using Chatiks.Chat.Data.EF;
using Chatiks.Chat.DomainApi;
using Chatiks.Chat.DomainApi.Interfaces;
using Chatiks.Tools.DI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatiks.Chat;

public class DIManager: IDiManager
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var connStr = Environment.GetEnvironmentVariable("EF_CORE_CHAT_DB") ?? configuration.GetConnectionString("ChatContext");
        
        services.AddDbContext<ChatContext>(options =>
        {
            options.UseNpgsql(connStr);
        });
        
        services.AddScoped<IChatMessagesStore, ChatMessagesStore>();
    }

    public async Task OnAppStartedActions(IServiceCollection services, IConfiguration configuration)
    {
        var provider = services.BuildServiceProvider();
        var scope = provider.CreateScope();
        var chatContext = scope.ServiceProvider.GetService<ChatContext>();

        if ((await chatContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await chatContext.Database.MigrateAsync();
        }
    }
}