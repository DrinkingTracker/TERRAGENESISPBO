using System.Collections.Generic;
using System.Threading.Tasks;
using TERRAGENESIS.Models;
using TERRAGENESIS.Repositories;

namespace TERRAGENESIS.Services
{
    /// <summary>Wilayah, grid petak, dan detail petak terisi.</summary>
    public sealed class LahanService
    {
        private readonly IWilayahRepository _wilayah;
        private readonly IDonasiRepository _donasi;

        public LahanService(IWilayahRepository wilayah, IDonasiRepository donasi)
        {
            _wilayah = wilayah;
            _donasi = donasi;
        }

        public Task<IReadOnlyList<Wilayah>> SemuaWilayahAsync() => _wilayah.SemuaAsync();

        public async Task<Wilayah> WilayahAsync(string nama)
        {
            return await _wilayah.CariByNamaAsync(nama)
                ?? throw new LayananException($"Wilayah \"{nama}\" tidak ditemukan di database.");
        }

        public Task<IReadOnlyList<Petak>> PetakAsync(Wilayah wilayah) => _wilayah.PetakDiWilayahAsync(wilayah.Id);

        public async Task<Petak> PetakByIdAsync(int petakId)
        {
            return await _wilayah.CariPetakAsync(petakId)
                ?? throw new LayananException("Petak tidak ditemukan.");
        }

        /// <summary>Isi pop-up "Pohon Harapan"; null bila petak tidak sedang dipegang donasi.</summary>
        public Task<DetailPetak?> DetailPetakAsync(Petak petak) => _donasi.DetailDonasiAktifDiPetakAsync(petak.Id);

        /// <summary>Jumlah "Tersedia N Slot" per nama wilayah untuk kartu Beranda.</summary>
        public Task<IReadOnlyDictionary<string, int>> SlotTersediaAsync() => _wilayah.JumlahPetakKosongAsync();

        public Task<StatistikBeranda> StatistikBerandaAsync() => _donasi.StatistikBerandaAsync();

        /// <summary>Admin (Kelola Lahan): mengaktifkan atau menonaktifkan petak.</summary>
        public async Task UbahStatusPetakAsync(Pengguna oleh, int petakId, bool aktif)
        {
            if (oleh is not Admin)
            {
                throw new LayananException("Hanya admin yang boleh mengubah status lahan.");
            }
            await _wilayah.UbahStatusPetakAsync(petakId, aktif);
        }
    }
}
