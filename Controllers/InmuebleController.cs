using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IInmuebleRepository _repo;
        private readonly IPropietarioRepository _propietarioRepo;
        private readonly ITipoInmuebleRepository _tipoRepo; // asumido
        private readonly IImagenRepository _imagenRepo;
        private readonly IWebHostEnvironment _env;

        public InmuebleController(
            IInmuebleRepository repo,
            IPropietarioRepository propietarioRepo,
            ITipoInmuebleRepository tipoRepo, // si no lo tenés, lo quitamos y seteamos TipoId manual
            IImagenRepository imagenRepo,
            IWebHostEnvironment env)
        {
            _repo = repo;
            _propietarioRepo = propietarioRepo;
            _tipoRepo = tipoRepo;
            _imagenRepo = imagenRepo;
            _env = env;
        }

        // GET: Inmueble
        public async Task<IActionResult> Index()
        {
            var inmuebles = await _repo.GetAllWithFiltersAsync();

            // Propietarios: clave long para evitar mismatch
            var propietarios = await _propietarioRepo.GetAllAsync();
            var mapProp = propietarios.ToDictionary(
                p => (long)p.Id,                                 
                p => $"{p.Apellido}, {p.Nombre}");

            // Tipos: también long por coherencia (aunque funcione con int)
            var tipos = await _tipoRepo.GetAllAsync();
            var mapTipo = tipos.ToDictionary(
                t => (long)t.Id,                                 
                t => t.Nombre ?? $"Tipo {t.Id}");

            ViewBag.PropNombres = mapProp;
            ViewBag.TipoNombres = mapTipo;

            return View(inmuebles);
        }


        // GET: Inmueble/Create
        public async Task<IActionResult> Create()
        {
            await LoadCombosAsync();
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                var inmuebleId = await _repo.CreateAsync(inmueble);
                if (inmuebleId <= 0)
                {
                    ModelState.AddModelError("", "No se pudo crear el inmueble. Intente nuevamente.");
                    await LoadCombosAsync(inmueble.PropietarioId, inmueble.TipoId);
                    return View(inmueble);
                }else
                {
                    TempData["Success"] = "Inmueble creado exitosamente.";
                    await LoadCombosAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["Error"] = "Error al crear el inmueble.";
            await LoadCombosAsync();
            return View(inmueble);
        }

        // GET: Inmueble/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var inm = await _repo.GetByIdAsync(id);
            if (inm == null) return NotFound();

            await LoadCombosAsync(inm.PropietarioId, inm.TipoId);
            return View(inm);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Inmueble inmueble)
        {
            if (id != inmueble.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var ok = await _repo.UpdateAsync(inmueble);
                if (!ok) return NotFound();
                return RedirectToAction(nameof(Index));
            }
            await LoadCombosAsync(inmueble.PropietarioId, inmueble.TipoId);
            return View(inmueble);
        }

        // GET: Inmueble/Delete/5  (confirmar suspensión)
        public async Task<IActionResult> Delete(long id)
        {
            var inm = await _repo.GetByIdAsync(id);
            if (inm == null) return NotFound();
            return View(inm);
        }

        // POST: Inmueble/Delete/5  (soft delete → Suspendido = true)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Inmueble/ToggleSuspendido/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSuspendido(long id, bool suspendido)
        {
            var ok = await _repo.UpdateSuspendidoAsync(id, suspendido);
            if (!ok) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        // GET: Inmueble/PorPropietario/3
        public async Task<IActionResult> PorPropietario(long id)
        {
            var lista = await _repo.GetByPropietarioAsync(id);
            return View("Index", lista);
        }

        // GET: Inmueble/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var inmueble = await _repo.GetByIdAsync(id);
            if (inmueble == null) return NotFound();
            
            inmueble.Propietario = await _propietarioRepo.GetByIdAsync(inmueble.PropietarioId);
            inmueble.Tipo = await _tipoRepo.GetByIdAsync(inmueble.TipoId);
            
            // Cargar imágenes del inmueble
            var imagenes = await _imagenRepo.GetByInmuebleIdAsync((int)id);
            ViewBag.Imagenes = imagenes;
            
            return View(inmueble);
        }

        // ================= Helpers =================
        private async Task LoadCombosAsync(long? selectedPropietarioId = null, int? selectedTipoId = null)
        {
            var propietarios = await _propietarioRepo.GetAllAsync();
            ViewBag.PropietarioId = new SelectList(
                propietarios,
                "Id",
                "NombreCompleto", // o una propiedad compuesta con Nombre+Apellido si la tenés
                selectedPropietarioId);

            var tipos = await _tipoRepo.GetAllAsync();
            ViewBag.TipoId = new SelectList(
                tipos,
                "Id",
                "Nombre",
                selectedTipoId);

            // Uso fijo según tu modelo: RESIDENCIAL / COMERCIAL
            ViewBag.Usos = new SelectList(new[]
            {
                new { Value = "RESIDENCIAL", Text = "RESIDENCIAL" },
                new { Value = "COMERCIAL",   Text = "COMERCIAL" }
            }, "Value", "Text");
        }
    }
}
