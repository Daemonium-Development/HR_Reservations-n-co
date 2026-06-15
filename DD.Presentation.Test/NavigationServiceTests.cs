using DebugDiner.Services;

using FluentAssertions;

using Terminal.Gui;

namespace DD.Presentation.Test;

public class NavigationServiceTests
{
    private static NavigationService CreateSut() => new(StubProvider.Build());

    private static View? CurrentContent(View contentArea) => contentArea.Subviews.FirstOrDefault();

    [Fact]
    public void NavigateTo_WithoutContentArea_IsNoOp()
    {
        var sut = CreateSut();

        var act = () => sut.NavigateTo<StubViewA>();

        act.Should().NotThrow();
    }

    [Fact]
    public void NavigateTo_SwapsInTheRequestedView()
    {
        var sut = CreateSut();
        var content = new View();
        sut.SetContentArea(content);

        sut.NavigateTo<StubViewA>();

        content.Subviews.Should().ContainSingle();
        CurrentContent(content).Should().BeOfType<StubViewA>();
    }

    [Fact]
    public void NavigateTo_ReplacesPreviousView()
    {
        var sut = CreateSut();
        var content = new View();
        sut.SetContentArea(content);

        sut.NavigateTo<StubViewA>();
        sut.NavigateTo<StubViewB>();

        content.Subviews.Should().ContainSingle();
        CurrentContent(content).Should().BeOfType<StubViewB>();
    }

    [Fact]
    public void NavigateBack_RestoresPreviousView()
    {
        var sut = CreateSut();
        var content = new View();
        sut.SetContentArea(content);

        sut.NavigateTo<StubViewA>();
        sut.NavigateTo<StubViewB>();
        sut.NavigateBack();

        content.Subviews.Should().ContainSingle();
        CurrentContent(content).Should().BeOfType<StubViewA>();
    }

    [Fact]
    public void NavigateBack_WithEmptyHistory_IsNoOp()
    {
        var sut = CreateSut();
        var content = new View();
        sut.SetContentArea(content);

        sut.NavigateTo<StubViewA>();
        sut.NavigateBack();

        CurrentContent(content).Should().BeOfType<StubViewA>();
    }

    [Fact]
    public void ClearHistory_PreventsNavigateBack()
    {
        var sut = CreateSut();
        var content = new View();
        sut.SetContentArea(content);

        sut.NavigateTo<StubViewA>();
        sut.NavigateTo<StubViewB>();
        sut.ClearHistory();
        sut.NavigateBack();

        CurrentContent(content).Should().BeOfType<StubViewB>();
    }

    [Fact]
    public void NavigateTo_RaisesNavigationItemsChanged()
    {
        var sut = CreateSut();
        var content = new View();
        sut.SetContentArea(content);
        var fired = false;
        sut.NavigationItemsChanged += _ => fired = true;

        sut.NavigateTo<StubViewA>();

        fired.Should().BeTrue();
    }
}
