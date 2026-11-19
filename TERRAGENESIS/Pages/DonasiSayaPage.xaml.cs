using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TERRAGENESIS.Models;
using TERRAGENESIS.Services;
using Windows.UI;

namespace TERRAGENESIS.Pages
{
    /// <summary>Tabel riwayat donasi milik donatur yang login.</summary>
    public sealed partial class DonasiSayaPage : Page
    {
        public DonasiSayaPage()
        {
            InitializeComponent();
            Loaded += DonasiSayaPage_Loaded;
        }

        private async void DonasiSayaPage_Loaded(object sender, RoutedEventArgs e)
        {
            Pengguna? pengguna = Navigasi.PastikanLogin(Frame);
            if (pengguna is null) return;

            await Navigasi.JalankanAsync(XamlRoot, async () =>
            {
                IReadOnlyList<RiwayatDonasi> riwayat = await Layanan.Donasi.RiwayatAsync(pengguna);
                DaftarBaris.Children.Clear();

                if (riwayat.Count == 0)
                {
                    DaftarBaris.Children.Add(new TextBlock
                    {
                        Text = "Belum ada donasi. Pilih petak di menu Lahan untuk mulai menanam.",
                        Margin = new Thickness(54, 18, 20, 18),
                        FontSize = 18,
                        Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 0x5F, 0x7A, 0x6D)),
                    });
                    return;
                }

                foreach (RiwayatDonasi baris in riwayat)
                {
                    DaftarBaris.Children.Add(BuatBaris(baris));
                }
            });
        }

        private Button BuatBaris(RiwayatDonasi baris)
        {
            var isi = new Grid { Height = 60, Padding = new Thickness(54, 0, 0, 0) };
            foreach (double lebar in new double[] { 289, 242, 243 })
            {
                isi.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(lebar) });
            }
            isi.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TambahSel(isi, 0, Format.Tanggal(baris.Tanggal));
            TambahSel(isi, 1, baris.NamaWilayah);
            TambahSel(isi, 2, Format.Rupiah(baris.Nominal));

            var (latar, teks) = WarnaStatus(baris.Status);
            var lencana = new Border
            {
                Background = new SolidColorBrush(latar),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 6, 14, 6),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock
                {
                    Text = baris.Status.KeTampilan(),
                    FontSize = 20,
                    FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                    Foreground = new SolidColorBrush(teks),
                },
            };
            Grid.SetColumn(lencana, 3);
            isi.Children.Add(lencana);

            var tombol = new Button
            {
                Content = isi,
                Padding = new Thickness(0),
                CornerRadius = new CornerRadius(0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Tag = baris.DonasiId,
            };
            ToolTipService.SetToolTip(tombol, $"Petak #{baris.NomorPetak} · {baris.JumlahPohon} pohon — klik untuk melihat penanaman");
            tombol.Click += Baris_Click;
            return tombol;
        }

        private static void TambahSel(Grid isi, int kolom, string teks)
        {
            var sel = new TextBlock
            {
                Text = teks,
                FontSize = 20,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetColumn(sel, kolom);
            isi.Children.Add(sel);
        }

        private static (Color Latar, Color Teks) WarnaStatus(StatusDonasi status) => status switch
        {
            StatusDonasi.Menunggu => (ColorHelper.FromArgb(255, 0xFF, 0xF1, 0xC9), ColorHelper.FromArgb(255, 0x7A, 0x5A, 0x00)),
            StatusDonasi.Ditolak => (ColorHelper.FromArgb(255, 0xFD, 0xDE, 0xDC), ColorHelper.FromArgb(255, 0xA3, 0x1E, 0x16)),
            _ => (ColorHelper.FromArgb(255, 0xCB, 0xF7, 0xC8), Colors.Black),
        };

        private void Baris_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: int donasiId })
            {
                Frame.Navigate(typeof(PenanamanSayaPage), donasiId);
            }
        }

        private void Sidebar_MenuDipilih(object? sender, string menu)
        {
            if (menu == "Donasi Saya") return;
            Navigasi.KeMenuSidebar(Frame, menu);
        }
    }
}
