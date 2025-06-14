using BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Models;
using Dapper;
using System.Data;

namespace BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Impl;

internal class SecretRepository : ISecretRepository
{
    private readonly IDbConnection _db;

    public SecretRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<SecretsTable?> QueryById(Guid navigationId)
    {
        var sql =
            """ 
                SELECT id
                     , navigation_id as NavigationId
                     , file_name as FileName
                     , "content"
                     , iv
                FROM public.encrypted_files
                where navigation_id = @NavigationId;
            """;
        var result = await _db.QueryFirstOrDefaultAsync<SecretsTable>(sql, new { navigationId });

        return result;
    }

    public async Task<Guid> SaveAsync(MemoryStream text, byte[] iv, string fileName)
    {
        var navigationId = Guid.NewGuid();
        var sql = "INSERT INTO encrypted_files (navigation_id, file_name, content, iv) VALUES (@NavigationId, @FileName, @Content, @IV)";
        await _db.ExecuteAsync(sql, new
        {
            navigationId,
            fileName,
            Content = text.ToArray(),
            IV = iv,
        });

        return navigationId;
    }
}
