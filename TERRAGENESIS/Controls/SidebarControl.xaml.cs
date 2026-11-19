using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace TERRAGENESIS.Controls
{
    /// <summary>
    /// Sidebar halaman akun donatur (Profile, Donasi Saya, Penanaman Saya, Keluar).
    /// Sama seperti NavbarControl: hanya memberi tahu halaman induk lewat MenuDipilih.
    /// </summary>
    public sealed partial class SidebarControl : UserControl
    {
        private static readonly SolidColorBrush LatarAktif = new(ColorHelper.FromArgb(255, 0x43, 0x89, 0x3A));

        /// <summary>
        /// Dipanggil dengan nama menu: "Profile", "Donasi Saya", "Penanaman Saya", "Keluar",
        /// atau "Beranda" (logo diklik).
        /// </summary>
        public event EventHandler<string>? MenuDipilih;

        public static readonly DependencyProperty ActiveMenuProperty =
            DependencyProperty.Register(
                nameof(ActiveMenu),
                typeof(string),
                typeof(SidebarControl),
                new PropertyMetadata(string.Empty, (d, _) => ((SidebarControl)d).PerbaruiMenuAktif()));

        /// <summary>Menu yang diberi latar hijau.</summary>
        public string ActiveMenu
        {
            get => (string)GetValue(ActiveMenuProperty);
            set => SetValue(ActiveMenuProperty, value);
        }

        public SidebarControl()
        {
            InitializeComponent();
            PerbaruiMenuAktif();
        }

        private void PerbaruiMenuAktif()
        {
            foreach (Button menu in new[] { MenuProfile, MenuDonasiSaya, MenuPenanamanSaya, MenuKeluar })
            {
                if ((string)menu.Tag == ActiveMenu)
                {
                    menu.Background = LatarAktif;
                }
                else
                {
                    menu.ClearValue(BackgroundProperty);
                }
            }
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement { Tag: string tujuan })
            {
                MenuDipilih?.Invoke(this, tujuan);
            }
        }
    }
}
