namespace BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Impl;

public record SecretResponse
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}
