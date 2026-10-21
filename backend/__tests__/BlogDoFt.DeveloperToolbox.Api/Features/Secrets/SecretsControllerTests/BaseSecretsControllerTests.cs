using BlogDoFt.DeveloperToolbox.Api.Features.Secrets;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets.SecretsControllerTests;

public abstract class BaseSecretsControllerTests
{
    protected static readonly Faker Fake = new();

    protected BaseSecretsControllerTests()
    {
        Sut = new SecretsController(Cryptography, Repository)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
    }

    protected ISecretCriptographyService Cryptography { get; } = Substitute.For<ISecretCriptographyService>();

    protected ISecretRepository Repository { get; } = Substitute.For<ISecretRepository>();

    protected SecretsController Sut { get; }

    protected void AcceptHeader(string value) => Sut.HttpContext.Request.Headers.Accept = value;
}
