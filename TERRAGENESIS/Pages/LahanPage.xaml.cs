using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using TERRAGENESIS.Models;
using TERRAGENESIS.Services;
using Windows.Foundation;
using Windows.UI;

namespace TERRAGENESIS.Pages
{
    /// <summary>Grid 10x10 petak satu wilayah, ringkasan, dan pop-up detail petak terisi.</summary>
    public sealed partial class LahanPage : Page
    {
        private const int Kolom = 10;
        private const double UkuranPetak = 52;
        private const double JarakPetak = 60; // 52 + celah 8

        private static readonly Color WarnaKosong = ColorHelper.FromArgb(255, 0xE8, 0xEE, 0xDF);
        private static readonly Color GarisKosong = ColorHelper.FromArgb(255, 0xD7, 0xE3, 0xCC);
        private static readonly Color WarnaTerisi = ColorHelper.FromArgb(255, 0xCD, 0xEF, 0xD3);
        private static readonly Color GarisTerisi = ColorHelper.FromArgb(255, 0x9E, 0xD9, 0xA9);
        private static readonly Color WarnaNonaktif = ColorHelper.FromArgb(255, 0xEE, 0xEE, 0xEE);
        private static readonly Color GarisDipilih = ColorHelper.FromArgb(255, 0x1F, 0x7A, 0x33);

        private readonly Dictionary<int, Border> _kotakPetak = new();
        private string _namaWilayah = string.Empty;
        private Petak? _petakDipilih;
        private int _versiPopup;

        public LahanPage()
        {
            InitializeComponent();
            Loaded += LahanPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _namaWilayah = e.Parameter as string ?? string.Empty;
            TxtWilayah.Text = _namaWilayah;
        }

        private async void LahanPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var bayangan = new ThemeShadow();
                bayangan.Receivers.Add(GridPetak);
                PopupDetail.Shadow = bayangan;
                PopupDetail.Translation = new Vector3(0, 0, 32);
            }
            catch (ArgumentException)
            {
            }

            if (Navigasi.PastikanLogin(Frame) is null) return;

            bool berhasil = await Navigasi.JalankanAsync(XamlRoot, async () =>
            {
                Wilayah wilayah = await Layanan.Lahan.WilayahAsync(_namaWilayah);
                IReadOnlyList<Petak> semuaPetak = await Layanan.Lahan.PetakAsync(wilayah);
                TampilkanGrid(semuaPetak);
            });

            if (!berhasil && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void TampilkanGrid(IReadOnlyList<Petak> semuaPetak)
        {
            GridPetak.Children.Clear();
            _kotakPetak.Clear();

            foreach (Petak petak in semuaPetak)
            {
                int indeks = petak.Nomor - 1;
                var kotak = BuatKotak(petak);
                Canvas.SetLeft(kotak, indeks % Kolom * JarakPetak);
                Canvas.SetTop(kotak, indeks / Kolom * JarakPetak);
                GridPetak.Children.Add(kotak);
                _kotakPetak[petak.Id] = kotak;
            }

            int terisi = semuaPetak.Count(p => p.Terisi);
            TxtTerisi.Text = terisi.ToString();
            BarTerisi.Maximum = Math.Max(semuaPetak.Count, 1);
            BarTerisi.Value = terisi;
        }

        private Border BuatKotak(Petak petak)
        {
            var kotak = new Border
            {
                Width = UkuranPetak,
                Height = UkuranPetak,
                CornerRadius = new CornerRadius(10),
                BorderThickness = new Thickness(1),
                Tag = petak,
            };

            if (!petak.Aktif)
            {
                kotak.Background = new SolidColorBrush(WarnaNonaktif);
                kotak.BorderBrush = new SolidColorBrush(WarnaNonaktif);
                ToolTipService.SetToolTip(kotak, $"Petak #{petak.Nomor} sedang tidak aktif");
                return kotak;
            }

            if (petak.Terisi)
            {
                kotak.Background = new SolidColorBrush(WarnaTerisi);
                kotak.BorderBrush = new SolidColorBrush(GarisTerisi);
                kotak.Child = new Image
                {
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/Lahan/treeicon.png")),
                    Width = 22,
                    Height = 22,
                };
            }
            else
            {
                kotak.Background = new SolidColorBrush(WarnaKosong);
                kotak.BorderBrush = new SolidColorBrush(GarisKosong);
            }

            ToolTipService.SetToolTip(kotak, $"Petak #{petak.Nomor}");
            kotak.Tapped += Kotak_Tapped;
            return kotak;
        }

        private async void Kotak_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true; // jangan sampai IsiKartu_Tapped ikut menutup pop-up
            if (sender is not Border { Tag: Petak petak } kotak) return;

            TandaiDipilih(kotak, petak);

            if (petak.BisaDipilih)
            {
                TutupPopup();
                BtnPilihArea.IsEnabled = true;
                TxtPilihArea.Text = $"Pilih Area (Petak #{petak.Nomor})";
                return;
            }

            BtnPilihArea.IsEnabled = false;
            TxtPilihArea.Text = "Pilih Area";

            int versi = ++_versiPopup;
            DetailPetak? detail = null;
            await Navigasi.JalankanAsync(XamlRoot, async () => detail = await Layanan.Lahan.DetailPetakAsync(petak));

            // Abaikan hasil lama bila pengguna sudah mengklik petak lain.
            if (versi != _versiPopup || detail is null) return;
            TampilkanPopup(petak, detail);
        }

