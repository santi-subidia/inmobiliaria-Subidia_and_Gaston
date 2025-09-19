using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IImagenRepository : IBaseRepository<Imagen>
    {
        Task<IEnumerable<Imagen>> GetByInmuebleIdAsync(int inmuebleId);
    }
}