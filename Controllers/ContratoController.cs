using Inmobiliaria.Data;
using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? estado = null,
            long? propietarioId = null, long? inmuebleId = null, DateOnly? fechaDesde = null,
            DateOnly? fechaHasta = null, int? proximosVencer = null)
        {
            try
            {
                // Si se solicita filtro de próximos a vencer
                if (proximosVencer.HasValue)
                {
                    var proximosContratos = await _repo.GetProximosVencerAsync(proximosVencer.Value);

                    // Cargar datos relacionados
                    foreach (var contrato in proximosContratos)
                    {
                        contrato.Inquilino = await _inquilinoRepo.GetByIdAsync(contrato.InquilinoId);
                        contrato.Inmueble = await _inmuebleRepo.GetByIdAsync(contrato.InmuebleId);
                        if (contrato.Inmueble != null)
                            contrato.Inmueble.Propietario = await _propietarioRepo.GetByIdAsync(contrato.Inmueble.PropietarioId);
                    }

                    // Convertir a PagedResult
                    var totalProximos = proximosContratos.Count();
                    var itemsProximos = proximosContratos.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    var pagedProximos = new PagedResult<Contrato>
                    {
                        Items = itemsProximos,
                        TotalCount = totalProximos,
                        CurrentPage = page,
                        PageSize = pageSize
                    };

                    ViewBag.ProximosVencer = proximosVencer;
                    await CargarListasParaFiltros();
                    return View(pagedProximos);
                }

                // Filtros normales con paginación
                var (items, total) = await _repo.GetPagedWithFiltersAsync(page, pageSize, estado, propietarioId, inmuebleId, fechaDesde, fechaHasta);

                // Cargar datos relacionados para cada contrato
                foreach (var contrato in items)
                {
                    contrato.Inquilino = await _inquilinoRepo.GetByIdAsync(contrato.InquilinoId);
                    contrato.Inmueble = await _inmuebleRepo.GetByIdAsync(contrato.InmuebleId);
                    if (contrato.Inmueble != null)
                        contrato.Inmueble.Propietario = await _propietarioRepo.GetByIdAsync(contrato.Inmueble.PropietarioId);
                }

                // Crear el modelo paginado
                var model = new PagedResult<Contrato>
                {
                    Items = items,
                    TotalCount = total,
                    PageSize = pageSize,
                    CurrentPage = page
                };

                // Mantener filtros en ViewBag
                ViewBag.FiltroEstado = estado;
                ViewBag.FiltroPropietario = propietarioId;
                ViewBag.FiltroInmueble = inmuebleId;
                ViewBag.FechaDesde = fechaDesde;
                ViewBag.FechaHasta = fechaHasta;

                await CargarListasParaFiltros();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar contratos: {ex.Message}";
                await CargarListasParaFiltros();
                return View(new PagedResult<Contrato> { Items = new List<Contrato>(), TotalCount = 0, CurrentPage = 1, PageSize = pageSize });
            }
        }

        private async Task CargarListasParaFiltros()
        {
            try
            {
                // Cargar listas para los filtros
                var propietarios = await _propietarioRepo.GetAllAsync();
                var inmuebles = await _inmuebleRepo.GetAllAsync();

                ViewBag.PropietariosLista = propietarios.OrderBy(p => p.Nombre).ToList();
                ViewBag.InmueblesLista = inmuebles.OrderBy(i => i.Direccion).ToList();
            }
            catch
            {
                // Si hay error cargando las listas, usar listas vacías
                ViewBag.PropietariosLista = new List<Propietario>();
                ViewBag.InmueblesLista = new List<Inmueble>();
            }
        }

        // GET: Contrato/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var contrato = await _repo.GetByIdAsync(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // GET: Contrato/Create
        public IActionResult Create()
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
                var ok = await _repo.UpdateAsync(contrato);
                if (!ok) return NotFound();
                return RedirectToAction(nameof(Index));
            }

            PrepareViewBagsAsync();
            return View(contrato);
        }

        // GET: Contrato/Delete/5
        [Authorize(Policy="Administrador")]
        public async Task<IActionResult> Delete(long id)
        {
            var contrato = await _repo.GetByIdAsync(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // POST: Contrato/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id, long idUsuario)
        {
            await _repo.DeleteAsync(id, idUsuario);
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

        // GET: Contrato/PorInquilino/5
        public async Task<IActionResult> PorInquilino(long inquilinoId)
        {
            var contratos = await _repo.GetByInquilinoIdAsync(inquilinoId);
            var inquilino = await _inquilinoRepo.GetByIdAsync(inquilinoId);

            if (inquilino == null) return NotFound();

            ViewData["Inquilino"] = $"{inquilino.Apellido}, {inquilino.Nombre}";
            return View("Index", contratos);
        }

        public async Task<IActionResult> VerificarInmueblesDisponibles(DateOnly fechaInicio, DateOnly fechaFin, long? contratoId = null)
        {
            if (fechaInicio == default || fechaFin == default) return Json(null);

            var inmueblesDisponibles = await _inmuebleRepo.GetAllbyFechasAsync(fechaInicio, fechaFin, contratoId);

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


        }

        // GET: Contrato/VigentesPorFecha
        public async Task<IActionResult> VigentesPorFecha(DateOnly? fechaDesde = null, DateOnly? fechaHasta = null)
        {
            if (!fechaDesde.HasValue || !fechaHasta.HasValue)
            {
                // Valores por defecto: último mes
                fechaHasta = DateOnly.FromDateTime(DateTime.Now);
                fechaDesde = fechaHasta.Value.AddDays(-30);
            }

            var contratos = await _repo.GetVigentesEnRangoAsync(fechaDesde.Value, fechaHasta.Value);

            foreach (var contrato in contratos)
            {
                contrato.Inquilino = await _inquilinoRepo.GetByIdAsync(contrato.InquilinoId);
                contrato.Inmueble = await _inmuebleRepo.GetByIdAsync(contrato.InmuebleId);
                if (contrato.Inmueble != null)
                    contrato.Inmueble.Propietario = await _propietarioRepo.GetByIdAsync(contrato.Inmueble.PropietarioId);
            }

            ViewBag.FechaDesde = fechaDesde;
            ViewBag.FechaHasta = fechaHasta;
            ViewData["Title"] = $"Contratos Vigentes ({fechaDesde:dd/MM/yyyy} - {fechaHasta:dd/MM/yyyy})";

            return View("Index", contratos);
        }

        // GET: Contrato/PorInmueble/5
        public async Task<IActionResult> PorInmueble(long inmuebleId)
        {
            var contratos = await _repo.GetByInmuebleIdAsync(inmuebleId);
            var inmueble = await _inmuebleRepo.GetByIdAsync(inmuebleId);

            if (inmueble == null) return NotFound();

            foreach (var contrato in contratos)
            {
                contrato.Inquilino = await _inquilinoRepo.GetByIdAsync(contrato.InquilinoId);
                contrato.Inmueble = inmueble;
                contrato.Inmueble.Propietario = await _propietarioRepo.GetByIdAsync(inmueble.PropietarioId);
            }

            ViewData["Title"] = $"Contratos del Inmueble: {inmueble.Direccion}";
            ViewData["Inmueble"] = inmueble.Direccion;
            return View("Index", contratos);
        }

        // GET: Contrato/ProximosVencerFlex
        public async Task<IActionResult> ProximosVencerFlex(int dias = 30)
        {
            var contratos = await _repo.GetProximosAVencerAsync(dias);

            foreach (var contrato in contratos)
            {
                contrato.Inquilino = await _inquilinoRepo.GetByIdAsync(contrato.InquilinoId);
                contrato.Inmueble = await _inmuebleRepo.GetByIdAsync(contrato.InmuebleId);
                if (contrato.Inmueble != null)
                    contrato.Inmueble.Propietario = await _propietarioRepo.GetByIdAsync(contrato.Inmueble.PropietarioId);
            }

            ViewBag.DiasSeleccionados = dias;
            ViewData["Title"] = $"Contratos que Vencen en {dias} días";
            return View("ProximosVencer", contratos);
        }

        // GET: Contrato/Renovar/5
        public async Task<IActionResult> Renovar(long id)
        {
            var contratoAnterior = await _repo.GetByIdAsync(id);
            if (contratoAnterior == null)
            {
                TempData["Error"] = "Contrato no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar que el contrato esté finalizado o rescindido
            if (contratoAnterior.Estado() == "VIGENTE")
            {
                TempData["Error"] = "No se puede renovar un contrato vigente. Debe finalizar o rescindir primero.";
                return RedirectToAction(nameof(Index));
            }

            // Cargar datos del inmueble e inquilino
            contratoAnterior.Inmueble = await _inmuebleRepo.GetByIdAsync(contratoAnterior.InmuebleId);
            contratoAnterior.Inquilino = await _inquilinoRepo.GetByIdAsync(contratoAnterior.InquilinoId);

            if (contratoAnterior.Inmueble?.PropietarioId != null)
            {
                contratoAnterior.Inmueble.Propietario = await _propietarioRepo.GetByIdAsync(contratoAnterior.Inmueble.PropietarioId);
            }

            // Obtener contratos existentes para mostrar ocupación
            var contratosExistentes = await _repo.GetByInmuebleIdAsync(contratoAnterior.InmuebleId);
            var contratosVigentes = contratosExistentes.Where(c => c.Estado() == "VIGENTE").ToList();

            // Crear el nuevo contrato con datos predeterminados
            var nuevoContrato = new Contrato
            {
                InmuebleId = contratoAnterior.InmuebleId,
                InquilinoId = contratoAnterior.InquilinoId,
                MontoMensual = contratoAnterior.MontoMensual * 1.1m, // Aumento del 10% por defecto
                FechaInicio = contratoAnterior.FechaFinEfectiva?.AddDays(1) ?? contratoAnterior.FechaFinOriginal.AddDays(1),
                FechaFinOriginal = (contratoAnterior.FechaFinEfectiva?.AddDays(1) ?? contratoAnterior.FechaFinOriginal.AddDays(1)).AddYears(1),
                Inmueble = contratoAnterior.Inmueble,
                Inquilino = contratoAnterior.Inquilino
            };

            ViewBag.ContratoAnterior = contratoAnterior;
            ViewBag.ContratosVigentes = contratosVigentes;
            ViewBag.MontoAnterior = contratoAnterior.MontoMensual;
            ViewBag.PorcentajeAumento = 10; // Por defecto 10%

            return View(nuevoContrato);
        }

        // POST: Contrato/Renovar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renovar(Contrato nuevoContrato, long contratoAnteriorId)
        {
            try
            {
                // Verificar que no haya conflictos de fechas con otros contratos
                var inmuebleDisponible = await VerificarDisponibilidadInmueble(nuevoContrato.InmuebleId,
                    nuevoContrato.FechaInicio, nuevoContrato.FechaFinOriginal, null);

                if (!inmuebleDisponible)
                {
                    TempData["Error"] = "El inmueble no está disponible en las fechas seleccionadas.";
                    return await Renovar(contratoAnteriorId);
                }

                // Establecer datos adicionales
                nuevoContrato.CreadoPor = 1; // TODO: Usar usuario actual
                nuevoContrato.CreadoAt = DateTime.UtcNow;

                var id = await _repo.CreateAsync(nuevoContrato);

                TempData["Success"] = $"Contrato renovado exitosamente. Nuevo contrato ID: {id}";
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al renovar el contrato: {ex.Message}";
                return await Renovar(contratoAnteriorId);
            }
        }

        // Método auxiliar para verificar disponibilidad
        private async Task<bool> VerificarDisponibilidadInmueble(long inmuebleId, DateOnly fechaInicio, DateOnly fechaFin, long? excepto = null)
        {
            var contratos = await _repo.GetByInmuebleIdAsync(inmuebleId);

            foreach (var contrato in contratos)
            {
                if (excepto.HasValue && contrato.Id == excepto.Value)
                    continue;

                // Solo verificar contratos vigentes o que se superpongan en fechas
                if (contrato.Estado() == "VIGENTE")
                {
                    var finEfectiva = contrato.FechaFinEfectiva ?? contrato.FechaFinOriginal;

                    // Verificar superposición de fechas
                    if (!(fechaFin < contrato.FechaInicio || fechaInicio > finEfectiva))
                    {
                        return false; // Hay conflicto
                    }
                }
            }

            return true; // Sin conflictos
        }

        // GET: Verificar disponibilidad para renovación
        public async Task<IActionResult> VerificarDisponibilidadRenovacion(long inmuebleId, DateOnly fechaInicio, DateOnly fechaFin)
        {
            try
            {
                var disponible = await VerificarDisponibilidadInmueble(inmuebleId, fechaInicio, fechaFin);

                return Json(new
                {
                    disponible = disponible,
                    mensaje = disponible ? "Inmueble disponible para las fechas seleccionadas" : "El inmueble no está disponible en ese período"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    disponible = false,
                    mensaje = $"Error al verificar disponibilidad: {ex.Message}"
                });
            }
        }
    }
}