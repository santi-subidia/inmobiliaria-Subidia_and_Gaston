using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface ITipoInmuebleRepository : IBaseRepository<TipoInmueble>
    {
        Task<TipoInmueble?> GetByNombreAsync(string nombre);
        Task<bool> UpdateFechaEliminacionAsync(long id); // para reactivar si fuese necesario, igual que usaste en Inquilino
    }
}
