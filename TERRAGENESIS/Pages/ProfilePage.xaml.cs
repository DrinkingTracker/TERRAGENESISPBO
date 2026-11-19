using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TERRAGENESIS.Models;
using TERRAGENESIS.Services;

namespace TERRAGENESIS.Pages
{
    /// <summary>Data diri donatur dan ringkasan donasinya.</summary>
    public sealed partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            Loaded += ProfilePage_Loaded;
        }

        private async void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            Pengguna? pengguna = Navigasi.PastikanLogin(Frame);
            if (pengguna is null) return;

            TxtSapaan.Text = $"Halo, {pengguna.NamaDepan}!";
            TxtNama.Text = pengguna.Nama;
            TxtEmail.Text = pengguna.Email;

            await Navigasi.JalankanAsync(XamlRoot, async () =>
            {
                RingkasanDonatur ringkasan = await Layanan.Donasi.RingkasanAsync(pengguna);
                TxtTotalPohon.Text = Format.Angka(ringkasan.TotalPohon);
                TxtTotalDonasi.Text = Format.Rupiah(ringkasan.TotalDonasi).ToUpperInvariant();
                TxtWilayah.Text = Format.Angka(ringkasan.JumlahWilayah);
            });
        }

        private async void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            await Navigasi.TampilkanPesanAsync(XamlRoot, "Belum tersedia", "Fitur edit profile belum tersedia.");
        }

        private void Sidebar_MenuDipilih(object? sender, string menu)
        {
            if (menu == "Profile") return;
            Navigasi.KeMenuSidebar(Frame, menu);
        }
    }
}
