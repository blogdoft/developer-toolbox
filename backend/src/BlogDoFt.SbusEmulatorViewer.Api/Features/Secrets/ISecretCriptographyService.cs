using BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Models;

namespace BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets;

public interface ISecretCriptographyService
{
    (MemoryStream Text, byte[] IV) Encrypt(SecretSaveModel model);

    Task<(string Filename, MemoryStream Content)> DecryptAsync(SecretsTable model);
}
