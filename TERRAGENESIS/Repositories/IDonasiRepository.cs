using System.Collections.Generic;
using System.Threading.Tasks;
using TERRAGENESIS.Models;

namespace TERRAGENESIS.Repositories
{
    public interface IDonasiRepository
    {
        /// <summary>Menyimpan donasi dan pembayarannya dalam satu transaksi; mengembalikan id donasi.</summary>
        Task<int> TambahAsync(Donasi donasi, int metodePembayaranId, int jumlahBayar, string? buktiBayarPath);

        Task<Donasi?> CariByIdAsync(int donasiId);

        /// <summary>Donasi terbaru milik pengguna (tidak termasuk yang ditolak), atau null.</summary>
        Task<Donasi?> TerbaruMilikAsync(int penggunaId);

        Task<DetailPetak?> DetailDonasiAktifDiPetakAsync(int petakId);
        Task<IReadOnlyList<RiwayatDonasi>> RiwayatMilikAsync(int penggunaId);
        Task<RingkasanDonatur> RingkasanMilikAsync(int penggunaId);
        Task<StatistikBeranda> StatistikBerandaAsync();
        Task<IReadOnlyList<Dokumentasi>> DokumentasiAsync(int donasiId);

        /// <summary>Admin: daftar donasi menurut status (Kelola Donasi).</summary>
        Task<IReadOnlyList<Donasi>> SemuaDenganStatusAsync(StatusDonasi status);

        /// <summary>Admin: menyimpan status dan waktu-waktu donasi setelah diubah lewat method Donasi.</summary>
        Task SimpanStatusAsync(Donasi donasi);

        /// <summary>Admin: menambah foto dokumentasi penanaman.</summary>
        Task TambahDokumentasiAsync(int donasiId, int adminId, string pathFoto);
    }
}
