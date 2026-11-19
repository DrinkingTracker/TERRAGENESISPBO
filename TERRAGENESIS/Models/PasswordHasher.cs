using System;
using System.Security.Cryptography;
using System.Text;

namespace TERRAGENESIS.Models
{
    /// <summary>
    /// Hash password PBKDF2-SHA256. Format yang disimpan di <c>pengguna.password_hash</c>:
    /// <c>pbkdf2$iterasi$saltBase64$hashBase64</c>.
    /// </summary>
    public static class PasswordHasher
    {
        private const int Iterasi = 100_000;
        private const int UkuranSalt = 16;
        private const int UkuranHash = 32;

        public static string Buat(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(UkuranSalt);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password), salt, Iterasi, HashAlgorithmName.SHA256, UkuranHash);
            return $"pbkdf2${Iterasi}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool Cocok(string password, string tersimpan)
        {
            string[] bagian = tersimpan.Split('$');
            if (bagian.Length != 4 || bagian[0] != "pbkdf2" || !int.TryParse(bagian[1], out int iterasi))
            {
                return false;
            }

            try
            {
                byte[] salt = Convert.FromBase64String(bagian[2]);
                byte[] hashTersimpan = Convert.FromBase64String(bagian[3]);
                byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(password), salt, iterasi, HashAlgorithmName.SHA256, hashTersimpan.Length);
                return CryptographicOperations.FixedTimeEquals(hash, hashTersimpan);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
