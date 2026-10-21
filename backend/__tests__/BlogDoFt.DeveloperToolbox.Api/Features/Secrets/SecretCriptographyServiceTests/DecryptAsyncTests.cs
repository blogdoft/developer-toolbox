using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Models;
using System.Text;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets.SecretCriptographyServiceTests;

public class DecryptAsyncTests : BaseSecretCriptographyServiceTests
{
    [Fact]
    public async Task Should_ReturnOriginalContent_When_DecryptingEncryptedSecret()
    {
        var plainText = Fake.Lorem.Paragraphs();
        var (encrypted, iv) = Sut.Encrypt(new SecretSaveModel(Fake.System.FileName(), plainText));
        var table = SecretsTableFaker.Generate();
        table.Content = encrypted.ToArray();
        table.IV = iv;

        var (_, content) = await Sut.DecryptAsync(table);

        Encoding.UTF8.GetString(content.ToArray()).ShouldBe(plainText);
    }

    [Fact]
    public async Task Should_ReturnStoredFileName_When_FileNameIsSet()
    {
        var table = EncryptedTable(fileName: Fake.System.FileName());

        var (fileName, _) = await Sut.DecryptAsync(table);

        fileName.ShouldBe(table.FileName);
    }

    [Fact]
    public async Task Should_ReturnUnspecifiedFileName_When_FileNameIsNull()
    {
        var table = EncryptedTable(fileName: null);

        var (fileName, _) = await Sut.DecryptAsync(table);

        fileName.ShouldBe("unspecified");
    }

    [Fact]
    public async Task Should_ThrowArgumentNullException_When_ContentIsNull()
    {
        var table = SecretsTableFaker.Generate();
        table.Content = null;

        var action = () => Sut.DecryptAsync(table);

        await action.ShouldThrowAsync<ArgumentNullException>();
    }

    private SecretsTable EncryptedTable(string? fileName)
    {
        var (encrypted, iv) = Sut.Encrypt(new SecretSaveModel("f", Fake.Lorem.Sentence()));
        var table = SecretsTableFaker.Generate();
        table.FileName = fileName;
        table.Content = encrypted.ToArray();
        table.IV = iv;
        return table;
    }
}
