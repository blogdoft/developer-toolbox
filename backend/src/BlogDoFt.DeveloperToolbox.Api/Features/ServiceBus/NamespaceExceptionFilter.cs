using Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;

/// <summary>
/// Turns Service Bus and Storage failures into problem details responses, so the
/// frontend can show the reason to the user.
/// </summary>
public class NamespaceExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var (status, title) = context.Exception switch
        {
            NamespaceOperationNotSupportedException => (StatusCodes.Status501NotImplemented, "Operation not supported"),
            ServiceBusException { Reason: ServiceBusFailureReason.MessagingEntityNotFound }
                => (StatusCodes.Status404NotFound, "Entity not found"),
            ServiceBusException { Reason: ServiceBusFailureReason.MessagingEntityAlreadyExists }
                => (StatusCodes.Status409Conflict, "Entity already exists"),
            ServiceBusException => (StatusCodes.Status502BadGateway, "Service Bus request failed"),
            RequestFailedException { Status: StatusCodes.Status404NotFound }
                => (StatusCodes.Status404NotFound, "Resource not found"),
            RequestFailedException { Status: StatusCodes.Status409Conflict }
                => (StatusCodes.Status409Conflict, "Resource already exists"),
            RequestFailedException { Status: StatusCodes.Status400BadRequest }
                => (StatusCodes.Status400BadRequest, "Invalid storage request"),
            RequestFailedException => (StatusCodes.Status502BadGateway, "Storage request failed"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Not authorized"),
            _ => (0, null),
        };

        if (title is null)
        {
            return;
        }

        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = Detail(context.Exception),
        })
        {
            StatusCode = status,
        };
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Storage errors append the raw HTTP response, which only adds noise for the user.
    /// </summary>
    private static string Detail(Exception exception)
        => exception is RequestFailedException
            ? exception.Message.Split('\n', 2)[0].Trim()
            : exception.Message;
}
