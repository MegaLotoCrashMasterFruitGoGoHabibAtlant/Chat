using System;
using System.Linq;
using System.Reflection;
using HotChocolate;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Chatiks.Extensions;

public static class GraphQlExtensions
{
    public static IRequestExecutorBuilder ConfigureGraphQl<TErrorFilter>(this IServiceCollection serviceCollection)
        where TErrorFilter: class, IErrorFilter
    {
        var server = serviceCollection.AddGraphQLServer()
            .AddErrorFilter<TErrorFilter>()
            .AddAuthorization();

        var queryExtensions = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<ExtendObjectTypeAttribute>() != null)
            .ToArray();

        foreach (var type in queryExtensions)
        {
            server = server.AddTypeExtension(type);
        }

        server = server
            .BindRuntimeType<DateTime, DateType>()
            .AddQueryType(d => d.Name("Query"))
            .AddMutationType(d => d.Name("Mutation"));
        
        return server;
    }
}