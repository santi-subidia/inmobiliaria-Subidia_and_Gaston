using Inmobiliaria.Data;
using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class PropietarioController : Controller
    {
        private readonly IPropietarioRepository _repo;

        public PropietarioController(IPropietarioRepository repo)
        {
            _repo = repo;
        }

        // GET: Propietario
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                const int pageSize = 10;
                var (items, total) = await _repo.GetPagedAsync(page, pageSize);

                // Crear el modelo paginado
                var model = new PagedResult<Propietario>
                {
                    Items = items,
                    TotalCount = total,
                    PageSize = pageSize,
                    CurrentPage = page
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar propietarios: {ex.Message}";
                return View(new PagedResult<Propietario> { Items = new List<Propietario>(), TotalCount = 0, CurrentPage = 1, PageSize = 10 });
            }
        }

        // GET: Propietario/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var propietario = await _repo.GetByIdAsync(id);
            if (propietario == null) return NotFound();
            return View(propietario);
        }

        // GET: Propietario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateAsync(propietario);
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        // GET: Propietario/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var propietario = await _repo.GetByIdAsync(id);
            if (propietario == null) return NotFound();
            return View(propietario);
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Propietario propietario)
        {
            if (id != propietario.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var ok = await _repo.UpdateAsync(propietario);
                if (!ok) return NotFound();
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        // GET: Propietario/Delete/5
        [Authorize(Policy = "Administrador")]
        public async Task<IActionResult> Delete(long id)
        {
            var propietario = await _repo.GetByIdAsync(id);
            if (propietario == null) return NotFound();
            return View(propietario);
        }

        // POST: Propietario/Delete/5 (soft delete con FechaEliminacion)
        [Authorize(Policy = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
