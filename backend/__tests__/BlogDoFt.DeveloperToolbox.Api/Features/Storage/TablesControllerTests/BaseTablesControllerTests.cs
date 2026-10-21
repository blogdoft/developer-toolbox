using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.TablesControllerTests;

public abstract class BaseTablesControllerTests
{
    protected static readonly Faker Fake = new();

    protected BaseTablesControllerTests()
    {
        var problemFactory = Substitute.For<ProblemDetailsFactory>();
        problemFactory
            .CreateProblemDetails(
                Arg.Any<HttpContext>(),
                Arg.Any<int?>(),
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<string?>())
            .Returns(call => new ProblemDetails { Status = call.ArgAt<int?>(1), Detail = call.ArgAt<string?>(4) });

        Sut = new TablesController(Service)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
            ProblemDetailsFactory = problemFactory,
        };
    }

    protected ITableStorageService Service { get; } = Substitute.For<ITableStorageService>();

    protected TablesController Sut { get; }

    protected static JsonElement Json(string value) => JsonSerializer.SerializeToElement(value);
}
