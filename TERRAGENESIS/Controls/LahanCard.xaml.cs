using System;
using System.Numerics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace TERRAGENESIS.Controls
{
    /// <summary>
    /// Kartu wilayah (foto, nama, jumlah slot). Hanya tampilan: data diberikan dari luar,
    /// sehingga tidak bergantung pada Models atau database.
    /// </summary>
    public sealed partial class LahanCard : UserControl
    {
        private int _jumlahSlot;

        /// <summary>Dipanggil saat kartu diklik, membawa nama wilayah (Judul).</summary>
        public event EventHandler<string>? Dipilih;

        public LahanCard()
        {
            InitializeComponent();

            // Bayangan kartu. Receiver dipasang oleh halaman lewat PasangReceiver.
            Root.Shadow = new ThemeShadow();
            Root.Translation = new Vector3(0, 0, 24);
        }

        public string Judul
        {
            get => TxtJudul.Text;
            set => TxtJudul.Text = value;
        }

        public int JumlahSlot
        {
            get => _jumlahSlot;
            set
            {
                _jumlahSlot = value;
                TxtSlot.Text = $"Tersedia {value} Slot";
            }
        }

        /// <summary>URI gambar, contoh: ms-appx:///Assets/beranda/lahan-jatim.png</summary>
        public string Gambar
        {
            set => BrushGambar.ImageSource = new BitmapImage(new Uri(value));
        }

        /// <summary>Elemen di belakang kartu yang menerima bayangan (mis. kanvas halaman).</summary>
        public void PasangReceiver(UIElement receiver)
        {
            if (Root.Shadow is ThemeShadow bayangan)
            {
                bayangan.Receivers.Add(receiver);
            }
        }

        private void Root_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dipilih?.Invoke(this, Judul);
        }
    }
}