using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly IInquilinoRepository _repo;
        public InquilinoController(IInquilinoRepository repo) => _repo = repo;

        // LISTA
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var inquilinos = await _repo.GetAllAsync(ct);
            return View(inquilinos);
        }

        // DETALLES
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var i = await _repo.GetByIdAsync(id, ct);
            if (i is null) return NotFound();
            return View(i);
        }

        // CREATE
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inquilino i, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(i);
            await _repo.CreateAsync(i, ct);
            TempData["Success"] = "Inquilino creado";
            return RedirectToAction(nameof(Index));
        }

        // EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var i = await _repo.GetByIdAsync(id, ct);
            if (i is null) return NotFound();
            return View(i);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Inquilino i, CancellationToken ct)
        {
            Console.WriteLine("Edit POST called");
            if (!ModelState.IsValid) return View(i);

            var ok = await _repo.UpdateAsync(i, ct);
            if (!ok) return NotFound();

            TempData["Success"] = "Inquilino actualizado";
            return RedirectToAction(nameof(Index));
        }

        // DELETE (confirmación + POST)
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var i = await _repo.GetByIdAsync(id, ct);
            if (i is null) return NotFound();
            return View(i);
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