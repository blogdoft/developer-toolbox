using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Impl;
using Bogus;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets.SecretCriptographyServiceTests;

public abstract class BaseSecretCriptographyServiceTests
{
    protected static readonly Faker Fake = new();

    internal SecretCriptographyService Sut { get; } = new();
}
