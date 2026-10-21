using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets.SecretsControllerTests;

public class DeleteAsyncTests : BaseSecretsControllerTests
{
    [Fact]
    public async Task Should_ReturnNoContent_When_SecretIsDeleted()
    {
        var id = Guid.NewGuid();
        Repository.Delete(id).Returns(true);

        var result = await Sut.DeleteAsync(id);

        result.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_SecretDoesNotExist()
    {
        var id = Guid.NewGuid();
        Repository.Delete(id).Returns(false);

        var result = await Sut.DeleteAsync(id);

        result.ShouldBeOfType<NotFoundResult>();
    }
}
