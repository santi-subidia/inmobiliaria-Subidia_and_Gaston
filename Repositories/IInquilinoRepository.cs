using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IInquilinoRepository
    {
        Task<List<Inquilino>> GetAllAsync(CancellationToken ct = default);
        Task<Inquilino?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(Inquilino i, CancellationToken ct = default);
        Task<bool> UpdateAsync(Inquilino i, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}