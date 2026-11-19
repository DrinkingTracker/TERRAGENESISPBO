using System.Collections.Generic;
using System.Threading.Tasks;
using TERRAGENESIS.Models;

namespace TERRAGENESIS.Repositories
{
    public interface IReferensiRepository
    {
        Task<Pohon> PohonAsync();
        Task<IReadOnlyList<MetodePembayaran>> SemuaMetodeAsync();
    }
}
