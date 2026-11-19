using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TERRAGENESIS.Controls
{
    /// <summary>
    /// Navbar atas untuk area donatur. Tidak tahu apa-apa soal halaman lain:
    /// ia hanya memberi tahu halaman induk tujuan mana yang diklik lewat NavigateRequested.
    /// </summary>
    public sealed partial class NavbarControl : UserControl
    {
        // Titik tengah tiap menu (koordinat kanvas 1440 px dari Figma).
        private static readonly Dictionary<string, double> PusatMenu = new()
        {
            ["Beranda"] = 683,
            ["Lahan"] = 844,
            ["Donasi"] = 995,
            ["Profile"] = 1149.5,
        };

        /// <summary>Dipanggil dengan nama tujuan: "Beranda", "Lahan", "Donasi", atau "Profile".</summary>
        public event EventHandler<string>? NavigateRequested;

        public static readonly DependencyProperty ActivePageProperty =
            DependencyProperty.Register(
                nameof(ActivePage),
                typeof(string),
                typeof(NavbarControl),
                new PropertyMetadata(string.Empty, (d, _) => ((NavbarControl)d).PerbaruiGaris()));

        /// <summary>Menu yang diberi garis bawah. Kosongkan untuk menyembunyikan garis.</summary>
        public string ActivePage
        {
            get => (string)GetValue(ActivePageProperty);
            set => SetValue(ActivePageProperty, value);
        }

        public NavbarControl()
        {
            InitializeComponent();
            PerbaruiGaris();
        }

        private void PerbaruiGaris()
        {
            if (PusatMenu.TryGetValue(ActivePage ?? string.Empty, out double pusat))
            {
                Canvas.SetLeft(GarisAktif, pusat - GarisAktif.Width / 2);
                GarisAktif.Visibility = Visibility.Visible;
            }
            else
            {
                GarisAktif.Visibility = Visibility.Collapsed;
            }
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement { Tag: string tujuan })
            {
                NavigateRequested?.Invoke(this, tujuan);
            }
        }
    }
}