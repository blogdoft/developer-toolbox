using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class CreateRuleAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNamespaceResult_When_Called()
    {
        var topicName = Fake.Lorem.Word();
        var subscriptionName = Fake.Lorem.Word();
        var ruleName = Fake.Lorem.Word();
        var settings = new RuleSettings();
        var expected = new RuleInfo("r", new RuleSettings());
        Namespace.CreateRuleAsync(topicName, subscriptionName, ruleName, settings, Arg.Any<CancellationToken>()).Returns(expected);

        var result = await Sut.CreateRuleAsync(topicName, subscriptionName, ruleName, settings, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(expected);
    }
}
