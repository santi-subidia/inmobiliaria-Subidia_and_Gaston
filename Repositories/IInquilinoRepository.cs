using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IInquilinoRepository : IBaseRepository<Inquilino>
    {
        Task<(IEnumerable<Inquilino> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
        Task<Inquilino?> GetByDniAsync(string dni);
        Task<bool> UpdateFechaEliminacionAsync(long id);
    }
}
