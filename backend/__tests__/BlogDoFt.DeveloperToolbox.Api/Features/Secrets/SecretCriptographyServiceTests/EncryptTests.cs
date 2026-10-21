using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Models;
using System.Text;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets.SecretCriptographyServiceTests;

public class EncryptTests : BaseSecretCriptographyServiceTests
{
    [Fact]
    public void Should_ReturnSixteenByteIv_When_Encrypting()
    {
        var model = new SecretSaveModel(Fake.System.FileName(), Fake.Lorem.Sentence());

        var (_, iv) = Sut.Encrypt(model);

        iv.Length.ShouldBe(16);
    }

    [Fact]
    public void Should_NotExposePlainText_When_Encrypting()
    {
        var model = new SecretSaveModel(Fake.System.FileName(), Fake.Lorem.Sentence());

        var (content, _) = Sut.Encrypt(model);

        Encoding.UTF8.GetString(content.ToArray()).ShouldNotContain(model.Content);
    }

    [Fact]
    public void Should_GenerateDifferentIv_When_EncryptingTwice()
    {
        var model = new SecretSaveModel(Fake.System.FileName(), Fake.Lorem.Sentence());

        var (_, first) = Sut.Encrypt(model);
        var (_, second) = Sut.Encrypt(model);

        first.ShouldNotBe(second);
    }
}
