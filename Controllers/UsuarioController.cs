// Controllers/UsuarioController.cs
using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Inmobiliaria.Controllers
{
    // [Authorize] // requiere estar logueado por cookie
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _repo;
        private readonly IWebHostEnvironment _env;
        public UsuarioController(IUsuarioRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        // Listado simple (sin paginado)
        public async Task<IActionResult> Index()
        {
            var users = await _repo.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(long id)
        {
            var userId = id;

            var usuario = await _repo.GetByIdAsync(userId);
            if (usuario == null) return NotFound();

            ViewBag.RolNombre = GetRoleName(usuario.RolId); // Método auxiliar


            return View(usuario);
        }

        // Método auxiliar para obtener nombre del rol
        private string GetRoleName(int rolId)
        {
            return rolId switch
            {
                1 => "Administrador",
                2 => "Empleado",
                _ => "Sin rol definido"
            };
        }

        [HttpGet]
        public IActionResult Create() => View(new Usuario { IsActive = true });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario u, [FromServices] Services.IAuthService auth)
        {
            if (await _repo.ExistsByEmailAsync(u.Email))
                ModelState.AddModelError(nameof(u.Email), "Ya existe un usuario con ese email.");
            if (string.IsNullOrWhiteSpace(u.PasswordHash))
                ModelState.AddModelError(nameof(u.PasswordHash), "La contraseña es obligatoria.");

            if (!ModelState.IsValid) return View(u);

            u.PasswordHash = auth.HashPassword(u.PasswordHash);
            u.CreatedAt = DateTime.UtcNow;
            u.UpdatedAt = DateTime.UtcNow;

            await _repo.CreateAsync(u);
            TempData["Success"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var u = await _repo.GetByIdAsync(id);
            return u is null ? NotFound() : View(u);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Usuario u, string? newPassword, [FromServices] Services.IAuthService auth)
        {
            if (id != u.Id) return NotFound();
            if (await _repo.ExistsByEmailAsync(u.Email, excludeId: id))
                ModelState.AddModelError(nameof(u.Email), "Ya existe otro usuario con ese email.");

            if (!ModelState.IsValid) return View(u);

            if (!string.IsNullOrWhiteSpace(newPassword))
                u.PasswordHash = auth.HashPassword(newPassword);

            u.UpdatedAt = DateTime.UtcNow;

            var ok = await _repo.UpdateAsync(u);
            if (!ok) return NotFound();

            // Actualizar claims si es el usuario actual
            if (User.Identity?.Name == id.ToString())
            {
                await UpdateUserClaimsAsync(u, auth);
            }

            TempData["Success"] = "Usuario actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var u = await _repo.GetByIdAsync(id);
            return u is null ? NotFound() : View(u);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _repo.DeleteAsync(id);
            TempData["Success"] = "Usuario eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Usuario/ChangePassword  
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(long userId, string currentPassword, string newPassword, string confirmPassword, [FromServices] Services.IAuthService auth)
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(currentPassword))
                {
                    TempData["Error"] = "La contraseña actual es obligatoria.";
                    return RedirectToAction("Details", new { id = userId });
                }

                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                {
                    TempData["Error"] = "La nueva contraseña debe tener al menos 6 caracteres.";
                    return RedirectToAction("Details", new { id = userId });
                }

                if (newPassword != confirmPassword)
                {
                    TempData["Error"] = "Las contraseñas no coinciden.";
                    return RedirectToAction("Details", new { id = userId });
                }

                // Obtener usuario actual
                var usuario = await _repo.GetByIdAsync(userId);
                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado.";
                    return RedirectToAction("Details", new { id = userId });
                }

                // Verificar contraseña actual
                if (!auth.VerifyPassword(currentPassword, usuario.PasswordHash))
                {
                    TempData["Error"] = "La contraseña actual es incorrecta.";
                    return RedirectToAction("Details", new { id = userId });
                }

                // Actualizar contraseña
                usuario.PasswordHash = auth.HashPassword(newPassword);
                usuario.UpdatedAt = DateTime.UtcNow;

                var success = await _repo.UpdateAsync(usuario);
                if (success)
                {
                    TempData["Success"] = "Contraseña cambiada exitosamente.";
                }
                else
                {
                    TempData["Error"] = "Error al cambiar la contraseña.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cambiar contraseña: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = userId });
        }

        // POST: Usuario/ChangeAvatar
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeAvatar(long userId, IFormFile? avatarFile, [FromServices] Services.IAuthService auth)
        {
            try
            {
                var usuario = await _repo.GetByIdAsync(userId);
                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado.";
                    return RedirectToAction("Details", new { id = userId });
                }


                if (usuario.AvatarUrl != null)
                {
                    // Eliminar avatar anterior del servidor
                    var oldFilePath = Path.Combine(_env.WebRootPath, usuario.AvatarUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                }

                string? newAvatarUrl = null;

                if (avatarFile != null && avatarFile.Length > 0)
                {
                    if (avatarFile.Length > 10 * 1024 * 1024) // 10MB máximo
                    {
                        TempData["Error"] = "El archivo no puede ser mayor a 10MB.";
                        return RedirectToAction("Details", new { id = userId });
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Error"] = "Solo se permiten archivos JPG, PNG y GIF.";
                        return RedirectToAction("Details", new { id = userId });
                    }


                    // Crear directorio si no existe
                    var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    // Generar nombre único
                    var fileName = $"avatar_{userId}_{DateTime.UtcNow.Ticks}{extension}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    // Guardar archivo
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }

                    newAvatarUrl = $"/uploads/avatars/{fileName}";
                    
                    usuario.AvatarUrl = newAvatarUrl;
                    usuario.UpdatedAt = DateTime.UtcNow;

                    var success = await _repo.UpdateAsync(usuario);
                    if (success)
                    {
                        // Actualizar claims si es el usuario actual
                        if (User.Identity?.Name == userId.ToString())
                        {
                            await UpdateUserClaimsAsync(usuario, auth);
                        }
                        
                        TempData["Success"] = "Avatar actualizado exitosamente.";
                    }
                    else
                    {
                        TempData["Error"] = "Error al actualizar el avatar.";
                    }
                }
                else
                {
                    usuario.AvatarUrl = null;
                    usuario.UpdatedAt = DateTime.UtcNow;
                    var success = await _repo.UpdateAsync(usuario);
                    if (success)
                    {
                        // Actualizar claims si es el usuario actual
                        if (User.Identity?.Name == userId.ToString())
                        {
                            await UpdateUserClaimsAsync(usuario, auth);
                        }
                        
                        TempData["Success"] = "Avatar eliminado exitosamente.";
                    }
                    else
                    {
                        TempData["Error"] = "Error al eliminar el avatar.";
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cambiar avatar: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = userId });
        }

        // GET: Usuario/Profile (Ruta alternativa para el perfil actual)
        public IActionResult Profile()
        {
            // Redirigir al Details sin ID para mostrar perfil del usuario actual
            return RedirectToAction("Details");
        }
        
        private async Task UpdateUserClaimsAsync(Usuario usuario, Services.IAuthService auth)
        {
            // Crear nuevo principal con las claims actualizadas
            var newPrincipal = auth.CreatePrincipal(usuario);
            
            // Renovar la autenticación con las nuevas claims
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                newPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });
        }
    }
}
