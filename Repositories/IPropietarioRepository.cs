using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IPropietarioRepository
    {
        Task<IEnumerable<Propietario>> GetAllAsync();                        // solo activos (fecha_eliminacion IS NULL)
        Task<IEnumerable<Propietario>> GetAllWithFiltersAsync(bool activos); // true = sin eliminados; false = todos
        Task<(IEnumerable<Propietario> Items, int TotalCount)> GetPagedAsync(int page = 1, int pageSize = 10);
        Task<Propietario?> GetByIdAsync(long id);
        Task<Propietario?> GetByDniAsync(string dni);
        Task<long> CreateAsync(Propietario p);
        Task<bool> UpdateAsync(Propietario p);
        Task<bool> DeleteAsync(long id);                 // soft delete: setea fecha_eliminacion
        Task<bool> UpdateFechaEliminacionAsync(long id); // alias por compatibilidad (marca eliminado            // revierte soft delete (fecha_eliminacion = NULL)
    }
}
