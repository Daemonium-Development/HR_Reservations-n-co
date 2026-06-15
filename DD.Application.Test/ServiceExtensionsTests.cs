using DebugDiner.Application;
using DebugDiner.Domain.Abstractions;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Serilog;

namespace DD.Application.Test;

public class ServiceExtensionsTests
{
    [Fact]
    public void AddApplication_ResolvesAuthService()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddSingleton(new Mock<IUserRepository>().Object);
        services.AddSingleton(new Mock<ILogger>().Object);

        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IAuthService>().Should().BeOfType<AuthService>();
    }

    [Fact]
    public void AddApplication_RegistersAuthServiceAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        var descriptor = services.Single(d => d.ServiceType == typeof(IAuthService));

        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ImplementationType.Should().Be(typeof(AuthService));
    }
}
