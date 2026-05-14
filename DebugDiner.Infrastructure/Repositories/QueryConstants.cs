namespace DebugDiner.Infrastructure.Repositories;

/// <summary>
/// Replace any `{table}` placeholders with the table name
/// Replace any `{columns}` placeholders with the column names
/// Replace any `{values}` placeholders with the values
/// </summary>
public static class QueryConstants
{
    public const string GetAll = "SELECT * FROM {table}";
    public const string GetById = "SELECT * FROM {table} WHERE id in ({values})";
    public const string GetByColumn = "SELECT * FROM {table} WHERE {column} in ({values})";
    public const string CountByColumn = "SELECT COUNT(*) FROM {table} WHERE {column} = @value";
    public const string Insert = "INSERT INTO {table} ({columns}) VALUES ({values}) RETURNING id";
    public const string Update = "UPDATE {table} SET {columns} WHERE id = @id RETURNING id";
    public const string Delete = "DELETE FROM {table} WHERE id = @id";
    public const string DeleteByColumn = "DELETE FROM {table} WHERE {column} = @value";
}
