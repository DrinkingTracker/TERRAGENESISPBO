using System.Threading.Tasks;
using Npgsql;
using TERRAGENESIS.Models;

namespace TERRAGENESIS.Repositories
{
    public sealed class PgPenggunaRepository : IPenggunaRepository
    {
        private const string Kolom = "id, nama, email, password_hash, peran, dibuat_pada";

        private readonly NpgsqlDataSource _db;

        public PgPenggunaRepository(NpgsqlDataSource db)
        {
            _db = db;
        }

        public async Task<Pengguna?> CariByEmailAsync(string email)
        {
            await using var cmd = _db.CreateCommand($"SELECT {Kolom} FROM pengguna WHERE lower(email) = lower($1)");
            cmd.Parameters.AddWithValue(email);
            return await BacaSatuAsync(cmd);
        }

        public async Task<Pengguna?> CariByIdAsync(int id)
        {
            await using var cmd = _db.CreateCommand($"SELECT {Kolom} FROM pengguna WHERE id = $1");
            cmd.Parameters.AddWithValue(id);
            return await BacaSatuAsync(cmd);
        }

        private static async Task<Pengguna?> BacaSatuAsync(NpgsqlCommand cmd)
        {
            await using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync())
            {
                return null;
            }
            return Pengguna.Buat(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3), r.GetString(4), r.GetDateTime(5));
        }
    }
}
