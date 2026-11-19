using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using TERRAGENESIS.Models;

namespace TERRAGENESIS.Repositories
{
    public sealed class PgWilayahRepository : IWilayahRepository
    {
        // v_status_petak sudah menghitung terisi/kosong dari donasi aktif (lihat schema.sql).
        private const string KolomPetak = "id, wilayah_id, wilayah, nomor, status_lahan, status_petak";

        private readonly NpgsqlDataSource _db;

        public PgWilayahRepository(NpgsqlDataSource db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Wilayah>> SemuaAsync()
        {
            await using var cmd = _db.CreateCommand("SELECT id, nama FROM wilayah ORDER BY id");
            await using var r = await cmd.ExecuteReaderAsync();
            var hasil = new List<Wilayah>();
            while (await r.ReadAsync())
            {
                hasil.Add(new Wilayah(r.GetInt32(0), r.GetString(1)));
            }
            return hasil;
        }

        public async Task<Wilayah?> CariByNamaAsync(string nama)
        {
            await using var cmd = _db.CreateCommand("SELECT id, nama FROM wilayah WHERE nama = $1");
            cmd.Parameters.AddWithValue(nama);
            await using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? new Wilayah(r.GetInt32(0), r.GetString(1)) : null;
        }

        public async Task<IReadOnlyList<Petak>> PetakDiWilayahAsync(int wilayahId)
        {
            await using var cmd = _db.CreateCommand(
                $"SELECT {KolomPetak} FROM v_status_petak WHERE wilayah_id = $1 ORDER BY nomor");
            cmd.Parameters.AddWithValue(wilayahId);
            await using var r = await cmd.ExecuteReaderAsync();
            var hasil = new List<Petak>();
            while (await r.ReadAsync())
            {
                hasil.Add(BacaPetak(r));
            }
            return hasil;
        }

        public async Task<Petak?> CariPetakAsync(int petakId)
        {
            await using var cmd = _db.CreateCommand($"SELECT {KolomPetak} FROM v_status_petak WHERE id = $1");
            cmd.Parameters.AddWithValue(petakId);
            await using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? BacaPetak(r) : null;
        }

        public async Task<IReadOnlyDictionary<string, int>> JumlahPetakKosongAsync()
        {
            await using var cmd = _db.CreateCommand(
                "SELECT wilayah, COUNT(*) FILTER (WHERE status_petak = 'kosong') FROM v_status_petak GROUP BY wilayah");
            await using var r = await cmd.ExecuteReaderAsync();
            var hasil = new Dictionary<string, int>();
            while (await r.ReadAsync())
            {
                hasil[r.GetString(0)] = (int)r.GetInt64(1);
            }
            return hasil;
        }

        public async Task UbahStatusPetakAsync(int petakId, bool aktif)
        {
            await using var cmd = _db.CreateCommand("UPDATE petak SET status = $1 WHERE id = $2");
            cmd.Parameters.AddWithValue(aktif ? "aktif" : "nonaktif");
            cmd.Parameters.AddWithValue(petakId);
            await cmd.ExecuteNonQueryAsync();
        }

        private static Petak BacaPetak(NpgsqlDataReader r)
        {
            return new Petak(
                id: r.GetInt32(0),
                wilayahId: r.GetInt32(1),
                namaWilayah: r.GetString(2),
                nomor: r.GetInt32(3),
                aktif: r.GetString(4) == "aktif",
                terisi: r.GetString(5) == "terisi");
        }
    }
}
