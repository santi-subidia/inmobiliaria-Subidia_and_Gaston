using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class PersonasController : Controller
    {
        private readonly IPersonaRepository _repo;
        public PersonasController(IPersonaRepository repo) => _repo = repo;

        // LISTA
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var personas = await _repo.GetAllAsync(ct);
            return View(personas);
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
        public async Task<IActionResult> Create(Persona p, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(p);
            await _repo.CreateAsync(p, ct);
            TempData["Success"] = "Persona creada";
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
        public async Task<IActionResult> Edit(int id, Persona p, CancellationToken ct)
        {
            if (id != p.Id) return BadRequest();
            if (!ModelState.IsValid) return View(p);

            var ok = await _repo.UpdateAsync(p, ct);
            if (!ok) return NotFound();

            TempData["Success"] = "Persona actualizada";
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
