namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;

/// <summary>
/// Thrown when the connected Service Bus target cannot perform a management operation.
/// </summary>
public class NamespaceOperationNotSupportedException : Exception
{
    public NamespaceOperationNotSupportedException(string operation)
        : base(
            $"The operation '{operation}' is not supported by the Azure Service Bus emulator. " +
            "The emulator has no management API and loads its entities from the config file at startup. " +
            "Edit the config file and restart the emulator, or connect to a real Service Bus namespace.")
    {
    }
}
