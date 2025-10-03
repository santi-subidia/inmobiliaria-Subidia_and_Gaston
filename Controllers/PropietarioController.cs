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
        private readonly IInquilinoRepository _inquilinoRepo;

        public PropietarioController(IPropietarioRepository repo, IInquilinoRepository inquilinoRepo)
        {
            _repo = repo;
            _inquilinoRepo = inquilinoRepo;
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

        // GET: Propietario/ExisteDni?dni=12345678
        [HttpGet]
        public async Task<IActionResult> ExisteDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) return Json(null);
            var propietario = await _repo.GetByDniAsync(dni);
            if (propietario != null)
            {
                if (propietario.FechaEliminacion == null)
                    return Json(new { mensaje = "El propietario ya existe." });
                else
                {
                    if (await _repo.UpdateFechaEliminacionAsync(propietario.Id))
                    {
                        propietario.FechaEliminacion = null;
                        return Json(new { mensaje = "El propietario existía pero fue reactivado." });
                    }
                }
            }
            var inquilino = await _inquilinoRepo.GetByDniAsync(dni);
            if (inquilino != null)
            {
                var PropietarioFromInquilino = Propietario.PropietarioFromInquilino(inquilino);
                var id = await _repo.CreateAsync(PropietarioFromInquilino);
                return Json(new { mensaje = "El propietario fue creado a partir del inquilino." });
            }
            return Json(null);
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
                var existente = await _repo.GetByDniAsync(propietario.Dni!);
                if (existente != null && existente.Id != id)
                {
                    if (existente.FechaEliminacion == null)
                    {
                        ModelState.AddModelError(nameof(Propietario.Dni), "Ya existe un propietario con ese DNI.");
                        return View(propietario);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(Propietario.Dni), "Ya existe un propietario con ese DNI que fue eliminado. Intente crear el propietario nuevamente para reactivarlo.");
                        return View(propietario);
                    }
                }

                var existeInquilino = await _inquilinoRepo.GetByDniAsync(propietario.Dni!);
                if (existeInquilino != null)
                {
                    ModelState.AddModelError(nameof(Propietario.Dni), "Ya existe un inquilino con ese DNI. Intente crear el propietario nuevamente para copiar los datos del inquilino.");
                    return View(propietario);
                }
                
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
