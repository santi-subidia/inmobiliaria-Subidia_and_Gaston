// Repositories/IUsuarioRepository.cs
using Inmobiliaria.Models;

namespace Inmobiliaria.Repositories
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(long id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email, long? excludeId = null);
        Task<long> CreateAsync(Usuario u);
        Task<bool> UpdateAsync(Usuario u);
        Task<bool> DeleteAsync(long id);
    }
}
