using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using TERRAGENESIS.Models;

namespace TERRAGENESIS.Repositories
{
    public sealed class PgDonasiRepository : IDonasiRepository
    {
        private const string KolomDonasi =
            "id, pengguna_id, petak_id, pohon_id, nama_donatur, pesan, jumlah_pohon, status, " +
            "dibuat_pada, disetujui_pada, ditanam_pada, selesai_pada";

        private readonly NpgsqlDataSource _db;

        public PgDonasiRepository(NpgsqlDataSource db)
        {
            _db = db;
        }

        public async Task<int> TambahAsync(Donasi donasi, int metodePembayaranId, int jumlahBayar, string? buktiBayarPath)
        {
            await using var koneksi = await _db.OpenConnectionAsync();
            await using var transaksi = await koneksi.BeginTransactionAsync();

            int donasiId;
            await using (var cmd = new NpgsqlCommand(
                "INSERT INTO donasi (pengguna_id, petak_id, pohon_id, nama_donatur, pesan, jumlah_pohon, status, dibuat_pada) " +
                "VALUES ($1, $2, $3, $4, $5, $6, $7, $8) RETURNING id", koneksi, transaksi))
            {
                cmd.Parameters.AddWithValue(donasi.PenggunaId);
                cmd.Parameters.AddWithValue(donasi.PetakId);
                cmd.Parameters.AddWithValue(donasi.PohonId);
                cmd.Parameters.AddWithValue(donasi.NamaDonatur);
                cmd.Parameters.AddWithValue((object?)donasi.Pesan ?? DBNull.Value);
                cmd.Parameters.AddWithValue(donasi.JumlahPohon);
                cmd.Parameters.AddWithValue(donasi.Status.KeDatabase());
                cmd.Parameters.AddWithValue(donasi.DibuatPada);
                donasiId = (int)(await cmd.ExecuteScalarAsync())!;
            }

            await using (var cmd = new NpgsqlCommand(
                "INSERT INTO pembayaran (donasi_id, metode_pembayaran_id, jumlah, bukti_bayar_path) VALUES ($1, $2, $3, $4)",
                koneksi, transaksi))
            {
                cmd.Parameters.AddWithValue(donasiId);
                cmd.Parameters.AddWithValue(metodePembayaranId);
                cmd.Parameters.AddWithValue(jumlahBayar);
                cmd.Parameters.AddWithValue((object?)buktiBayarPath ?? DBNull.Value);
                await cmd.ExecuteNonQueryAsync();
            }

            await transaksi.CommitAsync();
            return donasiId;
        }

        public async Task<Donasi?> CariByIdAsync(int donasiId)
        {
            await using var cmd = _db.CreateCommand($"SELECT {KolomDonasi} FROM donasi WHERE id = $1");
            cmd.Parameters.AddWithValue(donasiId);
            await using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? BacaDonasi(r) : null;
        }

        public async Task<Donasi?> TerbaruMilikAsync(int penggunaId)
        {
            await using var cmd = _db.CreateCommand(
                $"SELECT {KolomDonasi} FROM donasi WHERE pengguna_id = $1 AND status <> 'ditolak' " +
                "ORDER BY dibuat_pada DESC, id DESC LIMIT 1");
            cmd.Parameters.AddWithValue(penggunaId);
            await using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? BacaDonasi(r) : null;
        }

        public async Task<DetailPetak?> DetailDonasiAktifDiPetakAsync(int petakId)
        {
            await using var cmd = _db.CreateCommand(
                "SELECT p.nomor, w.nama, d.nama_donatur, d.pesan, d.status, d.dibuat_pada " +
                "FROM donasi d JOIN petak p ON p.id = d.petak_id JOIN wilayah w ON w.id = p.wilayah_id " +
                "WHERE d.petak_id = $1 AND d.status <> 'ditolak'");
            cmd.Parameters.AddWithValue(petakId);
            await using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync())
            {
                return null;
            }
            return new DetailPetak(r.GetInt32(0), r.GetString(1), r.GetString(2),
                r.IsDBNull(3) ? null : r.GetString(3), StatusDonasiTeks.DariDatabase(r.GetString(4)), r.GetDateTime(5));
        }

        public async Task<IReadOnlyList<RiwayatDonasi>> RiwayatMilikAsync(int penggunaId)
        {
            await using var cmd = _db.CreateCommand(
                "SELECT d.id, d.dibuat_pada, w.nama, p.nomor, d.jumlah_pohon, COALESCE(pb.jumlah, 0), d.status " +
                "FROM donasi d JOIN petak p ON p.id = d.petak_id JOIN wilayah w ON w.id = p.wilayah_id " +
                "LEFT JOIN pembayaran pb ON pb.donasi_id = d.id " +
                "WHERE d.pengguna_id = $1 ORDER BY d.dibuat_pada DESC, d.id DESC");
            cmd.Parameters.AddWithValue(penggunaId);
            await using var r = await cmd.ExecuteReaderAsync();
            var hasil = new List<RiwayatDonasi>();
            while (await r.ReadAsync())
            {
                hasil.Add(new RiwayatDonasi(r.GetInt32(0), r.GetDateTime(1), r.GetString(2), r.GetInt32(3),
                    r.GetInt32(4), r.GetInt32(5), StatusDonasiTeks.DariDatabase(r.GetString(6))));
            }
            return hasil;
        }

