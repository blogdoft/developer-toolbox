using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;

/// <summary>
/// Turns Service Bus management failures into problem details responses, so the
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
            Detail = context.Exception.Message,
        })
        {
            StatusCode = status,
        };
        context.ExceptionHandled = true;
    }
}
