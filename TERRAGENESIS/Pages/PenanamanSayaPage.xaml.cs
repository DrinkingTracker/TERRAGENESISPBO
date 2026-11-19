using System;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using TERRAGENESIS.Models;
using TERRAGENESIS.Services;
using Windows.Storage;

namespace TERRAGENESIS.Pages
{
    /// <summary>
    /// Timeline satu donasi (terverifikasi → ditanam → selesai) dan foto dokumentasinya.
    /// Parameter navigasi: id donasi dari Donasi Saya; tanpa parameter, tampilkan donasi aktif terbaru.
    /// </summary>
    public sealed partial class PenanamanSayaPage : Page
    {
        private static readonly SolidColorBrush Hijau = new(ColorHelper.FromArgb(255, 0x0D, 0xBA, 0x2A));
        private static readonly SolidColorBrush HijauMuda = new(ColorHelper.FromArgb(255, 0xC9, 0xE8, 0xCC));

        private int? _donasiId;

        public PenanamanSayaPage()
        {
            InitializeComponent();
            Loaded += PenanamanSayaPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _donasiId = e.Parameter as int?;
        }

        private async void PenanamanSayaPage_Loaded(object sender, RoutedEventArgs e)
        {
            Pengguna? pengguna = Navigasi.PastikanLogin(Frame);
            if (pengguna is null) return;

            Donasi? donasi = null;
            Petak? petak = null;
            IReadOnlyList<Dokumentasi> dokumentasi = [];

            bool berhasil = await Navigasi.JalankanAsync(XamlRoot, async () =>
            {
                donasi = await Layanan.Donasi.DonasiUntukPenanamanAsync(pengguna, _donasiId);
                if (donasi is not null)
                {
                    petak = await Layanan.Lahan.PetakByIdAsync(donasi.PetakId);
                    dokumentasi = await Layanan.Donasi.DokumentasiAsync(donasi);
                }
            });
            if (!berhasil) return;

            TampilkanTimeline(donasi, petak);
            await TampilkanDokumentasiAsync(dokumentasi);
        }

        private void TampilkanTimeline(Donasi? donasi, Petak? petak)
        {
            if (donasi is null || petak is null)
            {
                TxtPetak.Text = "Belum ada penanaman. Donasi yang dikirim akan muncul di sini.";
            }
            else
            {
                string keterangan = donasi.Status switch
                {
                    StatusDonasi.Menunggu => "menunggu verifikasi admin",
                    StatusDonasi.Ditolak => "donasi ditolak admin",
                    _ => donasi.Status.KeTampilan().ToLowerInvariant(),
                };
                TxtPetak.Text = $"{petak.Label} · {donasi.JumlahPohon} pohon · {keterangan}";
            }

            // Donasi ditolak tidak pernah melewati tahap mana pun.
            bool aktif = donasi is { Aktif: true };
            DateTime? tahap1 = aktif ? donasi!.DisetujuiPada : null;
            DateTime? tahap2 = aktif ? donasi!.DitanamPada : null;
            DateTime? tahap3 = aktif ? donasi!.SelesaiPada : null;

            IsiTahap(Tahap1, TxtTanggal1, tahap1, null);
            IsiTahap(Tahap2, TxtTanggal2, tahap2, "ms-appx:///Assets/user/leaf.png");
            IsiTahap(Tahap3, TxtTanggal3, tahap3, "ms-appx:///Assets/user/propagule.png");
            Garis1.Fill = tahap2 is null ? HijauMuda : Hijau;
            Garis2.Fill = tahap3 is null ? HijauMuda : Hijau;
        }

        /// <summary>Tahap selesai: lingkaran hijau bercentang. Belum: lingkaran putih bergaris hijau dengan ikonnya.</summary>
        private static void IsiTahap(Grid wadah, TextBlock teksTanggal, DateTime? tanggal, string? ikon)
        {
            wadah.Children.Clear();
            bool selesai = tanggal is not null;

            wadah.Children.Add(new Microsoft.UI.Xaml.Shapes.Ellipse
            {
                Fill = selesai ? Hijau : new SolidColorBrush(Colors.White),
                Stroke = Hijau,
                StrokeThickness = selesai ? 0 : 3,
            });

            if (selesai || ikon is null)
            {
                wadah.Children.Add(new FontIcon
                {
                    Glyph = "", // centang
                    FontSize = 46,
                    Foreground = new SolidColorBrush(selesai ? Colors.White : HijauMuda.Color),
                });
            }
            else
            {
                wadah.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri(ikon)),
                    Width = 58,
                    Height = 58,
                });
            }

            teksTanggal.Text = selesai ? $"({Format.Tanggal(tanggal!.Value)})" : "(belum)";
        }

        private async System.Threading.Tasks.Task TampilkanDokumentasiAsync(IReadOnlyList<Dokumentasi> dokumentasi)
        {
            DaftarFoto.Children.Clear();

            foreach (Dokumentasi foto in dokumentasi)
            {
                try
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(foto.PathFoto);
                    using var stream = await file.OpenReadAsync();
                    var gambar = new BitmapImage();
                    await gambar.SetSourceAsync(stream);

                    var bingkai = new Border
                    {
                        Width = 280,
                        Height = 200,
                        CornerRadius = new CornerRadius(16),
                        Background = new ImageBrush { ImageSource = gambar, Stretch = Stretch.UniformToFill },
                    };
                    ToolTipService.SetToolTip(bingkai, $"Diunggah {Format.Tanggal(foto.DiunggahPada)}");
                    DaftarFoto.Children.Add(bingkai);
                }
                catch (Exception ex) when (ex is System.IO.FileNotFoundException or UnauthorizedAccessException or ArgumentException)
                {
                    // Foto hilang atau tidak bisa dibaca dari komputer ini; lewati saja.
                }
            }

            bool adaFoto = DaftarFoto.Children.Count > 0;
            ScrollFoto.Visibility = adaFoto ? Visibility.Visible : Visibility.Collapsed;
            KosongDokumentasi.Visibility = adaFoto ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Sidebar_MenuDipilih(object? sender, string menu)
        {
            if (menu == "Penanaman Saya" && _donasiId is null) return;
            Navigasi.KeMenuSidebar(Frame, menu);
        }
    }
}
