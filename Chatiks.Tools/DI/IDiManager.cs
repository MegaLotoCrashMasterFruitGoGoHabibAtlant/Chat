using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatiks.Tools.DI;

public interface IDiManager
{
    void Register(IServiceCollection services, IConfiguration configuration);
    
    Task OnAppStartedActions(IServiceCollection services, IConfiguration configuration);
}