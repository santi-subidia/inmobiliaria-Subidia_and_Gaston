using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IContratoRepository
    {
        Task<IEnumerable<Contrato>> GetAllAsync();
        Task<Contrato?> GetByIdAsync(long id);
        Task<bool> DeleteAsync(long idContrato, long idUsuario);
        Task<long> CreateAsync(Contrato contrato);
        Task<bool> UpdateAsync(Contrato contrato);
        Task<(IEnumerable<Contrato> Items, int TotalCount)> GetPagedWithFiltersAsync(int page, int pageSize, string? tipoEstado = null, long? propietarioId = null, long? inmuebleId = null, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null);
        Task<IEnumerable<Contrato>> GetVigentesEnRangoAsync(DateOnly fechaDesde, DateOnly fechaHasta);
        Task<IEnumerable<Contrato>> GetProximosVencerAsync(int dias);
        Task<IEnumerable<Contrato>> GetByInquilinoIdAsync(long inquilinoId);
        Task<IEnumerable<Contrato>> GetByInmuebleIdAsync(long inmuebleId);
        Task<IEnumerable<Contrato>> GetProximosAVencerAsync(int dias = 30);
        Task<bool> RescindirContratoAsync(long id, long finalizadoPor, DateTime fechaFinEfectiva);
    }
}