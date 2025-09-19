using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class ImagenController : Controller
    {
        private readonly IImagenRepository _imagenRepo;
        private readonly IInmuebleRepository _inmuebleRepo;
        private readonly IWebHostEnvironment _env;

        public ImagenController(IImagenRepository imagenRepo, IInmuebleRepository inmuebleRepo, IWebHostEnvironment env)
        {
            _imagenRepo = imagenRepo;
            _inmuebleRepo = inmuebleRepo;
            _env = env;
        }

        // GET: Imagen/GaleriaInmueble/5
        public async Task<IActionResult> GaleriaInmueble(int inmuebleId)
        {
            var imagenes = await _imagenRepo.GetByInmuebleIdAsync(inmuebleId);
            ViewBag.InmuebleId = inmuebleId;
            return View(imagenes);
        }

        // POST: Imagen/SubirImagen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirImagen(int inmuebleId, List<IFormFile> archivos)
        {
            if (archivos == null || !archivos.Any() || archivos.Count == 0)
            {
                TempData["Error"] = "Debe seleccionar al menos un archivo.";
                return RedirectToAction("GaleriaInmueble", new { inmuebleId });
            }

            foreach (var archivo in archivos)
            {
                if (archivo == null || archivo.Length == 0)
                {
                    TempData["Error"] = "Uno de los archivos seleccionados está vacío.";
                    return RedirectToAction("GaleriaInmueble", new { inmuebleId });
                }
            }

            // Si no es portada, validar límite de 5 imágenes en galería
            var imagenesExistentes = await _imagenRepo.GetByInmuebleIdAsync(inmuebleId);
            if (imagenesExistentes.Count() >= 5)
            {
                TempData["Error"] = "Máximo 5 imágenes permitidas en la galería. Elimine alguna imagen antes de agregar una nueva.";
                return RedirectToAction("GaleriaInmueble", new { inmuebleId });
            }
            else if (imagenesExistentes.Count() + archivos.Count > 5)
            {
                TempData["Error"] = $"Solo puede agregar {5 - imagenesExistentes.Count()} imágenes más para no superar el límite de 5.";
                return RedirectToAction("GaleriaInmueble", new { inmuebleId });
            }

            // Validar que sea una imagen
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            foreach (var archivo in archivos)
            {
                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(extension))
                {
                    TempData["Error"] = "Solo se permiten archivos de imagen (jpg, jpeg, png, gif, webp).";
                    return RedirectToAction("GaleriaInmueble", new { inmuebleId });
                }

                // Validar tamaño (max 10MB)
                if (archivo.Length > 10 * 1024 * 1024)
                {
                    TempData["Error"] = "El archivo no puede ser mayor a 10MB.";
                    return RedirectToAction("GaleriaInmueble", new { inmuebleId });
                }


                try
                {
                    // Crear directorio específico para este inmueble en la galería
                    var uploadsPath = Path.Combine(_env.WebRootPath, "Uploads", inmuebleId.ToString());
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    // Generar nombre único para el archivo
                    var fileName = $"{inmuebleId}_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    // Guardar archivo
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await archivo.CopyToAsync(stream);
                    }

                    var imageUrl = $"/Uploads/{inmuebleId}/{fileName}";

                    // Guardar en galería (tabla imagenes)
                    var imagen = new Imagen
                    {
                        InmuebleId = inmuebleId,
                        Url = imageUrl
                    };

                    await _imagenRepo.CreateAsync(imagen);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al subir la imagen: {ex.Message}";
                    return RedirectToAction("GaleriaInmueble", new { inmuebleId });
                }
            }
            
            TempData["Success"] = "Imágenes agregadas a la galería correctamente.";
            return RedirectToAction("GaleriaInmueble", new { inmuebleId });
        }

        // POST: Imagen/EliminarImagen/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarImagen(long id, int inmuebleId)
        {
            try
            {
                var imagen = await _imagenRepo.GetByIdAsync(id);
                if (imagen == null)
                {
                    TempData["Error"] = "Imagen no encontrada.";
                    return RedirectToAction("GaleriaInmueble", new { inmuebleId });
                }

                // Eliminar archivo físico
                var filePath = Path.Combine(_env.WebRootPath, imagen.Url.TrimStart('/').Replace('/', '\\'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Eliminar de base de datos
                await _imagenRepo.DeleteAsync(id);
                TempData["Success"] = "Imagen eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar la imagen: {ex.Message}";
            }

            return RedirectToAction("GaleriaInmueble", new { inmuebleId });
        }

        // POST: Imagen/EstablecerPortada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Portada_CreateOrDelete(Imagen file)
        {
            try
            {
                var inmueble = await _inmuebleRepo.GetByIdAsync(file.InmuebleId);
                if (inmueble != null && !string.IsNullOrEmpty(inmueble.Portada_Url))
                {
                    // Construir la ruta física del archivo a eliminar
                    string rutaEliminar = Path.Combine(_env.WebRootPath, inmueble.Portada_Url.TrimStart('/').Replace('/', '\\'));
                    if (System.IO.File.Exists(rutaEliminar))
                    {
                        System.IO.File.Delete(rutaEliminar);
                    }
                }

                if (file.Archivo != null)
                {
                    // Crear directorio base si no existe
                    string path = Path.Combine(_env.WebRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    // Nombre del archivo: solo el ID del inmueble + extensión
                    string fileName = file.InmuebleId + Path.GetExtension(file.Archivo.FileName);
                    string rutaFisicaCompleta = Path.Combine(path, fileName);
                    
                    using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                    {
                        await file.Archivo.CopyToAsync(stream);
                    }
                    
                    file.Url = $"/Uploads/{fileName}";
                }
                else
                { 
                    file.Url = string.Empty;
                }

                await _inmuebleRepo.UpdatePortadaAsync(file.InmuebleId, file.Url);
                TempData["Success"] = "Portada actualizada correctamente.";
                return RedirectToAction("GaleriaInmueble", new { inmuebleId = file.InmuebleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al establecer la portada: {ex.Message}";
                return RedirectToAction("GaleriaInmueble", new { inmuebleId = file.InmuebleId });
            }
        }
    }
}