using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly IPropietarioRepository _repo;
        public PropietarioController(IPropietarioRepository repo) => _repo = repo;

        // LISTA
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var propietarios = await _repo.GetAllAsync(ct);
            return View(propietarios);
        }

        // DETALLES
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var p = await _repo.GetByIdAsync(id, ct);
            if (p is null) return NotFound();
            return View(p);
        }

        // CREATE
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Propietario p, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(p);
            await _repo.CreateAsync(p, ct);
            TempData["Success"] = "Propietario creado";
            return RedirectToAction(nameof(Index));
        }

        // EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var p = await _repo.GetByIdAsync(id, ct);
            if (p is null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Propietario p, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(p);

            var ok = await _repo.UpdateAsync(p, ct);
            if (!ok) return NotFound();

            TempData["Success"] = "Propietario actualizado";
            return RedirectToAction(nameof(Index));
        }

        // DELETE (confirmación + POST)
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var p = await _repo.GetByIdAsync(id, ct);
            if (p is null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var ok = await _repo.DeleteAsync(id, ct);
            if (!ok) return NotFound();

            TempData["Success"] = "Persona eliminada";
            return RedirectToAction(nameof(Index));
        }
    }
}