using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Npgsql;
using TERRAGENESIS.Models;
using TERRAGENESIS.Services;

namespace TERRAGENESIS.Pages
{
    public sealed partial class LoginPage : Page
    {
        private bool _passwordTerlihat = false;
        private bool _sedangMasuk = false;

        public LoginPage()
        {
            InitializeComponent();

            // "Ingat Saya": isi kembali email yang terakhir diingat.
            string? emailDiingat = Layanan.Auth.EmailDiingat();
            if (emailDiingat is not null)
            {
                EmailTextBox.Text = emailDiingat;
                IngatSayaCheckBox.IsChecked = true;
            }
        }

        private async void MasukButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sedangMasuk) return;

            string email = EmailTextBox.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                PesanTextBlock.Text = "Email dan password wajib diisi.";
                return;
            }

            _sedangMasuk = true;
            MasukButton.IsEnabled = false;
            PesanTextBlock.Text = "";
            try
            {
                Pengguna pengguna = await Layanan.Auth.LoginAsync(email, password);
                Layanan.Auth.IngatEmail(IngatSayaCheckBox.IsChecked == true ? email : null);

                if (pengguna is Admin)
                {
                    // Area admin belum dibuat; jangan tinggalkan sesi admin menggantung.
                    Layanan.Auth.Logout();
                    PesanTextBlock.Text = "Halaman admin belum tersedia.";
                    return;
                }

                Frame.Navigate(typeof(BerandaPage));
                Frame.BackStack.Clear();
            }
            catch (LayananException ex)
            {
                PesanTextBlock.Text = ex.Message;
            }
            catch (NpgsqlException)
            {
                PesanTextBlock.Text = "Tidak bisa terhubung ke database. Pastikan PostgreSQL berjalan.";
            }
            finally
            {
                _sedangMasuk = false;
                MasukButton.IsEnabled = true;
            }
        }

        private void PasswordInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                MasukButton_Click(sender, e);
            }
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _passwordTerlihat = !_passwordTerlihat;

            PasswordInput.PasswordRevealMode = _passwordTerlihat
                ? PasswordRevealMode.Visible
                : PasswordRevealMode.Hidden;

            string ikon = _passwordTerlihat
                ? "ms-appx:///Assets/login/eye.png"
                : "ms-appx:///Assets/login/eye-off.png";
            EyeImage.Source = new BitmapImage(new Uri(ikon));
        }

        private void LupaPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            PesanTextBlock.Text = "Fitur lupa password belum tersedia.";
        }

        private void DaftarLink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            PesanTextBlock.Text = "Halaman daftar belum dibuat.";
        }
    }
}
