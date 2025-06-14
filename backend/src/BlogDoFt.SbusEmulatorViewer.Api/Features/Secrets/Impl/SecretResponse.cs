using BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Models;
using System.Text;

namespace BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Impl;

public record SecretResponse
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    internal static SecretResponse From(Guid id, string fileName, MemoryStream? stream) => new()
    {
        Id = id,
        FileName = fileName,
        Content = stream is null ? string.Empty : Encoding.UTF8.GetString(stream.ToArray()),
    };

    internal static SecretResponse From(SecretsTable table) => new()
    {
        Id = table.NavigationId,
        FileName = table.FileName ?? "undefined",
        Content = Encoding.UTF8.GetString(table.Content ?? []),
    };
}
