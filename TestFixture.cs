using _13Laba_Proga;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace _13Laba_Proga.Tests
{
    public class TestFixture : IAsyncLifetime
    {
        public TestApplicationFactory Factory { get; } = new();

        public async Task InitializeAsync()
        {
            await using var scope = Factory.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

            await db.Database.OpenConnectionAsync();
            await db.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}