using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using TERRAGENESIS.Models;
using TERRAGENESIS.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace TERRAGENESIS.Pages
{
    /// <summary>Form donasi untuk satu petak: jumlah pohon, data donatur, metode, dan bukti bayar.</summary>
    public sealed partial class DonasiPage : Page
    {
        private const int JumlahMaksimal = 9999;

        // Foto lahan per wilayah; Sulawesi dan Papua belum punya foto sendiri.
        private static readonly Dictionary<string, string> FotoWilayah = new()
        {
            ["Jawa Timur"] = "ms-appx:///Assets/Donasi/jawatimur.png",
            ["Kalimantan"] = "ms-appx:///Assets/Donasi/kalimantan.jpeg",
            ["Sumatra"] = "ms-appx:///Assets/Donasi/sumaterautara.jpeg",
        };
        private const string FotoCadangan = "ms-appx:///Assets/beranda/Frame%209.png";

        private static readonly Dictionary<string, string> IkonMetode = new()
        {
            ["Transfer Bank"] = "ms-appx:///Assets/Donasi/bankicon.png",
            ["E-Wallet"] = "ms-appx:///Assets/Donasi/walleticon.png",
            ["QRIS"] = "ms-appx:///Assets/Donasi/qricon.png",
        };

        private readonly Dictionary<MetodePembayaran, Border> _barisMetode = new();
        private int _petakId;
        private Petak? _petak;
        private Pohon? _pohon;
        private int _jumlah = 1;
        private MetodePembayaran? _metode;
        private StorageFile? _bukti;
        private bool _sedangMengirim;

        public DonasiPage()
        {
            InitializeComponent();
            Loaded += DonasiPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _petakId = e.Parameter is int id ? id : 0;
        }

        private async void DonasiPage_Loaded(object sender, RoutedEventArgs e)
        {
            Pengguna? pengguna = Navigasi.PastikanLogin(Frame);
            if (pengguna is null) return;

            if (_petakId == 0)
            {
                Frame.Navigate(typeof(PetaWilayahPage), PetaWilayahPage.PesanPilihPetak);
                return;
            }

            IReadOnlyList<MetodePembayaran> daftarMetode = [];
            bool berhasil = await Navigasi.JalankanAsync(XamlRoot, async () =>
            {
                _petak = await Layanan.Lahan.PetakByIdAsync(_petakId);
                _pohon = await Layanan.Donasi.PohonAsync();
                daftarMetode = await Layanan.Donasi.MetodePembayaranAsync();
            });
            if (!berhasil || _petak is null || _pohon is null) return;

            if (!_petak.BisaDipilih)
            {
                Sesi.PetakTerpilihId = null;
                await Navigasi.TampilkanPesanAsync(XamlRoot, "Petak tidak tersedia",
                    $"Petak #{_petak.Nomor} di {_petak.NamaWilayah} sudah dipilih donatur lain. Silakan pilih petak lain.");
                Frame.Navigate(typeof(LahanPage), _petak.NamaWilayah);
                return;
            }

            TxtLabelPetak.Text = _petak.Label;
            BrushFotoLahan.ImageSource = new BitmapImage(new Uri(FotoWilayah.GetValueOrDefault(_petak.NamaWilayah, FotoCadangan)));
            TxtHargaPohon.Text = $"Harga untuk setiap pohonnya adalah {Format.Rupiah(_pohon.Harga)}";

            foreach (MetodePembayaran metode in daftarMetode)
            {
                DaftarMetode.Children.Add(BuatBarisMetode(metode));
            }

            PerbaruiTotal();
        }

        private Button BuatBarisMetode(MetodePembayaran metode)
        {
            var isi = new Grid { Padding = new Thickness(25, 0, 25, 0) };
            isi.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(74) });
            isi.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            isi.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var ikon = new Image
            {
                Source = new BitmapImage(new Uri(IkonMetode.GetValueOrDefault(metode.Metode, IkonMetode["Transfer Bank"]))),
                Width = 48,
                Height = 48,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            var nama = new TextBlock
            {
                Text = metode.Metode,
                FontFamily = (FontFamily)Resources["FontBlack"],
                FontSize = 34,
                Foreground = new SolidColorBrush(Colors.Black),
                VerticalAlignment = VerticalAlignment.Center,
            };
            var panah = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Donasi/arrowicon.png")),
                Width = 46,
                Height = 46,
            };
            Grid.SetColumn(nama, 1);
            Grid.SetColumn(panah, 2);
            isi.Children.Add(ikon);
            isi.Children.Add(nama);
            isi.Children.Add(panah);

            var bingkai = new Border
            {
                Height = 82,
                CornerRadius = new CornerRadius(6),
                BorderThickness = new Thickness(1),
                Child = isi,
            };
            _barisMetode[metode] = bingkai;
            TandaiMetode(bingkai, false);

            var tombol = new Button
            {
                Content = bingkai,
                Padding = new Thickness(0),
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(Colors.Transparent),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Tag = metode,
            };
            AutomationProperties.SetName(tombol, $"Bayar dengan {metode.Metode}");
            tombol.Click += Metode_Click;
            return tombol;
        }

        private static void TandaiMetode(Border bingkai, bool dipilih)
        {
            bingkai.Background = new SolidColorBrush(dipilih ? ColorHelper.FromArgb(255, 0xF1, 0xFA, 0xEF) : Colors.White);
            bingkai.BorderBrush = new SolidColorBrush(dipilih
                ? ColorHelper.FromArgb(255, 0x2E, 0x7D, 0x32)
                : ColorHelper.FromArgb(255, 0xE3, 0xE7, 0xE4));
            bingkai.BorderThickness = new Thickness(dipilih ? 2 : 1);
        }

        private void Metode_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: MetodePembayaran metode }) return;

            _metode = metode;
            foreach (var (m, bingkai) in _barisMetode)
            {
                TandaiMetode(bingkai, m == metode);
            }
            PerbaruiInstruksi();
        }

        private void BtnKurang_Click(object sender, RoutedEventArgs e)
        {
            if (_jumlah > 1)
            {
                _jumlah--;
                PerbaruiTotal();
            }
        }

        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            if (_jumlah < JumlahMaksimal)
            {
                _jumlah++;
                PerbaruiTotal();
            }
        }

        private int Total => _pohon is null ? 0 : _jumlah * _pohon.Harga;

        private void PerbaruiTotal()
        {
            TxtJumlah.Text = _jumlah.ToString();
            TxtTotal.Text = Format.Rupiah(Total);
            BtnKurang.IsEnabled = _jumlah > 1;
            PerbaruiInstruksi();
        }

        private void PerbaruiInstruksi()
        {
            if (_metode is null) return;

            // Polymorphism: tiap metode punya instruksi sendiri lewat AmbilInstruksi().
            string[] langkah = _metode.AmbilInstruksi(Total);
            TxtInstruksi.Text = string.Join("\n", langkah.Select((teks, i) => $"{i + 1}.  {teks}"));
        }

        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail,
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            WinRT.Interop.InitializeWithWindow.Initialize(picker,
                WinRT.Interop.WindowNative.GetWindowHandle(App.JendelaUtama));

            StorageFile? file = await picker.PickSingleFileAsync();
            if (file is null) return;

            bool valid = await Navigasi.JalankanAsync(XamlRoot, () => Layanan.Donasi.ValidasiBuktiAsync(file));
            if (!valid) return;

            _bukti = file;
            using (var stream = await file.OpenReadAsync())
            {
                var gambar = new BitmapImage();
                await gambar.SetSourceAsync(stream);
                GambarBukti.Source = gambar;
            }
            GambarBukti.Height = 76;
            TxtUpload.Text = $"{file.Name}\n(klik untuk mengganti)";
        }

        private async void BtnKonfirmasi_Click(object sender, RoutedEventArgs e)
        {
            Pengguna? pengguna = Navigasi.PastikanLogin(Frame);
            if (pengguna is null || _petak is null || _sedangMengirim) return;

            _sedangMengirim = true;
            BtnKonfirmasi.IsEnabled = false;
            try
            {
                bool berhasil = await Navigasi.JalankanAsync(XamlRoot, () =>
                    Layanan.Donasi.BuatDonasiAsync(pengguna, _petak.Id, _jumlah, NamaTextBox.Text,
                        PesanTextBox.Text, _metode, _bukti));
                if (!berhasil) return;

                Sesi.PetakTerpilihId = null;
                await Navigasi.TampilkanPesanAsync(XamlRoot, "Donasi terkirim",
                    $"Terima kasih! Donasi {_jumlah} pohon untuk {_petak.Label} sebesar {Format.Rupiah(Total)} " +
                    "sudah tercatat dan menunggu verifikasi admin.");
                Frame.Navigate(typeof(DonasiSayaPage));
            }
            finally
            {
                _sedangMengirim = false;
                BtnKonfirmasi.IsEnabled = true;
            }
        }

        private void Navbar_NavigateRequested(object? sender, string halaman)
        {
            if (halaman == "Donasi") return;
            Navigasi.KeMenuNavbar(Frame, halaman);
        }
    }
}
