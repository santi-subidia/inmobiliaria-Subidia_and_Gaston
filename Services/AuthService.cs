using System.Security.Claims;
using System.Security.Cryptography;
using Inmobiliaria.Models;
using Inmobiliaria.Repositories;

namespace Inmobiliaria.Services
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
        Task<(bool ok, Usuario? user, string? error)> ValidateCredentialsAsync(string email, string password);
        ClaimsPrincipal CreatePrincipal(Usuario u);
    }

    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _repo;
        public AuthService(IUsuarioRepository repo) => _repo = repo;

        // PBKDF2: guarda "saltBase64:hashBase64"
        public string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string stored)
        {
            var parts = stored.Split(':');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var expected = Convert.FromBase64String(parts[1]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            var actual = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }

        public async Task<(bool ok, Usuario? user, string? error)> ValidateCredentialsAsync(string email, string password)
        {
            var u = await _repo.GetByEmailAsync(email);
            if (u is null) return (false, null, "Email o contraseña inválidos.");
            if (!u.IsActive) return (false, null, "Usuario inactivo.");

            if (!VerifyPassword(password, u.PasswordHash))
                return (false, null, "Email o contraseña inválidos.");

            return (true, u, null);
        }

        public ClaimsPrincipal CreatePrincipal(Usuario u)
        {
            var roleName = u.RolId switch
            {
                1 => "Administrador",
                2 => "Empleado",
                _ => "Usuario"
            };

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, u.Id.ToString()),
                new("full_name", u.NombreCompleto),
                new(ClaimTypes.Role, roleName),
                new("role_id", u.RolId.ToString()),
                new("avatar_url", u.AvatarUrl ?? ""),
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            return new ClaimsPrincipal(identity);
        }
    }
}