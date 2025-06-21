using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Impl;

public class SecretLookupResponse
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    internal static SecretLookupResponse From(SecretsTable table) => new()
    {
        Id = table.NavigationId,
        FileName = table.FileName ?? "undefined",
    };
}
