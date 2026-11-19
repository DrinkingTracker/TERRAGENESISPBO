using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using TERRAGENESIS.Models;

namespace TERRAGENESIS.Repositories
{
    /// <summary>Data referensi: harga pohon dan daftar metode pembayaran.</summary>
    public sealed class PgReferensiRepository : IReferensiRepository
    {
        private readonly NpgsqlDataSource _db;

        public PgReferensiRepository(NpgsqlDataSource db)
        {
            _db = db;
        }

        public async Task<Pohon> PohonAsync()
        {
            await using var cmd = _db.CreateCommand("SELECT id, harga FROM pohon ORDER BY id LIMIT 1");
            await using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync())
            {
                throw new InvalidOperationException("Tabel pohon kosong. Jalankan bagian DATA AWAL di schema.sql.");
            }
            return new Pohon(r.GetInt32(0), r.GetInt32(1));
        }

        public async Task<IReadOnlyList<MetodePembayaran>> SemuaMetodeAsync()
        {
            await using var cmd = _db.CreateCommand("SELECT id, metode FROM metode_pembayaran ORDER BY id");
            await using var r = await cmd.ExecuteReaderAsync();
            var hasil = new List<MetodePembayaran>();
            while (await r.ReadAsync())
            {
                hasil.Add(MetodePembayaran.Buat(r.GetInt32(0), r.GetString(1)));
            }
            return hasil;
        }
    }
}
