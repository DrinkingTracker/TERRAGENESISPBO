using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Imaging;

namespace TERRAGENESIS.Pages
{
    public sealed partial class LoginPage : Page
    {
        private bool _passwordTerlihat = false;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void MasukButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                PesanTextBlock.Text = "Email/username dan password wajib diisi.";
                return;
            }

            // Sementara: database belum dihubungkan
            PesanTextBlock.Text = "";
            // TODO: panggil AuthService.Login(email, password) setelah database siap
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
            // TODO: RootFrame.Navigate(typeof(RegisterPage)) setelah RegisterPage dibuat
            PesanTextBlock.Text = "Halaman daftar belum dibuat.";
        }
    }
}