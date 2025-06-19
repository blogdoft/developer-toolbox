using BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Models;

namespace BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets;

public interface ISecretRepository
{
    Task<Guid> SaveAsync(MemoryStream text, byte[] iv, string fileName);

    Task<SecretsTable?> QueryById(Guid navigationId);

    Task<IEnumerable<SecretsTable>> AllAsync();

    Task<bool> Delete(Guid id);
}
