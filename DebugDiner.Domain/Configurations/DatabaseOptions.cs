namespace DebugDiner.Domain.Configurations;

public class DatabaseOptions
{
    public static string SectionName = "Database";
    public string Source { get; set; }

    public string ResolvedSource()
    {
        if (string.IsNullOrWhiteSpace(Source)) return Source;
        if (!Source.Contains("%APPDATA%"))
        {
            return Path.Combine(Environment.CurrentDirectory, Source);
        }
        var special = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var adjustedPath = Source.Replace("%APPDATA%", special);
        return adjustedPath;
    }
}
