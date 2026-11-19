using System.Collections.Generic;
using System.Numerics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TERRAGENESIS.Controls;
using TERRAGENESIS.Models;
using TERRAGENESIS.Services;

namespace TERRAGENESIS.Pages
{
    /// <summary>Beranda donatur: statistik dan kartu Lahan Unggulan diambil dari database.</summary>
    public sealed partial class BerandaPage : Page
    {
        // Judul kartu mengikuti Figma ("Sumatera Utara"), nama di database mengikuti label peta ("Sumatra").
        private static readonly Dictionary<string, string> NamaWilayahKartu = new()
        {
            ["Jawa Timur"] = "Jawa Timur",
            ["Kalimantan"] = "Kalimantan",
            ["Sumatera Utara"] = "Sumatra",
        };

        public BerandaPage()
        {
            InitializeComponent();

            // Receiver bayangan baru boleh dipasang setelah elemennya masuk visual tree,
            // jadi dikerjakan di Loaded, bukan di konstruktor.
            Loaded += BerandaPage_Loaded;
        }

        private async void BerandaPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Bayangan hanya hiasan: kalau gagal, halaman tetap jalan tanpa bayangan.
            try
            {
                var bayangan = new ThemeShadow();
                bayangan.Receivers.Add(Kanvas);
                KartuStatistik.Shadow = bayangan;
                KartuStatistik.Translation = new Vector3(0, 0, 32);

                CardJatim.PasangReceiver(Kanvas);
                CardKalimantan.PasangReceiver(Kanvas);
                CardSumut.PasangReceiver(Kanvas);
            }
            catch (System.ArgumentException)
            {
            }

            if (Navigasi.PastikanLogin(Frame) is null) return;

            await Navigasi.JalankanAsync(XamlRoot, async () =>
            {
                StatistikBeranda statistik = await Layanan.Lahan.StatistikBerandaAsync();
                TxtTotalPohon.Text = Format.Angka(statistik.TotalPohonTertanam);
                TxtDonatur.Text = Format.Angka(statistik.JumlahDonatur);
                TxtWilayah.Text = Format.Angka(statistik.WilayahTerhijaukan);

                IReadOnlyDictionary<string, int> slot = await Layanan.Lahan.SlotTersediaAsync();
                foreach (LahanCard kartu in new[] { CardJatim, CardKalimantan, CardSumut })
                {
                    kartu.JumlahSlot = slot.GetValueOrDefault(NamaWilayahKartu[kartu.Judul]);
                }
            });
        }

        private void Navbar_NavigateRequested(object? sender, string halaman)
        {
            if (halaman == "Beranda") return;
            Navigasi.KeMenuNavbar(Frame, halaman);
        }

        private void BtnJelajah_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PetaWilayahPage));
        }

        private void Card_Dipilih(object? sender, string judul)
        {
            Frame.Navigate(typeof(LahanPage), NamaWilayahKartu[judul]);
        }
    }
}
