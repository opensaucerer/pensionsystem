using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PensionSystem.Application.Behaviors;
using System.Reflection;

namespace PensionSystem.Application;

/// <summary>
/// Registers Application layer services: MediatR handlers, FluentValidation validators,
/// and the validation pipeline behavior.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
