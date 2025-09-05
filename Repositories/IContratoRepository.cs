using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IContratoRepository : IBaseRepository<Contrato>
    {
        Task<IEnumerable<Contrato>> GetByInquilinoIdAsync(long inquilinoId);
        Task<IEnumerable<Contrato>> GetByInmuebleIdAsync(long inmuebleId);
        Task<IEnumerable<Contrato>> GetByEstadoAsync(EstadoContrato estado);
        Task<IEnumerable<Contrato>> GetVigentesAsync();
        Task<IEnumerable<Contrato>> GetProximosAVencerAsync(int dias = 30);
        Task<Contrato?> GetContratoVigenteByInmuebleAsync(long inmuebleId);
        Task<bool> FinalizarContratoAsync(long id, long finalizadoPor, DateTime? fechaFinEfectiva = null);
        Task<bool> RescindirContratoAsync(long id, long finalizadoPor, DateTime? fechaFinEfectiva = null);
        Task<bool> ExisteContratoVigenteParaInmuebleAsync(long inmuebleId);
        Task<bool> ExisteContratoVigenteParaInquilinoAsync(long inquilinoId);
    }
}