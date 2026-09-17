using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_BarrosoEsteban.Models;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repoPropietario;
        private readonly IRepositorioTipoInmueble repoTipoInmueble;
        private readonly IRepositorioImagen repositorioImagen;

        public InmuebleController(
            IRepositorioInmueble repositorio, 
            IRepositorioPropietario repoPropietario, 
            IRepositorioTipoInmueble repoTipoInmueble, 
            IRepositorioImagen repositorioImagen)
        {
            this.repositorio = repositorio;
            this.repoPropietario = repoPropietario;
            this.repoTipoInmueble = repoTipoInmueble;
            this.repositorioImagen = repositorioImagen;
        }

        // LISTADO
        public IActionResult Index(bool? estado, int? idPropietario, string? busqueda, int paginaNro = 1, int tamPagina = 10)
        {
            IList<Inmueble> lista;

            if (idPropietario.HasValue)
            {
                lista = repositorio.ListarPorPropietario(idPropietario.Value);
                if (estado.HasValue)
                    lista = lista.Where(i => i.Estado == estado.Value).ToList();

                var propietarioSel = repoPropietario.ObtenerPorId(idPropietario.Value);
                ViewBag.PropietarioSeleccionadoId = idPropietario.Value;
                ViewBag.PropietarioSeleccionadoNombre = propietarioSel != null
                    ? $"{propietarioSel.Nombre} {propietarioSel.Apellido}"
                    : "";
            }
            else if (estado.HasValue)
            {
                lista = repositorio.ListarPorEstado(estado.Value);
            }
            else
            {
                lista = repositorio.Listar(paginaNro, tamPagina, busqueda);
            }

            ViewBag.EstadoSeleccionado = estado;
            ViewBag.PaginaNro = paginaNro;
            ViewBag.TamPagina = tamPagina;
            ViewBag.Busqueda = busqueda;

            return View(lista);
        }

        // CREATE GET
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            CargarSelects();
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(inmueble);
                return RedirectToAction(nameof(Index));
            }
            
            CargarSelects();
            return View(inmueble);
        }

        // EDIT GET
        
        public IActionResult Edit(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            
            CargarSelects();
            return View(inmueble);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public IActionResult Edit(int id, Inmueble i)
        {
            i.IdInmueble = id;
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(i);
                return RedirectToAction(nameof(Index));
            }
            
            CargarSelects();
            return View(i);
        }

        // DETAILS
        public IActionResult Details(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // DELETE GET
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // DELETE POST / BAJA LOGICA
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // REACTIVAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Reactivar(int id)
        {
            repositorio.Reactivar(id);
            return RedirectToAction(nameof(Index));
        }

        // GALERIA E IMAGENES
        public IActionResult Imagenes(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }

            inmueble.Imagenes = repositorioImagen.BuscarPorInmueble(id);
            return View(inmueble);
        }

        // PORTADA POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Portada(Imagen entidad, [FromServices] IWebHostEnvironment environment)
        {
            try
            {
                var inmueble = repositorio.ObtenerPorId(entidad.InmuebleId);
                if (inmueble == null)
                {
                    return NotFound();
                }

                if (entidad.Archivo != null && entidad.Archivo.Length > 0)
                {
                    // Eliminar portada anterior si existe
                    if (!string.IsNullOrWhiteSpace(inmueble.Portada))
                    {
                        string rutaEliminar = Path.Combine(
                            environment.WebRootPath,
                            "Uploads",
                            "Inmuebles",
                            Path.GetFileName(inmueble.Portada)
                        );

                        if (System.IO.File.Exists(rutaEliminar))
                        {
                            System.IO.File.Delete(rutaEliminar);
                        }
                    }

                    string path = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = $"portada_{entidad.InmuebleId}_{Guid.NewGuid()}{Path.GetExtension(entidad.Archivo.FileName)}";
                    string rutaFisicaCompleta = Path.Combine(path, fileName);

                    using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                    {
                        entidad.Archivo.CopyTo(stream);
                    }

                    entidad.Url = $"/Uploads/Inmuebles/{fileName}";
                    repositorio.ModificarPortada(entidad.InmuebleId, entidad.Url);
                    TempData["Mensaje"] = "Portada actualizada correctamente";
                }
                else
                {
                    TempData["Error"] = "Debe seleccionar un archivo de imagen válido.";
                }

                return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
            }
        }

        [HttpGet]
        public JsonResult BuscarJson(string term)
        {
            var lista = repositorio.Buscar(term ?? "");
            var resultado = lista.Select(i => new
            {
                id = i.IdInmueble,
                text = i.Direccion
            });

            return Json(new { results = resultado });
        }

        // Método auxiliar para evitar duplicación de código en los desplegables
        private void CargarSelects()
        {
            ViewBag.TiposInmueble = repoTipoInmueble.Listar();
            ViewBag.Propietarios = repoPropietario.Listar();
        }
    }
}