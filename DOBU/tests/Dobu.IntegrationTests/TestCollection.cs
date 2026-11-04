using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Dobu.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<DobuApiFactory>
{
    public const string Name = "Dobu API collection";
}
