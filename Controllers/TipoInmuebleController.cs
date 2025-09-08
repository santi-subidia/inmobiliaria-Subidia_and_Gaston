using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly ITipoInmuebleRepository _repo;
        public TipoInmuebleController(ITipoInmuebleRepository repo) => _repo = repo;

        // GET: TipoInmueble
        public async Task<IActionResult> Index()
        {
            var tipos = await _repo.GetAllAsync();
            return View(tipos);
        }

        // GET: TipoInmueble/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var tipo = await _repo.GetByIdAsync(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        // GET: TipoInmueble/Create
        public IActionResult Create() => View();

        // POST: TipoInmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoInmueble tipo)
        {
            if (!ModelState.IsValid) return View(tipo);

            // Evitar duplicados por nombre (opcional)
            var existente = await _repo.GetByNombreAsync(tipo.Nombre!);
            if (existente != null && existente.FechaEliminacion == null)
            {
                ModelState.AddModelError(nameof(TipoInmueble.Nombre), "Ya existe un tipo con ese nombre.");
                return View(tipo);
            }
            if (existente != null && existente.FechaEliminacion != null)
            {
                await _repo.UpdateFechaEliminacionAsync(existente.Id);
                return RedirectToAction(nameof(Index));
            }

            await _repo.CreateAsync(tipo);
            return RedirectToAction(nameof(Index));
        }

        // GET: TipoInmueble/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var tipo = await _repo.GetByIdAsync(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        // POST: TipoInmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, TipoInmueble tipo)
        {
            if (id != tipo.Id) return NotFound();
            if (!ModelState.IsValid) return View(tipo);

            var ok = await _repo.UpdateAsync(tipo);
            if (!ok) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        // GET: TipoInmueble/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var tipo = await _repo.GetByIdAsync(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        // POST: TipoInmueble/Delete/5 (soft delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
