using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

namespace TERRAGENESIS.Pages
{
    /// <summary>Peta wilayah: memilih satu dari lima wilayah, lalu membuka grid petaknya.</summary>
    public sealed partial class PetaWilayahPage : Page
    {
        /// <summary>Parameter navigasi saat menu "Donasi" dibuka sebelum petak dipilih.</summary>
        public const string PesanPilihPetak = "pilih-petak";

        public PetaWilayahPage()
        {
            InitializeComponent();
            Loaded += (_, _) => Navigasi.PastikanLogin(Frame);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter as string == PesanPilihPetak)
            {
                TxtSubjudul.Text = "Pilih wilayah dan petak kosong dulu sebelum berdonasi.";
            }
        }

        private void Navbar_NavigateRequested(object? sender, string halaman)
        {
            if (halaman == "Lahan") return;
            Navigasi.KeMenuNavbar(Frame, halaman);
        }

        private void Label_Click(object sender, RoutedEventArgs e) => BukaWilayah(sender);

        private void Area_Tapped(object sender, TappedRoutedEventArgs e) => BukaWilayah(sender);

        private void BukaWilayah(object sender)
        {
            if (sender is FrameworkElement { Tag: string wilayah })
            {
                Frame.Navigate(typeof(LahanPage), wilayah);
            }
        }
    }
}
