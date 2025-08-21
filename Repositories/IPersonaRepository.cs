using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IPersonaRepository
    {
        Task<List<Persona>> GetAllAsync(CancellationToken ct = default);
        Task<Persona?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(Persona p, CancellationToken ct = default);
        Task<bool> UpdateAsync(Persona p, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
