using Inmobiliaria.Data;
using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria.Controllers
{
    public class ContratoController : Controller
    {
        private readonly IContratoRepository _repo;
        private readonly IInquilinoRepository _inquilinoRepo;
        private readonly IInmuebleRepository _inmuebleRepo;
        private readonly IPropietarioRepository _propietarioRepo;

        public ContratoController(IContratoRepository repo, IInquilinoRepository inquilinoRepo, IInmuebleRepository inmuebleRepo, IPropietarioRepository propietarioRepo)
        {
            _repo = repo;
            _inquilinoRepo = inquilinoRepo;
            _inmuebleRepo = inmuebleRepo;
            _propietarioRepo = propietarioRepo;
        }

        // GET: Contrato
        public async Task<IActionResult> Index()
        {
            var contratos = await _repo.GetAllAsync();
            foreach (var contrato in contratos)
            {
                contrato.Inquilino = await _inquilinoRepo.GetByIdAsync(contrato.InquilinoId);
                contrato.Inmueble = await _inmuebleRepo.GetByIdAsync(contrato.InmuebleId);
                if (contrato.Inmueble != null)
                    contrato.Inmueble.Propietario = await _propietarioRepo.GetByIdAsync(contrato.Inmueble.PropietarioId);
            }
            return View(contratos);
        }

        // GET: Contrato/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var contrato = await _repo.GetByIdAsync(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // GET: Contrato/Create
        public async Task<IActionResult> Create()
        {
            PrepareViewBagsAsync();
            return View();
        }

        // POST: Contrato/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateAsync(contrato);
                return RedirectToAction(nameof(Index));
            }

            PrepareViewBagsAsync();
            return View(contrato);
        }

        // GET: Contrato/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var contrato = await _repo.GetByIdAsync(id);
            if (contrato == null) return NotFound();

            contrato.Inquilino = await _inquilinoRepo.GetByIdAsync(contrato.InquilinoId);
            contrato.Inmueble = await _inmuebleRepo.GetByIdAsync(contrato.InmuebleId);

            PrepareViewBagsAsync();
            return View(contrato);
        }

        // POST: Contrato/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Contrato contrato)
        {
            if (id != contrato.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Verificar fechas lógicas
                if (contrato.FechaInicio >= contrato.FechaFinOriginal)
                {
                    ModelState.AddModelError("FechaFinOriginal", "La fecha de fin debe ser posterior a la fecha de inicio.");
                    PrepareViewBagsAsync();
                    return View(contrato);
                }

                var ok = await _repo.UpdateAsync(contrato);
                if (!ok) return NotFound();
                return RedirectToAction(nameof(Index));
            }

            PrepareViewBagsAsync();
            return View(contrato);
        }

        // GET: Contrato/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var contrato = await _repo.GetByIdAsync(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // POST: Contrato/Delete/5 (Rescisión)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            // En lugar de eliminar, rescindimos el contrato
            await _repo.RescindirContratoAsync(id, 1); // Usuario temporalmente hardcodeado
            return RedirectToAction(nameof(Index));
        }

        // GET: Contrato/Finalizar/5
        public async Task<IActionResult> Finalizar(long id)
        {
            var contrato = await _repo.GetByIdAsync(id);
            if (contrato == null) return NotFound();

            if (!contrato.PuedeFinalizarse)
            {
                TempData["Error"] = "Este contrato no puede ser finalizado.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(contrato);
        }

        // POST: Contrato/Finalizar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizarConfirmed(long id, DateTime? fechaFinEfectiva)
        {
            var ok = await _repo.FinalizarContratoAsync(id, 1, fechaFinEfectiva); // Usuario temporalmente hardcodeado
            if (!ok)
            {
                TempData["Error"] = "No se pudo finalizar el contrato.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Success"] = "Contrato finalizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Contrato/Rescindir/5
        public async Task<IActionResult> Rescindir(long id)
        {
            var contrato = await _repo.GetByIdAsync(id);
            if (contrato == null) return NotFound();

            if (!contrato.PuedeRescindirse)
            {
                TempData["Error"] = "Este contrato no puede ser rescindido.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(contrato);
        }

        // POST: Contrato/Rescindir/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RescindirConfirmed(long id, DateTime? fechaFinEfectiva)
        {
            var ok = await _repo.RescindirContratoAsync(id, 1, fechaFinEfectiva); // Usuario temporalmente hardcodeado
            if (!ok)
            {
                TempData["Error"] = "No se pudo rescindir el contrato.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Success"] = "Contrato rescindido exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Contrato/Vigentes
        public async Task<IActionResult> Vigentes()
        {
            var contratos = await _repo.GetVigentesAsync();
            return View("Index", contratos);
        }

        // GET: Contrato/ProximosVencer
        public async Task<IActionResult> ProximosVencer(int dias = 30)
        {
            var contratos = await _repo.GetProximosAVencerAsync(dias);
            ViewData["Dias"] = dias;
            return View(contratos);
        }

        // GET: Contrato/PorInquilino/5
        public async Task<IActionResult> PorInquilino(long inquilinoId)
        {
            var contratos = await _repo.GetByInquilinoIdAsync(inquilinoId);
            var inquilino = await _inquilinoRepo.GetByIdAsync(inquilinoId);

            if (inquilino == null) return NotFound();

            ViewData["Inquilino"] = $"{inquilino.Apellido}, {inquilino.Nombre}";
            return View("Index", contratos);
        }

        // API para verificar disponibilidad de inmueble
        [HttpGet]
        public async Task<IActionResult> VerificarDisponibilidadInmueble(long inmuebleId)
        {
            if (inmuebleId <= 0) return Json(null);

            var existe = await _repo.ExisteContratoVigenteParaInmuebleAsync(inmuebleId);
            if (existe)
            {
                return Json(new { mensaje = "Este inmueble ya tiene un contrato vigente." });
            }

            return Json(null);
        }

        public async Task<IActionResult> VerificarInmueblesDisponibles(DateOnly fechaInicio, DateOnly fechaFin)
        {
            if (fechaInicio == default || fechaFin == default) return Json(null);

            var inmueblesDisponibles = await _inmuebleRepo.GetAllbyFechasAsync(fechaInicio, fechaFin);

            return Json(new { inmuebles = inmueblesDisponibles });
        }

        private void PrepareViewBagsAsync()
        {
            // var inquilinos = await _inquilinoRepo.GetAllAsync();
            // if (inquilino != null)
            // {
            //     inquilinos.ToList().Add(inquilino);
            // }
            // ViewBag.InquilinoId = new SelectList(inquilinos, "Id", "NombreCompleto");

            // var inmuebles = (await _inmuebleRepo.GetAllWithFiltersAsync(disponible: true)).ToList();
            // if (inmueble != null)
            // {
            //     inmuebles.Add(inmueble);
            // }
            // ViewBag.InmuebleId = new SelectList(inmuebles, "Id", "Direccion");

            ViewBag.Estado = new SelectList(Enum.GetValues<EstadoContrato>());
        }
    }
}