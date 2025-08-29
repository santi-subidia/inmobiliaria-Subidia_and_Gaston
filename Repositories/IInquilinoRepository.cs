using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IInquilinoRepository : IBaseRepository<Inquilino>
    {
        Task<(Inquilino?, Propietario?)> GetInquilinoAndPropietarioAsync(string dni);
    }
}
