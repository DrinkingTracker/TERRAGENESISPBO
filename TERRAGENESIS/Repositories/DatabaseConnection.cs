using Npgsql;

namespace TERRAGENESIS.Repositories
{
    /// <summary>Koneksi ke PostgreSQL. Satu objek dipakai bersama oleh semua repository.</summary>
    public sealed class DatabaseConnection
    {
        public DatabaseConnection(string host, int port, string database, string username, string password)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Database = database,
                Username = username,
                Password = password,
            };
            Sumber = NpgsqlDataSource.Create(builder.ConnectionString);
        }

        public NpgsqlDataSource Sumber { get; }
    }
}
