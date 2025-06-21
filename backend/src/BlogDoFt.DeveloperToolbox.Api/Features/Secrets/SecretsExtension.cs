using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Impl;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Secrets;

public static class SecretsExtension
{
    public static IServiceCollection AddSecretsFeature(this IServiceCollection services) =>
        services
            .AddScoped<ISecretCriptographyService, SecretCriptographyService>()
            .AddScoped<ISecretRepository, SecretRepository>();
}
