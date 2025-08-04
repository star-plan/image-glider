using System.Reflection;

namespace ImageGlider.WebApi.Endpoints.Internal;

public static class EndpointExtensions
{
    public static void UseEndpoints<TMarker>(this IEndpointRouteBuilder app)
    {
        UseEndpoints(app, typeof(TMarker));
    }

    private static void UseEndpoints(this IEndpointRouteBuilder app, Type typeMarker)
    {
        var endpointTypes = GetEndpointTypes(typeMarker);
        foreach (var type in endpointTypes)
        {
            type.GetMethod(nameof(IEndpoint.UseEndpoints))?
                .Invoke(null, new object[] { app });
        }
    }

    public static void AddServices<TMarker>(this IServiceCollection services, IConfiguration configuration)
    {
        AddServices(services, typeof(TMarker), configuration);
    }

    private static void AddServices(IServiceCollection services, Type typeMarker, IConfiguration configuration)
    {
        var endpointTypes = GetEndpointTypes(typeMarker);
        foreach (var type in endpointTypes)
        {
            type.GetMethod(nameof(IEndpoint.AddServices))!
                .Invoke(null, new object[] { services, configuration });
        }
    }

    /// <summary>
    ///     获取指定类型标记的所有端点类型
    /// </summary>
    /// <param name="typeMarker">类型标记</param>
    /// <returns></returns>
    private static IEnumerable<TypeInfo> GetEndpointTypes(Type typeMarker)
    {
        var endpointTypes = typeMarker.Assembly.DefinedTypes
            .Where(x => !x.IsAbstract && !x.IsInterface &&
                        typeof(IEndpoint).IsAssignableFrom(x));
        return endpointTypes;
    }
}