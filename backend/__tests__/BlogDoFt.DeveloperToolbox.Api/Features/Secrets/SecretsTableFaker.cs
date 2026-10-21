using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Models;
using Bogus;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets;

internal static class SecretsTableFaker
{
    private static readonly Faker<SecretsTable> Builder = new Faker<SecretsTable>()
        .RuleFor(t => t.Id, f => f.IndexFaker + 1)
        .RuleFor(t => t.NavigationId, f => f.Random.Guid())
        .RuleFor(t => t.FileName, f => f.System.FileName())
        .RuleFor(t => t.Content, f => f.Random.Bytes(32))
        .RuleFor(t => t.IV, f => f.Random.Bytes(16));

    public static SecretsTable Generate() => Builder.Generate();

    public static List<SecretsTable> Generate(int count) => Builder.Generate(count);
}
