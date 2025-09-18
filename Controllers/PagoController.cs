using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria.Controllers
{
    public class PagoController : Controller
    {
        private readonly IPagoRepository _repo;
        private readonly IContratoRepository _contratoRepo;

        // Simulación de usuario actual para auditoría (remplazar por tu user real)
        private string UsuarioActual => User?.Identity?.Name ?? "sistema";

        public PagoController(IPagoRepository repo, IContratoRepository contratoRepo)
        {
            _repo = repo;
            _contratoRepo = contratoRepo;
        }

        // GET: Pagos
        // Opcional: filtro por contratoId para ver solo los pagos de un contrato
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, int? contratoId = null)
        {
            var (items, total) = await _repo.GetPagedAsync(page, pageSize, contratoId);

            // cargar contratos para filtro
            var contratos = await _contratoRepo.GetAllAsync();
            ViewBag.ContratosIndex = contratos.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"Contrato #{c.Id}"
            }).ToList();

            var model = new PagedResult<Pago>
            {
                Items = items,
                TotalCount = total,
                PageSize = pageSize,
                CurrentPage = page
            };
            ViewBag.ContratoId = contratoId;
            return View(model);
        }


        // GET: Pagos/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var pago = await _repo.GetByIdAsync(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        // GET: Pagos/Existe?contratoId=1&numeroPago=3
        // Útil para validación ajax en el form (evitar duplicado por contrato)
        [HttpGet]
        public async Task<IActionResult> Existe(int contratoId, int numeroPago)
        {
            var existe = await _repo.ExistsByContratoAndNumeroAsync(contratoId, numeroPago);
            if (existe)
                return Json(new { mensaje = "Ya existe un pago con ese número para el contrato seleccionado." });
            return Json(null);
        }

        // GET: Pagos/Create?contratoId=1
        public async Task<IActionResult> Create(int? contratoId)
        {
            await CargarContratosAsync(contratoId);
            var pago = new Pago
            {
                ContratoId = contratoId ?? 0,
                FechaPago = DateTime.Today,
                Estado = "Pendiente"
            };
            return View(pago);
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                // validación de duplicado Nº de pago por contrato
                var existe = await _repo.ExistsByContratoAndNumeroAsync(pago.ContratoId, pago.NumeroPago);
                if (existe)
                {
                    ModelState.AddModelError(nameof(pago.NumeroPago),
                        "Ya existe un pago con ese número para el contrato seleccionado.");
                }
                else
                {
                    pago.CreadoPor = UsuarioActual;
                    pago.CreadoAt = DateTime.UtcNow;

                    await _repo.CreateAsync(pago);
                    TempData["Msg"] = $"Pago #{pago.NumeroPago} creado para contrato #{pago.ContratoId}.";
                    return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
                }
            }

            await CargarContratosAsync(pago.ContratoId);
            return View(pago);
        }

        // GET: Pagos/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var pago = await _repo.GetByIdAsync(id);
            if (pago == null) return NotFound();

            await CargarContratosAsync(pago.ContratoId);
            return View(pago);
        }

        // POST: Pagos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Pago pago)
        {
            if (id != pago.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Chequear duplicado si cambió el número o el contrato
                var duplicado = await _repo.ExistsByContratoAndNumeroAsync(pago.ContratoId, pago.NumeroPago, excludeId: pago.Id);
                if (duplicado)
                {
                    ModelState.AddModelError(nameof(pago.NumeroPago),
                        "Ya existe un pago con ese número para el contrato seleccionado.");
                }
                else
                {
                    var ok = await _repo.UpdateAsync(pago);
                    if (!ok) return NotFound();

                    TempData["Msg"] = $"Pago #{pago.Id} actualizado.";
                    return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
                }
            }

            await CargarContratosAsync(pago.ContratoId);
            return View(pago);
        }

        // GET: Pagos/Delete/5
        // En pagos, Delete = Anular (soft delete)
        public async Task<IActionResult> Delete(long id)
        {
            var pago = await _repo.GetByIdAsync(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        // POST: Pagos/Delete/5  (Anulación)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var ok = await _repo.AnularAsync(id, UsuarioActual);
            if (!ok) return NotFound();

            TempData["Msg"] = $"Pago #{id} anulado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Utilidad: carga combo de contratos
        private async Task CargarContratosAsync(int? contratoSeleccionado = null)
        {
            var contratos = await _contratoRepo.GetAllAsync();
            ViewBag.Contratos = contratos
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"Contrato #{c.Id}",
                    Selected = contratoSeleccionado.HasValue && c.Id == contratoSeleccionado.Value
                })
                .ToList();
        }
    }
}
