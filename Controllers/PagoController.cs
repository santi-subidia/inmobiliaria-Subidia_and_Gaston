using System.Reflection.Metadata.Ecma335;
using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IPagoRepository _repo;
        private readonly IContratoRepository _contratoRepo;

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
                Text = $"Contrato #{c.Id} - {(c.Inquilino?.NombreCompleto ?? "Sin inquilino")}"
            }).ToList();

            // Estadísticas generales
            var totalRecaudado = 0;
            foreach (var pago in items)
            {
                if (pago.Estado == "Pagado")
                {
                    totalRecaudado += (int)pago.Importe;
                }
            }

            var pagosEstesMes = await _repo.GetPagosDelMesAsync(DateTime.Now.Year, DateTime.Now.Month);
            var contratosActivosCount = contratos.Count(c => c.Estado() == "VIGENTE");

            ViewBag.TotalRecaudado = totalRecaudado;
            ViewBag.PagosEstesMes = pagosEstesMes;
            ViewBag.ContratosActivos = contratosActivosCount;

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
        public async Task<IActionResult> Existe(long contratoId, int numeroPago)
        {
            var existe = await _repo.ExistsByContratoAndNumeroAsync(contratoId, numeroPago);
            if (existe)
                return Json(new { mensaje = "Ya existe un pago con ese número para el contrato seleccionado." });
            return Json(null);
        }

        // GET: Pagos/Create?contratoId=1
        public async Task<IActionResult> Create(long? contratoId)
        {
            if (!contratoId.HasValue || contratoId <= 0)
            {
                TempData["Error"] = "Debe especificar un contrato válido.";
                return RedirectToAction("Index", "Contrato");
            }

            var contrato = await _contratoRepo.GetByIdAsync((long)contratoId);
            if (contrato == null)
            {
                TempData["Error"] = "Contrato no encontrado.";
                return RedirectToAction("Index", "Contrato");
            }

            // Usar el nuevo método optimizado para obtener estadísticas
            var (cantidadPagos, montoPagado) = await _repo.GetMontoPagadoAndCantidadPagosByContratoAsync(contratoId.Value);
            decimal saldoPendiente = contrato.MontoTotal + contrato.MontoMulta - montoPagado;
            var cantidadPagosTotales = await _repo.GetCantidadPagosByContratoAsync(contratoId.Value);

            var pago = new Pago
            {
                ContratoId = contratoId.Value,
                NumeroPago = cantidadPagosTotales + 1,
                FechaPago = DateOnly.FromDateTime(DateTime.UtcNow),
                Estado = "Pagado",
                Importe = Math.Min(contrato.MontoMensual, saldoPendiente)
            };

            // Información del contrato para la vista
            ViewBag.Contrato = contrato;
            ViewBag.MontoMensual = contrato.MontoMensual;
            ViewBag.MontoTotal = contrato.MontoTotal + contrato.MontoMulta;
            ViewBag.MontoPagado = montoPagado;
            ViewBag.SaldoPendiente = saldoPendiente;
            ViewBag.CantidadPagos = cantidadPagos;
            ViewBag.MontoMaximoPermitido = saldoPendiente;

            // Si ya está totalmente pagado, mostrar advertencia
            if (saldoPendiente <= 0)
            {
                TempData["Warning"] = "Este contrato ya está totalmente pagado.";
                return RedirectToAction("IndexByContrato", new { id = contratoId });
            }

            return View(pago);
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago)
        {
            // Validar que el contrato existe y obtener información actualizada
            var contrato = await _contratoRepo.GetByIdAsync(pago.ContratoId);
            if (contrato == null)
            {
                ModelState.AddModelError("", "Contrato no encontrado.");
                return View(pago);
            }

            // Calcular saldo pendiente usando el método optimizado
            var (cantidadPagos, montoPagado) = await _repo.GetMontoPagadoAndCantidadPagosByContratoAsync(pago.ContratoId);
            decimal saldoPendiente = contrato.MontoTotal + contrato.MontoMulta - montoPagado;

            // Validación del importe máximo permitido
            if (pago.Importe > saldoPendiente)
            {
                ModelState.AddModelError(nameof(pago.Importe),
                    $"El importe no puede ser superior al saldo pendiente: ${saldoPendiente:N2}");
            }

            // Validación de importe mínimo
            if (pago.Importe <= 0)
            {
                ModelState.AddModelError(nameof(pago.Importe),
                    "El importe debe ser mayor a cero.");
            }

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
                    pago.CreadoPorId = int.Parse(User.Identity.Name);
                    pago.CreadoAt = DateTime.UtcNow;
                    pago.Estado = "Pagado";

                    await _repo.CreateAsync(pago);
                    TempData["Success"] = $"Pago #{pago.NumeroPago} creado correctamente por ${pago.Importe:N2}.";

                    // Verificar si el contrato queda totalmente pagado
                    decimal nuevoSaldo = saldoPendiente - pago.Importe;
                    if (nuevoSaldo <= 0)
                    {
                        TempData["Info"] = "¡El contrato ha sido totalmente pagado!";
                    }

                    return RedirectToAction("IndexByContrato", new { id = pago.ContratoId });
                }
            }

            // Si hay errores, recargar información del contrato
            ViewBag.Contrato = contrato;
            ViewBag.MontoMensual = contrato.MontoMensual;
            ViewBag.MontoTotal = contrato.MontoTotal + contrato.MontoMulta;
            ViewBag.MontoPagado = montoPagado;
            ViewBag.SaldoPendiente = saldoPendiente;
            ViewBag.CantidadPagos = cantidadPagos;
            ViewBag.MontoMaximoPermitido = saldoPendiente;

            return View(pago);
        }

        // GET: Pagos/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var pago = await _repo.GetByIdAsync(id);
            if (pago == null) return NotFound();

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

                    TempData["Success"] = $"Concepto del pago #{pago.NumeroPago} actualizado correctamente.";
                    return RedirectToAction("IndexByContrato", new { id = pago.ContratoId });
                }
            }

            // Si hay errores, simplemente retornar la vista sin cargar contratos
            return View(pago);
        }

        // GET: Pago/IndexByContrato/5
        public async Task<IActionResult> IndexByContrato(long id)
        {
            // Obtener el contrato
            var contrato = await _contratoRepo.GetByIdAsync(id);
            if (contrato == null)
            {
                TempData["Error"] = "Contrato no encontrado.";
                return RedirectToAction("Index", "Contrato");
            }

            var pagos = await _repo.GetByContratoIdAsync(id);
            var (cantidadPagos, totalPagado) = await _repo.GetMontoPagadoAndCantidadPagosByContratoAsync(id);

            ViewBag.Contrato = contrato;
            ViewBag.ContratoId = id;
            ViewBag.TotalPagado = totalPagado;

            return View(pagos);
        }

        public async Task<IActionResult> Delete(long id)
        {
            var pago = await _repo.GetByIdAsync(id);
            if (pago == null) return NotFound();

            return View(pago);
        }

        // POST: Pago/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int? ContratoId)
        {
            try
            {
                var pago = await _repo.GetByIdAsync(id);
                if (pago == null)
                {
                    TempData["Error"] = "Pago no encontrado.";
                    if (ContratoId.HasValue)
                    {
                        return RedirectToAction("IndexByContrato", new { id = ContratoId });

                    }
                    return RedirectToAction("Index");
                }

                if (pago.Estado == "Anulado")
                {
                    TempData["Warning"] = "El pago ya está anulado.";
                    if (ContratoId.HasValue)
                    {
                        return RedirectToAction("IndexByContrato", new { id = ContratoId });

                    }
                    return RedirectToAction("Index");
                }

                if (pago.Estado == "Anulado")
                {
                    TempData["Warning"] = "El pago ya está anulado.";
                    if (ContratoId.HasValue)
                    {
                        return RedirectToAction("IndexByContrato", new { id = ContratoId });
                    }
                    return RedirectToAction("Index");
                }

                var idUsuario = int.Parse(User.Identity.Name);
                var ok = await _repo.AnularAsync(id, idUsuario);
                if (ok)
                {
                    TempData["Success"] = "Pago anulado correctamente.";
                }
                else
                {
                    TempData["Error"] = "Error al anular el pago.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al anular el pago: {ex.Message}";
            }

            if (ContratoId.HasValue)
            {
                return RedirectToAction("IndexByContrato", new { id = ContratoId });
            }
            return RedirectToAction("Index");
        }
    }
}
