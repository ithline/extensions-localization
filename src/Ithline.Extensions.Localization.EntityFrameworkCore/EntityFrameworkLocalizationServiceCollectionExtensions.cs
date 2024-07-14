using Ithline.Extensions.Localization.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Microsoft.Extensions.DependencyInjection;

public static class EntityFrameworkLocalizationServiceCollectionExtensions
{
    public static IServiceCollection AddDbContextLocalization<TContext>(this IServiceCollection services)
        where TContext : DbContext, IStringLocalizationDbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMemoryCache();
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        services.TryAddSingleton<IStringLocalizerFactory, EntityFrameworkStringLocalizer<TContext>>();

        return services;
    }
}