using System;
using System.Globalization;

namespace TERRAGENESIS.Models
{
    /// <summary>Format angka dan tanggal gaya Indonesia, contoh "Rp 45.000" dan "16 November 2026".</summary>
    public static class Format
    {
        public static readonly CultureInfo Indonesia = CultureInfo.GetCultureInfo("id-ID");

        public static string Rupiah(int nilai) => "Rp " + nilai.ToString("N0", Indonesia);

        public static string Angka(int nilai) => nilai.ToString("N0", Indonesia);

        public static string Tanggal(DateTime tanggal) => tanggal.ToString("d MMMM yyyy", Indonesia);
    }
}
