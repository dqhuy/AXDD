using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AXDD.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Factory for creating database connections
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a new database connection
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An open database connection</returns>
    Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// SQL Server connection factory implementation
/// </summary>
public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));
        _connectionString = connectionString;
    }

    public async Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
