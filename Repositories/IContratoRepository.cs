using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IContratoRepository : IBaseRepository<Contrato>
    {
        Task<(IEnumerable<Contrato> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? tipoEstado = null, bool noEliminado = true);
        Task<(IEnumerable<Contrato> Items, int TotalCount)> GetPagedWithFiltersAsync(int page, int pageSize, string? tipoEstado = null, long? propietarioId = null, long? inmuebleId = null, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null);
        Task<IEnumerable<Contrato>> GetVigentesEnRangoAsync(DateOnly fechaDesde, DateOnly fechaHasta);
        Task<IEnumerable<Contrato>> GetProximosVencerAsync(int dias);
        Task<IEnumerable<Contrato>> GetByInquilinoIdAsync(long inquilinoId);
        Task<IEnumerable<Contrato>> GetByInmuebleIdAsync(long inmuebleId);
        Task<IEnumerable<Contrato>> GetVigentesAsync();
        Task<IEnumerable<Contrato>> GetFinalizadosAsync();
        Task<IEnumerable<Contrato>> GetRescindidosAsync();
        Task<IEnumerable<Contrato>> GetProximosAVencerAsync(int dias = 30);
        Task<Contrato?> GetContratoVigenteByInmuebleAsync(long inmuebleId);
        Task<bool> FinalizarContratoAsync(long id, long finalizadoPor, DateTime? fechaFinEfectiva = null);
        Task<bool> RescindirContratoAsync(long id, long finalizadoPor, DateTime? fechaFinEfectiva = null);
        Task<bool> ExisteContratoVigenteParaInmuebleAsync(long inmuebleId);
        Task<bool> ExisteContratoVigenteParaInquilinoAsync(long inquilinoId);
    }
}