        private void TandaiDipilih(Border kotak, Petak petak)
        {
            if (_petakDipilih is not null && _kotakPetak.TryGetValue(_petakDipilih.Id, out Border? lama))
            {
                lama.BorderBrush = new SolidColorBrush(_petakDipilih.Terisi ? GarisTerisi : GarisKosong);
                lama.BorderThickness = new Thickness(1);
            }

            _petakDipilih = petak;
            kotak.BorderBrush = new SolidColorBrush(GarisDipilih);
            kotak.BorderThickness = new Thickness(2.5);
        }

        private void TampilkanPopup(Petak petak, DetailPetak detail)
        {
            TxtPopupStatus.Text = $"Petak #{detail.NomorPetak} · {TeksStatus(detail.Status)}";
            TxtPopupTanggal.Text = Format.Tanggal(detail.TanggalDonasi);
            TxtPopupNama.Text = detail.NamaDonatur;
            TxtPopupLokasi.Text = $"{detail.NamaWilayah} · Petak #{detail.NomorPetak}";
            KotakPesan.Visibility = string.IsNullOrWhiteSpace(detail.Pesan) ? Visibility.Collapsed : Visibility.Visible;
            TxtPopupPesan.Text = $"“{detail.Pesan}”";

            // Pop-up di kanan petak; pindah ke kiri bila petaknya di sisi kanan grid.
            int indeks = petak.Nomor - 1;
            double kiriPetak = Canvas.GetLeft(GridPetak) + indeks % Kolom * JarakPetak;
            double atasPetak = Canvas.GetTop(GridPetak) + indeks / Kolom * JarakPetak;
            double kiri = indeks % Kolom < 6 ? kiriPetak + JarakPetak : kiriPetak - 8 - PopupDetail.Width;

            PopupDetail.Visibility = Visibility.Visible;
            PopupDetail.Measure(new Size(PopupDetail.Width, double.PositiveInfinity));
            double tinggi = PopupDetail.DesiredSize.Height;
            double atas = Math.Clamp(atasPetak - 6, 20, Math.Max(20, Kartu.Height - tinggi - 20));

            Canvas.SetLeft(PopupDetail, kiri);
            Canvas.SetTop(PopupDetail, atas);
        }

        private static string TeksStatus(StatusDonasi status) => status switch
        {
            StatusDonasi.Menunggu => "Menunggu verifikasi",
            StatusDonasi.Disetujui => "Disetujui, menunggu penanaman",
            StatusDonasi.Ditanam => "Sudah ditanam",
            StatusDonasi.Selesai => "Selesai ditanam",
            _ => status.KeTampilan(),
        };

        private void TutupPopup()
        {
            _versiPopup++;
            PopupDetail.Visibility = Visibility.Collapsed;
        }

        private void IsiKartu_Tapped(object sender, TappedRoutedEventArgs e) => TutupPopup();

        private void PopupDetail_Tapped(object sender, TappedRoutedEventArgs e) => e.Handled = true;

        private void BtnPilihArea_Click(object sender, RoutedEventArgs e)
        {
            if (_petakDipilih is not { BisaDipilih: true } petak) return;

            Sesi.PetakTerpilihId = petak.Id;
            Frame.Navigate(typeof(DonasiPage), petak.Id);
        }

        private void Kembali_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PetaWilayahPage));
        }

        private void Navbar_NavigateRequested(object? sender, string halaman)
        {
            Navigasi.KeMenuNavbar(Frame, halaman);
        }
    }
}
