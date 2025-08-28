using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IPropietarioRepository
    {
        Task<IEnumerable<Propietario>> GetAllAsync();
        Task<Propietario?> GetByIdAsync(long id);
        Task<long> CreateAsync(Propietario propietario);
        Task<bool> UpdateAsync(Propietario propietario);
        Task<bool> DeleteAsync(long id); // soft delete con FechaEliminacion
    }
}
