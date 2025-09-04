using Inmobiliaria.Data;
using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            var inquilinos = await _repo.GetAllAsync();
            return View(inquilinos);
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
                var ok = await _repo.UpdateAsync(inquilino);
                if (!ok) return NotFound();
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        // GET: Inquilino/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var inquilino = await _repo.GetByIdAsync(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        // POST: Inquilino/Delete/5 (soft delete con FechaEliminacion)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}