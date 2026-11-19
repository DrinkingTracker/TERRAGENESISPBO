using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace TERRAGENESIS.Services
{
    /// <summary>
    /// Menyalin foto (bukti bayar, dokumentasi) ke folder data aplikasi. Validasi format dan ukuran
    /// dilakukan di sini, bukan di database ("JPG/PNG, maksimal 5MB" di halaman Donasi).
    /// </summary>
    public sealed class PenyimpananFoto
    {
        public const ulong UkuranMaksimal = 5 * 1024 * 1024;
        private static readonly string[] EkstensiBoleh = [".jpg", ".jpeg", ".png"];

        public static bool FormatDidukung(string namaFile) =>
            EkstensiBoleh.Contains(Path.GetExtension(namaFile).ToLowerInvariant());

        public async Task ValidasiAsync(StorageFile foto)
        {
            if (!FormatDidukung(foto.Name))
            {
                throw new LayananException("Format foto harus JPG atau PNG.");
            }

            var properti = await foto.GetBasicPropertiesAsync();
            if (properti.Size > UkuranMaksimal)
            {
                throw new LayananException("Ukuran foto maksimal 5MB.");
            }
        }

        /// <summary>Menyalin foto ke subfolder (mis. "bukti") dan mengembalikan path lengkap salinannya.</summary>
        public async Task<string> SimpanAsync(StorageFile foto, string subfolder)
        {
            await ValidasiAsync(foto);

            StorageFolder folder = await ApplicationData.Current.LocalFolder
                .CreateFolderAsync(subfolder, CreationCollisionOption.OpenIfExists);
            string nama = $"{Guid.NewGuid():N}{Path.GetExtension(foto.Name).ToLowerInvariant()}";
            StorageFile salinan = await foto.CopyAsync(folder, nama, NameCollisionOption.GenerateUniqueName);
            return salinan.Path;
        }

        public void Hapus(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException)
            {
                // File sisa tidak mengganggu data; abaikan.
            }
        }
    }
}
