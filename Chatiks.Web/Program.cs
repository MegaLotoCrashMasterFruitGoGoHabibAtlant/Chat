using System;
using System.Linq;
using System.Reflection;
using Chatiks;
using Chatiks.Extensions;
using Chatiks.GraphQL;
using Chatiks.Hubs;
using Chatiks.Tools.DI;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<HttpContextAccessor>();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.ConfigureGraphQl<ErrorFilter>();

builder.Services.Configure<HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = null;
});

var diManagers =  AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(a => a.GetTypes())
    .Where(t => typeof(IDiManager).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
    .Select(x => (IDiManager)Activator.CreateInstance(x))
    .ToArray();

foreach (var diManager in diManagers)
{
    diManager?.Register(builder.Services, builder.Configuration);
}

builder.Services.AddAuthorization();

var config = new TypeAdapterConfig();
MapsterConfigure.Configure(config);
builder.Services.AddSingleton(config);

var app = builder.Build();

foreach (var diManager in diManagers)
{
    await diManager?.OnAppStartedActions(builder.Services, builder.Configuration);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.MapGraphQL();
app.MapHub<MessengerHub>("/hub/messengerHub");

app.Run();