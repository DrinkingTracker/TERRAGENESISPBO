using System.Collections.Generic;
using System.Threading.Tasks;
using TERRAGENESIS.Models;

namespace TERRAGENESIS.Repositories
{
    public interface IWilayahRepository
    {
        Task<IReadOnlyList<Wilayah>> SemuaAsync();
        Task<Wilayah?> CariByNamaAsync(string nama);

        /// <summary>100 petak satu wilayah, urut nomor, lengkap dengan status terisi.</summary>
        Task<IReadOnlyList<Petak>> PetakDiWilayahAsync(int wilayahId);

        Task<Petak?> CariPetakAsync(int petakId);

        /// <summary>Jumlah petak aktif yang masih kosong, per nama wilayah.</summary>
        Task<IReadOnlyDictionary<string, int>> JumlahPetakKosongAsync();

        /// <summary>Admin: menyalakan/mematikan petak (kolom <c>petak.status</c>).</summary>
        Task UbahStatusPetakAsync(int petakId, bool aktif);
    }
}
