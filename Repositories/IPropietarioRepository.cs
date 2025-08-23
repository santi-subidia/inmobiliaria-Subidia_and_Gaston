using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IPropietarioRepository
    {
        Task<List<Propietario>> GetAllAsync(CancellationToken ct = default);
        Task<Propietario?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(Propietario p, CancellationToken ct = default);
        Task<bool> UpdateAsync(Propietario p, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
