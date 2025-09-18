// Inmobiliaria/Repositories/IPagoRepository.cs
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IPagoRepository
    {
        Task<(IEnumerable<Pago> items, int total)> GetPagedAsync(int page, int pageSize, int? contratoId = null);
        Task<Pago?> GetByIdAsync(long id);
        Task<long> CreateAsync(Pago p);
        Task<bool> UpdateAsync(Pago p);
        Task<bool> AnularAsync(long id, string anuladoPor);
        Task<bool> ExistsByContratoAndNumeroAsync(int contratoId, int numeroPago, long? excludeId = null);
    }
}
