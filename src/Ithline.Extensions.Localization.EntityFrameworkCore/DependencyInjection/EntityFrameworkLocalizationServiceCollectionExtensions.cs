using System.Diagnostics.CodeAnalysis;
using Ithline.Extensions.Localization.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using static Ithline.Extensions.Localization.EntityFrameworkCore.LinkerFlags;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up EF Core string localizer in an <see cref="IServiceCollection" />.
/// </summary>
public static class EntityFrameworkLocalizationServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="DbContext" /> string localizer services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <typeparam name="TContext">The type of the <see cref="DbContext" />.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDbContextLocalization<[DynamicallyAccessedMembers(ContextFlags)] TContext>(this IServiceCollection services)
        where TContext : DbContext, IStringResourceDbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

        services.TryAddSingleton<EFStringLocalizer<TContext>>();
        services.TryResolveAsSingleton<IStringLocalizerFactory, EFStringLocalizer<TContext>>();
        services.TryResolveAsSingleton<IStringResourceChangeToken<TContext>, EFStringLocalizer<TContext>>();

        return services;
    }

    private static void TryResolveAsSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : TService
    {
        services.TryAddSingleton<TService>(sp => sp.GetRequiredService<TImplementation>());
    }
}
