using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IInmuebleRepository : IBaseRepository<Inmueble>
    {
        Task<IEnumerable<Inmueble>> GetAllWithFiltersAsync(bool fecha_eliminacion = false);
        Task<IEnumerable<Inmueble>> GetByPropietarioAsync(long propietarioId);
        Task<IEnumerable<Inmueble>> GetDisponiblesAsync();
        Task<bool> UpdateSuspendidoAsync(long id, bool suspendido);
        Task<bool> UpdateFechaEliminacionAsync(long id);
    }
}
