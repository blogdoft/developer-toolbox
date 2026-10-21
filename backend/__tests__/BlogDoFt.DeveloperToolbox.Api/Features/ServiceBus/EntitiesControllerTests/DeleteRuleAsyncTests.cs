using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class DeleteRuleAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNoContent_When_DeleteRuleSucceeds()
    {
        var topicName = Fake.Lorem.Word();
        var subscriptionName = Fake.Lorem.Word();
        var ruleName = Fake.Lorem.Word();

        var result = await Sut.DeleteRuleAsync(topicName, subscriptionName, ruleName, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<NoContentResult>();
        await Namespace.Received(1).DeleteRuleAsync(topicName, subscriptionName, ruleName, Arg.Any<CancellationToken>());
    }
}
