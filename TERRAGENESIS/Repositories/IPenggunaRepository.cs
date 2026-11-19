using System.Collections.Generic;
using System.Threading.Tasks;
using TERRAGENESIS.Models;

namespace TERRAGENESIS.Repositories
{
    public interface IPenggunaRepository
    {
        Task<Pengguna?> CariByEmailAsync(string email);
        Task<Pengguna?> CariByIdAsync(int id);
    }
}
