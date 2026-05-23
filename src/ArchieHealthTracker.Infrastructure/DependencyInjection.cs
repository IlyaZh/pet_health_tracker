using ArchieHealthTracker.Domain.Interfaces.Repositories;
using ArchieHealthTracker.Infrastructure.Data;
using ArchieHealthTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArchieHealthTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'Database' not found.");
        }

        var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(connectionString, serverVersion, mysqlOptions =>
            {
                mysqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null
                );
            });
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWeightRepository, WeightRepository>();
        services.AddScoped<IHygieneRepository, HygieneRepository>();
        services.AddScoped<ISymptomRepository, SymptomRepository>();
        services.AddScoped<IMedicalEventRepository, MedicalEventRepository>();


        return services;
    }
}