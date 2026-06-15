using DebugDiner.Domain.Configurations;

using FluentAssertions;

namespace DD.Domain.Test;

public class DatabaseOptionsTests
{
    [Fact]
    public void ResolvedSource_NullSource_Throws()
    {
        var options = new DatabaseOptions();

        var act = () => options.ResolvedSource();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ResolvedSource_WhitespaceSource_Throws()
    {
        var options = new DatabaseOptions { Source = "   " };

        var act = () => options.ResolvedSource();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ResolvedSource_RelativeSource_CombinesWithCurrentDirectory()
    {
        var options = new DatabaseOptions { Source = "test.db" };

        options.ResolvedSource()
            .Should().Be(Path.Combine(Environment.CurrentDirectory, "test.db"));
    }

    [Fact]
    public void ResolvedSource_AppDataToken_ExpandsToLocalApplicationData()
    {
        var options = new DatabaseOptions { Source = "%APPDATA%/x.db" };
        var special = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        var resolved = options.ResolvedSource();

        resolved.Should().NotContain("%APPDATA%");
        resolved.Should().StartWith(special);
    }
}
