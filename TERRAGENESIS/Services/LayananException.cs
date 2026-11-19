using System;

namespace TERRAGENESIS.Services
{
    /// <summary>
    /// Kesalahan yang pesannya aman dan cukup jelas untuk langsung ditampilkan ke pengguna,
    /// misalnya "Password salah." atau "Petak sudah dipilih donatur lain."
    /// </summary>
    public sealed class LayananException : Exception
    {
        public LayananException(string pesan) : base(pesan)
        {
        }
    }
}
