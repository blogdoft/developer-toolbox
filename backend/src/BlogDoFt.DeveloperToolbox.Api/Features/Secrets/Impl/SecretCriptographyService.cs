using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Models;
using System.Security.Cryptography;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Impl;

internal class SecretCriptographyService : ISecretCriptographyService
{
    private readonly byte[] _cryptoKey = Convert.FromBase64String("rGh3sZXc4YHkc7Y6ZVqtqv7YXym+d1Ns1jEGPa+dglk=");

    public async Task<(string Filename, MemoryStream Content)> DecryptAsync(SecretsTable model)
    {
        if (model.Content is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        await using var encryptedStream = new MemoryStream(model.Content);
        var decryptedStream = new MemoryStream();

        using (var aes = Aes.Create())
        {
            aes.Key = _cryptoKey;
            aes.IV = model.IV;

            await using var cryptoStream = new CryptoStream(encryptedStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            await cryptoStream.CopyToAsync(decryptedStream);
        }

        return (model.FileName ?? "unspecified", decryptedStream);
    }

    public (MemoryStream Content, byte[] IV) Encrypt(SecretSaveModel model)
    {
        var iv = new byte[16];
        MemoryStream memoryStream = new();
        using (var aes = Aes.Create())
        {
            aes.Key = _cryptoKey;
            aes.GenerateIV();
            iv = aes.IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
            using StreamWriter streamWriter = new(cryptoStream);
            streamWriter.Write(model.Content);
        }

        return (memoryStream, iv);
    }
}
