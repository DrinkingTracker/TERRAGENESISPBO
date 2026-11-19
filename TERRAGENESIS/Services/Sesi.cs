using TERRAGENESIS.Models;

namespace TERRAGENESIS.Services
{
    /// <summary>Pengguna yang sedang login. Diisi oleh <see cref="AuthService"/>.</summary>
    public static class Sesi
    {
        public static Pengguna? PenggunaAktif { get; private set; }

        /// <summary>Petak yang terakhir dipilih di halaman Lahan, dipakai menu "Donasi" di navbar.</summary>
        public static int? PetakTerpilihId { get; set; }

        internal static void Masuk(Pengguna pengguna)
        {
            PenggunaAktif = pengguna;
            PetakTerpilihId = null;
        }

        internal static void Keluar()
        {
            PenggunaAktif = null;
            PetakTerpilihId = null;
        }
    }
}
