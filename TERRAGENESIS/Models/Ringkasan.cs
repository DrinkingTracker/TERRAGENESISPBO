using System;

namespace TERRAGENESIS.Models
{
    // Data hasil query gabungan untuk ditampilkan; bukan entity di ERD.

    /// <summary>Isi pop-up "Pohon Harapan" untuk petak terisi di halaman Lahan.</summary>
    public sealed record DetailPetak(int NomorPetak, string NamaWilayah, string NamaDonatur, string? Pesan,
        StatusDonasi Status, DateTime TanggalDonasi);

    /// <summary>Satu baris tabel Riwayat Donasi (halaman Donasi Saya).</summary>
    public sealed record RiwayatDonasi(int DonasiId, DateTime Tanggal, string NamaWilayah, int NomorPetak,
        int JumlahPohon, int Nominal, StatusDonasi Status);

    /// <summary>Tiga kartu Ringkasan Donasi di halaman Profile.</summary>
    public sealed record RingkasanDonatur(int TotalPohon, int TotalDonasi, int JumlahWilayah);

    /// <summary>Kartu statistik di Beranda.</summary>
    public sealed record StatistikBeranda(int TotalPohonTertanam, int JumlahDonatur, int WilayahTerhijaukan);
}
