using System;

namespace TERRAGENESIS.Models
{
    /// <summary>
    /// Pembayaran satu donasi. <see cref="Jumlah"/> menyimpan nominal saat itu, jadi riwayat
    /// tidak berubah bila harga pohon diganti.
    /// </summary>
    public sealed class Pembayaran
    {
        public Pembayaran(int id, int donasiId, MetodePembayaran metode, int jumlah, string? buktiBayarPath, DateTime dibayarPada)
        {
            Id = id;
            DonasiId = donasiId;
            Metode = metode;
            Jumlah = jumlah;
            BuktiBayarPath = buktiBayarPath;
            DibayarPada = dibayarPada;
        }

        public int Id { get; }
        public int DonasiId { get; }
        public MetodePembayaran Metode { get; }
        public int Jumlah { get; }
        public string? BuktiBayarPath { get; }
        public DateTime DibayarPada { get; }

        public string[] AmbilInstruksi() => Metode.AmbilInstruksi(Jumlah);
    }
}
