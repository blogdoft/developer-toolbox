using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class UpdateRuleAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNoContent_When_UpdateRuleSucceeds()
    {
        var topicName = Fake.Lorem.Word();
        var subscriptionName = Fake.Lorem.Word();
        var ruleName = Fake.Lorem.Word();
        var settings = new RuleSettings();

        var result = await Sut.UpdateRuleAsync(topicName, subscriptionName, ruleName, settings, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<NoContentResult>();
        await Namespace.Received(1).UpdateRuleAsync(topicName, subscriptionName, ruleName, settings, Arg.Any<CancellationToken>());
    }
}
