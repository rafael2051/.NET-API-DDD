using DesafioNEGOCIE.Application.Services.RegistrationCep;
using DesafioNEGOCIE.Application.Services.RequisitionCep;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioNEGOCIE.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRegistrationCepService, RegistrationCepService>();
        services.AddScoped<IRequisitionCepService, RequisitionCepService>();
        return services;
    }
}