using Inmobiliaria.Data;
using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly IInquilinoRepository _repo;
        private readonly IPropietarioRepository _propietarioRepo;

        public InquilinoController(IInquilinoRepository repo, IPropietarioRepository propietarioRepo)
        {
            _repo = repo;
            _propietarioRepo = propietarioRepo;
        }

        // GET: Inquilino
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var (items, total) = await _repo.GetPagedAsync(page, pageSize);

            var model = new PagedResult<Inquilino>
            {
                Items = items,
                TotalCount = total,
                PageSize = pageSize,
                CurrentPage = page
            };

            return View(model);
        }

        public async Task<IActionResult> SearchByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(Array.Empty<object>());
            }

            var results = await _repo.SearchByNameAsync(name);

            var lista = results.Select(i => new
            {
                i.Id,
                i.NombreCompleto
            });

            return Json(lista);
        }

        // GET: Inquilino/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var inquilino = await _repo.GetByIdAsync(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        // GET: Inquilino/ExisteDni?dni=12345678
        [HttpGet]
        public async Task<IActionResult> ExisteDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) return Json(null);
            var inquilino = await _repo.GetByDniAsync(dni);
            if (inquilino != null)
            {
                if (inquilino.FechaEliminacion == null)
                    return Json(new { mensaje = "El inquilino ya existe." });
                else
                {
                    if (await _repo.UpdateFechaEliminacionAsync(inquilino.Id))
                    {
                        inquilino.FechaEliminacion = null;
                        return Json(new { mensaje = "El inquilino existía pero fue reactivado." });
                    }
                }
            }
            var propietario = await _propietarioRepo.GetByDniAsync(dni);
            if (propietario != null)
            {
                var inquilinoFromPropietario = Inquilino.InquilinoFromPropietario(propietario);
                var id = await _repo.CreateAsync(inquilinoFromPropietario);
                return Json(new { mensaje = "El inquilino fue creado a partir del propietario." });
            }
            return Json(null);
        }

        // GET: Inquilino/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateAsync(inquilino);
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        // GET: Inquilino/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var inquilino = await _repo.GetByIdAsync(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        // POST: Inquilino/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Inquilino inquilino)
        {
            if (id != inquilino.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existente = await _repo.GetByDniAsync(inquilino.Dni!);
                if (existente != null && existente.Id != id)
                {
                    if (existente.FechaEliminacion == null)
                    {
                        ModelState.AddModelError(nameof(Inquilino.Dni), "Ya existe un inquilino con ese DNI.");
                        return View(inquilino);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(Inquilino.Dni), "Ya existe un inquilino con ese DNI que fue eliminado. Intente crear el inquilino nuevamente para reactivarlo.");
                        return View(inquilino);
                    }
                }

                var existePropietario = await _propietarioRepo.GetByDniAsync(inquilino.Dni!);
                if (existePropietario != null)
                {
                    ModelState.AddModelError(nameof(Inquilino.Dni), "Ya existe un propietario con ese DNI. Intente crear el inquilino para copiar los datos del propietario.");
                    return View(inquilino);
                }
                
                var ok = await _repo.UpdateAsync(inquilino);
                if (!ok) return NotFound();
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        // GET: Inquilino/Delete/5
        [Authorize(Policy = "Administrador")]
        public async Task<IActionResult> Delete(long id)
        {
            var inquilino = await _repo.GetByIdAsync(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        // POST: Inquilino/Delete/5 (soft delete con FechaEliminacion)
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