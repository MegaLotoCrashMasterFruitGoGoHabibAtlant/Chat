using System;
using System.Linq;
using Chatiks.Tools.Mapster;
using Mapster;

namespace Chatiks;

public class MapsterConfigure
{
    public static void Configure(TypeAdapterConfig config)
    {
        var configurators = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IMapsterConfurator).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .Select(t => (IMapsterConfurator)Activator.CreateInstance(t))
            .ToArray();
        
        foreach (var mapsterConfurator in configurators)
        {
            mapsterConfurator.Configure(config);
        }
    }
}