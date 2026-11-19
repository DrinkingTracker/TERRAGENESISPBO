using System;

namespace TERRAGENESIS.Models
{
    public enum StatusDonasi
    {
        Menunggu,
        Disetujui,
        Ditanam,
        Selesai,
        Ditolak,
    }

    /// <summary>Konversi antara enum dan nilai teks kolom <c>donasi.status</c>.</summary>
    public static class StatusDonasiTeks
    {
        public static string KeDatabase(this StatusDonasi status) => status switch
        {
            StatusDonasi.Menunggu => "menunggu",
            StatusDonasi.Disetujui => "disetujui",
            StatusDonasi.Ditanam => "ditanam",
            StatusDonasi.Selesai => "selesai",
            StatusDonasi.Ditolak => "ditolak",
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };

        public static StatusDonasi DariDatabase(string nilai) => nilai switch
        {
            "menunggu" => StatusDonasi.Menunggu,
            "disetujui" => StatusDonasi.Disetujui,
            "ditanam" => StatusDonasi.Ditanam,
            "selesai" => StatusDonasi.Selesai,
            "ditolak" => StatusDonasi.Ditolak,
            _ => throw new ArgumentException($"Status donasi tidak dikenal: {nilai}", nameof(nilai)),
        };

        /// <summary>Teks yang tampil di UI (istilah final: "Disetujui").</summary>
        public static string KeTampilan(this StatusDonasi status) => status switch
        {
            StatusDonasi.Menunggu => "Menunggu",
            StatusDonasi.Disetujui => "Disetujui",
            StatusDonasi.Ditanam => "Ditanam",
            StatusDonasi.Selesai => "Selesai",
            StatusDonasi.Ditolak => "Ditolak",
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }
}
