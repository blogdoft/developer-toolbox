using Azure;
using Azure.Messaging.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus;

public class NamespaceExceptionFilterTests
{
    private readonly NamespaceExceptionFilter _sut = new();

    public static TheoryData<Exception, int, string> HandledExceptions => new()
    {
        { new NamespaceOperationNotSupportedException("create"), 501, "Operation not supported" },
        { new ServiceBusException("x", ServiceBusFailureReason.MessagingEntityNotFound), 404, "Entity not found" },
        { new ServiceBusException("x", ServiceBusFailureReason.MessagingEntityAlreadyExists), 409, "Entity already exists" },
        { new ServiceBusException("x", ServiceBusFailureReason.ServiceTimeout), 502, "Service Bus request failed" },
        { new RequestFailedException(404, "x"), 404, "Resource not found" },
        { new RequestFailedException(409, "x"), 409, "Resource already exists" },
        { new RequestFailedException(400, "x"), 400, "Invalid storage request" },
        { new RequestFailedException(500, "x"), 502, "Storage request failed" },
        { new UnauthorizedAccessException("x"), 403, "Not authorized" },
    };

    public static TheoryData<Exception> KnownExceptions
        => new(HandledExceptions.Select(row => row.Data.Item1));

    [Theory]
    [MemberData(nameof(HandledExceptions))]
    public void Should_SetProblemDetailsResult_When_ExceptionIsKnown(Exception exception, int status, string title)
    {
        var context = CreateContext(exception);

        _sut.OnException(context);

        var result = context.Result.ShouldBeOfType<ObjectResult>();
        result.StatusCode.ShouldBe(status);
        var problem = result.Value.ShouldBeOfType<ProblemDetails>();
        problem.Status.ShouldBe(status);
        problem.Title.ShouldBe(title);
    }

    [Theory]
    [MemberData(nameof(KnownExceptions))]
    public void Should_MarkExceptionAsHandled_When_ExceptionIsKnown(Exception exception)
    {
        var context = CreateContext(exception);

        _sut.OnException(context);

        context.ExceptionHandled.ShouldBeTrue();
    }

    [Fact]
    public void Should_UseFirstMessageLineAsDetail_When_StorageErrorHasRawResponse()
    {
        var exception = new RequestFailedException(404, "The specified container does not exist.\nStatus: 404\nHeaders: ...");
        var context = CreateContext(exception);

        _sut.OnException(context);

        var problem = context.Result.ShouldBeOfType<ObjectResult>().Value.ShouldBeOfType<ProblemDetails>();
        problem.Detail.ShouldBe("The specified container does not exist.");
    }

    [Fact]
    public void Should_UseExceptionMessageAsDetail_When_ExceptionIsNotStorageError()
    {
        var exception = new UnauthorizedAccessException("access denied\nsecond line");
        var context = CreateContext(exception);

        _sut.OnException(context);

        var problem = context.Result.ShouldBeOfType<ObjectResult>().Value.ShouldBeOfType<ProblemDetails>();
        problem.Detail.ShouldBe(exception.Message);
    }

    [Fact]
    public void Should_LeaveExceptionUnhandled_When_ExceptionIsUnknown()
    {
        var context = CreateContext(new InvalidOperationException("boom"));

        _sut.OnException(context);

        context.ExceptionHandled.ShouldBeFalse();
        context.Result.ShouldBeNull();
    }

    private static ExceptionContext CreateContext(Exception exception)
        => new(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            [])
        {
            Exception = exception,
        };
}
