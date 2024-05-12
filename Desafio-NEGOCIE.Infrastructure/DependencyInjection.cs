using DesafioNEGOCIE.Domain.Entities;
using DesafioNEGOCIE.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioNEGOCIE.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                                        ConfigurationManager configuration)
    {
        services.Configure<ConnectionSettings>(configuration.GetSection(ConnectionSettings.ConnectionSettingsName));
        services.AddScoped<IEnderecoRepository, EnderecoRepository>();

        return services;
    }
}