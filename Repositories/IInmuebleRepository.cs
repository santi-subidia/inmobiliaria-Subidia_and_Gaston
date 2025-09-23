using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IInmuebleRepository : IBaseRepository<Inmueble>
    {
        Task<IEnumerable<Inmueble>> GetAllWithFiltersAsync(bool fecha_eliminacion = false, bool suspendido = false, bool disponible = false);
        Task<(IEnumerable<Inmueble> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, bool? suspendido = null, long? propietarioId = null);
        Task<IEnumerable<Inmueble>> GetByPropietarioAsync(long propietarioId);
        Task<bool> UpdateSuspendidoAsync(long id, bool suspendido);
        Task<bool> UpdateFechaEliminacionAsync(long id);
        Task<IEnumerable<Inmueble>> GetAllbyFechasAsync(DateOnly fechaInicio, DateOnly fechaFin, long? contratoId = null);
        Task<bool> UpdatePortadaAsync(long id, string? portadaUrl);
    }
}
