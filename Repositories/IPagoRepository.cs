// Inmobiliaria/Repositories/IPagoRepository.cs
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IPagoRepository
    {
        Task<(IEnumerable<Pago> items, int total)> GetPagedAsync(int page, int pageSize, long? contratoId = null);
        Task<Pago?> GetByIdAsync(long id);
        Task<long> CreateAsync(Pago p);
        Task<bool> UpdateAsync(Pago p);
        Task<bool> AnularAsync(long id, string anuladoPor);
        Task<bool> ExistsByContratoAndNumeroAsync(long contratoId, int numeroPago, long? excludeId = null);
        Task<IEnumerable<Pago>> GetByContratoIdAsync(long contratoId);
        Task<bool> UpdateEstadoAsync(long id, string estado);
        Task<(int cantidadPagos, decimal montoPagado)> GetMontoPagadoAndCantidadPagosByContratoAsync(long contratoId);
        Task<int> GetCantidadPagosByContratoAsync(long contratoId);
    }
}
