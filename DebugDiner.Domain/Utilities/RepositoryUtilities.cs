namespace DebugDiner.Domain.Utilities;

public static class RepositoryUtilities
{
    extension(string value)
    {
        public T MapToEnum<T>() where T : Enum => (T)Enum.Parse(typeof(T), value);
    }
}
