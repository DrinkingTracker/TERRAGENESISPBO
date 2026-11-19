using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Npgsql;
using TERRAGENESIS.Models;
using TERRAGENESIS.Services;

namespace TERRAGENESIS.Pages
{
    /// <summary>Navigasi antarhalaman dan dialog yang dipakai bersama oleh semua Page.</summary>
    internal static class Navigasi
    {
        private static bool _dialogTampil;

        /// <summary>Tujuan dari NavbarControl: "Beranda", "Lahan", "Donasi", atau "Profile".</summary>
        public static void KeMenuNavbar(Frame frame, string tujuan)
        {
            switch (tujuan)
            {
                case "Beranda":
                    frame.Navigate(typeof(BerandaPage));
                    break;
                case "Lahan":
                    frame.Navigate(typeof(PetaWilayahPage));
                    break;
                case "Donasi":
                    // Halaman Donasi butuh petak; kalau belum ada yang dipilih, mulai dari peta.
                    if (Sesi.PetakTerpilihId is int petakId)
                    {
                        frame.Navigate(typeof(DonasiPage), petakId);
                    }
                    else
                    {
                        frame.Navigate(typeof(PetaWilayahPage), PetaWilayahPage.PesanPilihPetak);
                    }
                    break;
                case "Profile":
                    frame.Navigate(typeof(ProfilePage));
                    break;
            }
        }

        /// <summary>Tujuan dari SidebarControl: "Beranda", "Profile", "Donasi Saya", "Penanaman Saya", atau "Keluar".</summary>
        public static void KeMenuSidebar(Frame frame, string tujuan)
        {
            switch (tujuan)
            {
                case "Beranda":
                    frame.Navigate(typeof(BerandaPage));
                    break;
                case "Profile":
                    frame.Navigate(typeof(ProfilePage));
                    break;
                case "Donasi Saya":
                    frame.Navigate(typeof(DonasiSayaPage));
                    break;
                case "Penanaman Saya":
                    frame.Navigate(typeof(PenanamanSayaPage));
                    break;
                case "Keluar":
                    Keluar(frame);
                    break;
            }
        }

        public static void Keluar(Frame frame)
        {
            Layanan.Auth.Logout();
            frame.Navigate(typeof(LoginPage));
            frame.BackStack.Clear();
        }

        /// <summary>
        /// Mengembalikan pengguna yang sedang login. Bila belum login (misalnya sesi habis),
        /// halaman dialihkan ke Login dan hasilnya null.
        /// </summary>
        public static Pengguna? PastikanLogin(Frame frame)
        {
            Pengguna? pengguna = Sesi.PenggunaAktif;
            if (pengguna is null)
            {
                frame.Navigate(typeof(LoginPage));
                frame.BackStack.Clear();
            }
            return pengguna;
        }

        public static async Task TampilkanPesanAsync(XamlRoot? xamlRoot, string judul, string isi)
        {
            if (xamlRoot is null || _dialogTampil) return; // ContentDialog hanya boleh satu pada satu waktu
            _dialogTampil = true;
            try
            {
                var dialog = new ContentDialog
                {
                    Title = judul,
                    Content = isi,
                    CloseButtonText = "OK",
                    XamlRoot = xamlRoot,
                };
                await dialog.ShowAsync();
            }
            finally
            {
                _dialogTampil = false;
            }
        }

        /// <summary>
        /// Menjalankan pekerjaan yang memanggil Services dan menampilkan kesalahannya sebagai dialog.
        /// Mengembalikan true bila berhasil.
        /// </summary>
        public static async Task<bool> JalankanAsync(XamlRoot? xamlRoot, Func<Task> kerja)
        {
            try
            {
                await kerja();
                return true;
            }
            catch (LayananException ex)
            {
                await TampilkanPesanAsync(xamlRoot, "Tidak bisa diproses", ex.Message);
            }
            catch (NpgsqlException ex)
            {
                await TampilkanPesanAsync(xamlRoot, "Database tidak bisa diakses",
                    "Pastikan PostgreSQL berjalan, database \"terragenesis\" sudah dibuat dari schema.sql, " +
                    $"dan connection string sudah benar.\n\nDetail: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                await TampilkanPesanAsync(xamlRoot, "Terjadi kesalahan", ex.Message);
            }
            return false;
        }
    }
}
