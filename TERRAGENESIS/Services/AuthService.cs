using System.Threading.Tasks;
using Windows.Storage;
using TERRAGENESIS.Models;
using TERRAGENESIS.Repositories;

namespace TERRAGENESIS.Services
{
    public sealed class AuthService
    {
        private const string KunciEmailDiingat = "EmailDiingat";

        private readonly IPenggunaRepository _pengguna;

        public AuthService(IPenggunaRepository pengguna)
        {
            _pengguna = pengguna;
        }

        public Pengguna? PenggunaAktif => Sesi.PenggunaAktif;

        public async Task<Pengguna> LoginAsync(string email, string password)
        {
            email = email.Trim();
            if (email.Length == 0 || password.Length == 0)
            {
                throw new LayananException("Email dan password wajib diisi.");
            }

            Pengguna? pengguna = await _pengguna.CariByEmailAsync(email);

            // Pesan sengaja sama untuk email tidak terdaftar dan password salah.
            if (pengguna is null || !pengguna.VerifikasiPassword(password))
            {
                throw new LayananException("Email atau password salah.");
            }

            Sesi.Masuk(pengguna);
            return pengguna;
        }

        public void Logout() => Sesi.Keluar();

        /// <summary>Checkbox "Ingat Saya": menyimpan email (bukan password) di pengaturan lokal aplikasi.</summary>
        public void IngatEmail(string? email)
        {
            var pengaturan = ApplicationData.Current.LocalSettings.Values;
            if (string.IsNullOrWhiteSpace(email))
            {
                pengaturan.Remove(KunciEmailDiingat);
            }
            else
            {
                pengaturan[KunciEmailDiingat] = email.Trim();
            }
        }

        public string? EmailDiingat() => ApplicationData.Current.LocalSettings.Values[KunciEmailDiingat] as string;
    }
}
