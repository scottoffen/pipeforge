using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PipeForge.Extensions;
using PipeForge.Metadata;

namespace PipeForge;

[ExcludeFromCodeCoverage]
public static class AddPipelineStepExtensions
{
    /// <summary>
    /// Registers the specified pipeline step using its implemented <see cref="IPipelineStep{TContext}"/> interface.
    /// This overload is ideal when no custom interface is used for the step.
    /// </summary>
    /// <typeparam name="TStep">The concrete pipeline step type implementing <see cref="IPipelineStep{TContext}"/>.</typeparam>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="lifetime">The desired service lifetime. Defaults to <see cref="ServiceLifetime.Transient"/>.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddPipelineStep<TStep>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TStep : class, IPipelineStep
    {
        var typeDescriptor = StepTypeDescriptor.Create(typeof(TStep));
        return services.AddPipelineStep(typeDescriptor, lifetime);
    }

    /// <summary>
    /// Registers the specified pipeline step using the provided step interface.
    /// This overload allows registration using a custom step interface derived from <see cref="IPipelineStep{TContext}"/>.
    /// </summary>
    /// <typeparam name="TStep">The concrete pipeline step type.</typeparam>
    /// <typeparam name="TStepInterface">The interface implemented by <typeparamref name="TStep"/>, derived from <see cref="IPipelineStep{TContext}"/>.</typeparam>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="lifetime">The desired service lifetime. Defaults to <see cref="ServiceLifetime.Transient"/>.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddPipelineStep<TStep, TStepInterface>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TStep : class, TStepInterface
        where TStepInterface : class, IPipelineStep
    {
        var typeDescriptor = StepTypeDescriptor.Create<TStepInterface>(typeof(TStep));
        return services.AddPipelineStep(typeDescriptor, lifetime);
    }

    private static IServiceCollection AddPipelineStep(this IServiceCollection services, StepTypeDescriptor typeDescriptor, ServiceLifetime lifetime)
    {
        services.RegisterStep(typeDescriptor, lifetime, services.GetLogger());
        return services;
    }
}
