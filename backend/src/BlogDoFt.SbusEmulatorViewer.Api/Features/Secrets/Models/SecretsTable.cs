namespace BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Models;

public class SecretsTable
{
    public int Id { get; set; }

    public Guid NavigationId { get; set; }

    public string? FileName { get; set; }

    public byte[]? Content { get; set; }

    public byte[] IV { get; set; } = [];
}
