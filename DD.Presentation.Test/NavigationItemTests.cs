using DebugDiner.Services;

using FluentAssertions;

using Moq;

namespace DD.Presentation.Test;

public class NavigationItemTests
{
    [Fact]
    public void Label_IsExposed()
    {
        var item = new NavigationItem("Home", _ => { });

        item.Label.Should().Be("Home");
    }

    [Fact]
    public void Navigate_InvokesTheAction()
    {
        var nav = new Mock<INavigationService>();
        var item = new NavigationItem("Home", n => n.NavigateBack());

        item.Navigate(nav.Object);

        nav.Verify(n => n.NavigateBack(), Times.Once);
    }

    [Fact]
    public void Records_WithSameLabelAndDelegate_AreEqual()
    {
        Action<INavigationService> action = _ => { };
        var a = new NavigationItem("Home", action);
        var b = new NavigationItem("Home", action);

        a.Should().Be(b);
    }
}