        public async Task<RingkasanDonatur> RingkasanMilikAsync(int penggunaId)
        {
            // Donasi ditolak tidak dihitung; donasi yang masih menunggu verifikasi ikut dihitung.
            await using var cmd = _db.CreateCommand(
                "SELECT COALESCE(SUM(d.jumlah_pohon), 0), COALESCE(SUM(pb.jumlah), 0), COUNT(DISTINCT p.wilayah_id) " +
                "FROM donasi d JOIN petak p ON p.id = d.petak_id LEFT JOIN pembayaran pb ON pb.donasi_id = d.id " +
                "WHERE d.pengguna_id = $1 AND d.status <> 'ditolak'");
            cmd.Parameters.AddWithValue(penggunaId);
            await using var r = await cmd.ExecuteReaderAsync();
            await r.ReadAsync();
            return new RingkasanDonatur((int)r.GetInt64(0), (int)r.GetInt64(1), (int)r.GetInt64(2));
        }

        public async Task<StatistikBeranda> StatistikBerandaAsync()
        {
            await using var cmd = _db.CreateCommand(
                "SELECT " +
                "(SELECT COALESCE(SUM(jumlah_pohon), 0) FROM donasi WHERE status IN ('ditanam', 'selesai')), " +
                "(SELECT COUNT(DISTINCT pengguna_id) FROM donasi WHERE status <> 'ditolak'), " +
                "(SELECT COUNT(DISTINCT p.wilayah_id) FROM donasi d JOIN petak p ON p.id = d.petak_id " +
                " WHERE d.status IN ('ditanam', 'selesai'))");
            await using var r = await cmd.ExecuteReaderAsync();
            await r.ReadAsync();
            return new StatistikBeranda((int)r.GetInt64(0), (int)r.GetInt64(1), (int)r.GetInt64(2));
        }

        public async Task<IReadOnlyList<Dokumentasi>> DokumentasiAsync(int donasiId)
        {
            await using var cmd = _db.CreateCommand(
                "SELECT id, donasi_id, diunggah_oleh, path_foto, diunggah_pada FROM dokumentasi " +
                "WHERE donasi_id = $1 ORDER BY diunggah_pada");
            cmd.Parameters.AddWithValue(donasiId);
            await using var r = await cmd.ExecuteReaderAsync();
            var hasil = new List<Dokumentasi>();
            while (await r.ReadAsync())
            {
                hasil.Add(new Dokumentasi(r.GetInt32(0), r.GetInt32(1), r.GetInt32(2), r.GetString(3), r.GetDateTime(4)));
            }
            return hasil;
        }

        public async Task<IReadOnlyList<Donasi>> SemuaDenganStatusAsync(StatusDonasi status)
        {
            await using var cmd = _db.CreateCommand(
                $"SELECT {KolomDonasi} FROM donasi WHERE status = $1 ORDER BY dibuat_pada, id");
            cmd.Parameters.AddWithValue(status.KeDatabase());
            await using var r = await cmd.ExecuteReaderAsync();
            var hasil = new List<Donasi>();
            while (await r.ReadAsync())
            {
                hasil.Add(BacaDonasi(r));
            }
            return hasil;
        }

        public async Task SimpanStatusAsync(Donasi donasi)
        {
            await using var cmd = _db.CreateCommand(
                "UPDATE donasi SET status = $1, disetujui_pada = $2, ditanam_pada = $3, selesai_pada = $4 WHERE id = $5");
            cmd.Parameters.AddWithValue(donasi.Status.KeDatabase());
            cmd.Parameters.AddWithValue((object?)donasi.DisetujuiPada ?? DBNull.Value);
            cmd.Parameters.AddWithValue((object?)donasi.DitanamPada ?? DBNull.Value);
            cmd.Parameters.AddWithValue((object?)donasi.SelesaiPada ?? DBNull.Value);
            cmd.Parameters.AddWithValue(donasi.Id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task TambahDokumentasiAsync(int donasiId, int adminId, string pathFoto)
        {
            await using var cmd = _db.CreateCommand(
                "INSERT INTO dokumentasi (donasi_id, diunggah_oleh, path_foto) VALUES ($1, $2, $3)");
            cmd.Parameters.AddWithValue(donasiId);
            cmd.Parameters.AddWithValue(adminId);
            cmd.Parameters.AddWithValue(pathFoto);
            await cmd.ExecuteNonQueryAsync();
        }

        private static Donasi BacaDonasi(NpgsqlDataReader r)
        {
            return new Donasi(
                id: r.GetInt32(0),
                penggunaId: r.GetInt32(1),
                petakId: r.GetInt32(2),
                pohonId: r.GetInt32(3),
                namaDonatur: r.GetString(4),
                pesan: r.IsDBNull(5) ? null : r.GetString(5),
                jumlahPohon: r.GetInt32(6),
                status: StatusDonasiTeks.DariDatabase(r.GetString(7)),
                dibuatPada: r.GetDateTime(8),
                disetujuiPada: r.IsDBNull(9) ? null : r.GetDateTime(9),
                ditanamPada: r.IsDBNull(10) ? null : r.GetDateTime(10),
                selesaiPada: r.IsDBNull(11) ? null : r.GetDateTime(11));
        }
    }
}
