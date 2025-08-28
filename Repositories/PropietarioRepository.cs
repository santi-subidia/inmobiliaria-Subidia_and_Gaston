using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using Microsoft.EntityFrameworkCore;

namespace Inmobiliaria.Repositories
{
    public class PropietarioRepository : IPropietarioRepository
    {
        private readonly InmobiliariaContext _context;

        public PropietarioRepository(InmobiliariaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Propietario>> GetAllAsync()
        {
            return await _context.Propietarios
                .Where(p => p.FechaEliminacion == null) // solo activos
                .ToListAsync();
        }

        public async Task<Propietario?> GetByIdAsync(long id)
        {
            return await _context.Propietarios
                .FirstOrDefaultAsync(p => p.Id == id && p.FechaEliminacion == null);
        }

        public async Task<long> CreateAsync(Propietario propietario)
        {
            _context.Propietarios.Add(propietario);
            await _context.SaveChangesAsync();
            return propietario.Id;
        }

        public async Task<bool> UpdateAsync(Propietario propietario)
        {
            var existing = await _context.Propietarios.FindAsync(propietario.Id);
            if (existing == null || existing.FechaEliminacion != null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(propietario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var propietario = await _context.Propietarios.FindAsync(id);
            if (propietario == null || propietario.FechaEliminacion != null)
                return false;

            propietario.FechaEliminacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
