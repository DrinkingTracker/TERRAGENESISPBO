using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Windows.Storage;
using TERRAGENESIS.Models;
using TERRAGENESIS.Repositories;

namespace TERRAGENESIS.Services
{
    public sealed class DonasiService
    {
        private readonly IDonasiRepository _donasi;
        private readonly IWilayahRepository _wilayah;
        private readonly IReferensiRepository _referensi;
        private readonly PenyimpananFoto _foto;

        public DonasiService(IDonasiRepository donasi, IWilayahRepository wilayah, IReferensiRepository referensi,
            PenyimpananFoto foto)
        {
            _donasi = donasi;
            _wilayah = wilayah;
            _referensi = referensi;
            _foto = foto;
        }

        public Task<Pohon> PohonAsync() => _referensi.PohonAsync();

        public Task<IReadOnlyList<MetodePembayaran>> MetodePembayaranAsync() => _referensi.SemuaMetodeAsync();

        public Task ValidasiBuktiAsync(StorageFile bukti) => _foto.ValidasiAsync(bukti);

        /// <summary>Tombol "Konfirmasi Donasi": menyimpan donasi (status menunggu) beserta pembayarannya.</summary>
        public async Task<int> BuatDonasiAsync(Pengguna pengguna, int petakId, int jumlahPohon, string namaDonatur,
            string? pesan, MetodePembayaran? metode, StorageFile? bukti)
        {
            if (pengguna is not Donatur donatur)
            {
                throw new LayananException("Hanya akun donatur yang bisa berdonasi.");
            }

            namaDonatur = namaDonatur.Trim();
            pesan = string.IsNullOrWhiteSpace(pesan) ? null : pesan.Trim();

            if (jumlahPohon < 1)
            {
                throw new LayananException("Jumlah pohon minimal 1.");
            }
            if (namaDonatur.Length == 0)
            {
                throw new LayananException("Nama donatur wajib diisi.");
            }
            if (namaDonatur.Length > Donasi.MaksPanjangNama)
            {
                throw new LayananException($"Nama donatur maksimal {Donasi.MaksPanjangNama} karakter.");
            }
            if (pesan?.Length > Donasi.MaksPanjangPesan)
            {
                throw new LayananException($"Pesan maksimal {Donasi.MaksPanjangPesan} karakter.");
            }
            if (metode is null)
            {
                throw new LayananException("Pilih metode pembayaran dulu.");
            }
            if (bukti is null)
            {
                throw new LayananException("Upload bukti pembayaran dulu.");
            }

            Petak petak = await _wilayah.CariPetakAsync(petakId)
                ?? throw new LayananException("Petak tidak ditemukan.");
            if (!petak.BisaDipilih)
            {
                throw new LayananException($"Petak #{petak.Nomor} sudah tidak tersedia. Pilih petak lain.");
            }

            Pohon pohon = await _referensi.PohonAsync();
            Donasi donasi = donatur.BuatDonasi(petak, pohon, namaDonatur, pesan, jumlahPohon);

            string pathBukti = await _foto.SimpanAsync(bukti, "bukti");
            try
            {
                return await _donasi.TambahAsync(donasi, metode.Id, donasi.HitungTotal(pohon), pathBukti);
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                // Indeks uq_donasi_petak_aktif: petak keburu dipilih donatur lain.
                _foto.Hapus(pathBukti);
                throw new LayananException($"Petak #{petak.Nomor} baru saja dipilih donatur lain. Pilih petak lain.");
            }
            catch
            {
                _foto.Hapus(pathBukti);
                throw;
            }
        }

        public Task<IReadOnlyList<RiwayatDonasi>> RiwayatAsync(Pengguna pengguna) => _donasi.RiwayatMilikAsync(pengguna.Id);

        public Task<RingkasanDonatur> RingkasanAsync(Pengguna pengguna) => _donasi.RingkasanMilikAsync(pengguna.Id);

        /// <summary>
        /// Donasi yang ditampilkan di Penanaman Saya: donasi yang dipilih dari Donasi Saya,
        /// atau donasi aktif terbaru bila tidak ada yang dipilih.
        /// </summary>
        public async Task<Donasi?> DonasiUntukPenanamanAsync(Pengguna pengguna, int? donasiId)
        {
            if (donasiId is int id)
            {
                Donasi? donasi = await _donasi.CariByIdAsync(id);
                // Donasi milik orang lain tidak boleh dilihat.
                return donasi?.PenggunaId == pengguna.Id ? donasi : null;
            }
            return await _donasi.TerbaruMilikAsync(pengguna.Id);
        }

        public Task<IReadOnlyList<Dokumentasi>> DokumentasiAsync(Donasi donasi) => _donasi.DokumentasiAsync(donasi.Id);

        // ---------- Logika admin (halaman admin menyusul) ----------

        public Task<IReadOnlyList<Donasi>> DonasiMenungguAsync() => _donasi.SemuaDenganStatusAsync(StatusDonasi.Menunggu);

        /// <summary>Kelola Donasi: centang (setujui) atau silang (tolak).</summary>
        public Task VerifikasiAsync(Pengguna oleh, int donasiId, bool disetujui) =>
            UbahStatusAsync(oleh, donasiId, (admin, donasi) => admin.VerifikasiDonasi(donasi, disetujui));

        public Task TandaiDitanamAsync(Pengguna oleh, int donasiId) =>
            UbahStatusAsync(oleh, donasiId, (_, donasi) => donasi.Tanam());

        public Task TandaiSelesaiAsync(Pengguna oleh, int donasiId) =>
            UbahStatusAsync(oleh, donasiId, (_, donasi) => donasi.Selesaikan());

        public async Task UnggahDokumentasiAsync(Pengguna oleh, int donasiId, StorageFile foto)
        {
            Admin admin = PastikanAdmin(oleh);
            Donasi donasi = await _donasi.CariByIdAsync(donasiId)
                ?? throw new LayananException("Donasi tidak ditemukan.");
            if (donasi.Status is not (StatusDonasi.Ditanam or StatusDonasi.Selesai))
            {
                throw new LayananException("Dokumentasi hanya bisa diunggah setelah pohon ditanam.");
            }

            string path = await _foto.SimpanAsync(foto, "dokumentasi");
            await _donasi.TambahDokumentasiAsync(donasi.Id, admin.Id, path);
        }

        private async Task UbahStatusAsync(Pengguna oleh, int donasiId, Action<Admin, Donasi> ubah)
        {
            Admin admin = PastikanAdmin(oleh);
            Donasi donasi = await _donasi.CariByIdAsync(donasiId)
                ?? throw new LayananException("Donasi tidak ditemukan.");
            try
            {
                ubah(admin, donasi);
            }
            catch (InvalidOperationException ex)
            {
                throw new LayananException(ex.Message);
            }
            await _donasi.SimpanStatusAsync(donasi);
        }

        private static Admin PastikanAdmin(Pengguna pengguna) =>
            pengguna as Admin ?? throw new LayananException("Hanya admin yang boleh melakukan ini.");
    }
}
