// Controllers/UsuarioController.cs
using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    [Authorize] // requiere estar logueado por cookie
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _repo;
        public UsuarioController(IUsuarioRepository repo) => _repo = repo;

        // Listado simple (sin paginado)
        public async Task<IActionResult> Index()
        {
            var users = await _repo.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(long id)
        {
            var u = await _repo.GetByIdAsync(id);
            return u is null ? NotFound() : View(u);
        }

        [HttpGet]
        public IActionResult Create() => View(new Usuario { IsActive = true });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario u, string password, [FromServices] Services.IAuthService auth)
        {
            if (await _repo.ExistsByEmailAsync(u.Email))
                ModelState.AddModelError(nameof(u.Email), "Ya existe un usuario con ese email.");
            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError(nameof(u.PasswordHash), "La contraseña es obligatoria.");

            if (!ModelState.IsValid) return View(u);

            u.PasswordHash = auth.HashPassword(password);
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
                u.PasswordHash = auth.HashPassword(newPassword); // cambiar password

            u.UpdatedAt = DateTime.UtcNow;

            var ok = await _repo.UpdateAsync(u);
            if (!ok) return NotFound();

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
    }
}
