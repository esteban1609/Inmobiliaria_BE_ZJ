using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_BarrosoEsteban.Models;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{

    public class InmuebleController : Controller
    {

        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repoPropietario;
        private readonly IRepositorioTipoInmueble repoTipoInmueble;

        private readonly IRepositorioImagen repositorioImagen;
        

        public InmuebleController(IRepositorioInmueble repositorio, IRepositorioPropietario repoPropietario, IRepositorioTipoInmueble repositorioTipoInmueble, IRepositorioImagen repositorioImagen)
        {
            this.repositorio = repositorio;
            this.repoPropietario = repoPropietario;
            this.repoTipoInmueble = repositorioTipoInmueble;
            this.repositorioImagen = repositorioImagen;
        }

        // LISTADO
        public IActionResult Index(bool? estado, int? idPropietario, string? busqueda,int paginaNro = 1, int tamPagina = 10)
        {
            IList<Inmueble> lista;
        
            if (idPropietario.HasValue)
            {
                lista = repositorio.ListarPorPropietario(idPropietario.Value);
                if (estado.HasValue)
                    lista = lista.Where(i => i.Estado == estado.Value).ToList();
        
                // Trae SOLO el nombre del propietario seleccionado (no la lista completa)
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
        public IActionResult Create()
        {
            
            return View();
        }


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


        // POST: Inmuebles/Portada
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Administrador")]
public ActionResult Portada(
    Imagen entidad,
    [FromServices] IWebHostEnvironment environment)
{
    try
    {
        var inmueble = repositorio.ObtenerPorId(entidad.InmuebleId);

        if (inmueble == null)
        {
            return NotFound();
        }

        // Eliminar portada anterior
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

        // Si seleccionó una nueva imagen
        if (entidad.Archivo != null)
        {
            string path = Path.Combine(
                environment.WebRootPath,
                "Uploads",
                "Inmuebles"
            );

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName =
                "portada_" +
                entidad.InmuebleId +
                Path.GetExtension(entidad.Archivo.FileName);

            string rutaFisicaCompleta =
                Path.Combine(path, fileName);

            using (var stream = new FileStream(
                rutaFisicaCompleta,
                FileMode.Create))
            {
                entidad.Archivo.CopyTo(stream);
            }

            // URL para mostrar la imagen en el navegador
            entidad.Url = $"/Uploads/Inmuebles/{fileName}";
        }
        else
        {
            entidad.Url = string.Empty;
        }

        repositorio.ModificarPortada(
            entidad.InmuebleId,
            entidad.Url
        );

        TempData["Mensaje"] =
            "Portada actualizada correctamente";

        // Volver al administrador de imágenes
        return RedirectToAction(
            nameof(Imagenes),
            new { id = entidad.InmuebleId }
        );
    }
    catch (Exception ex)
    {
        TempData["Error"] = ex.Message;

        return RedirectToAction(
            nameof(Imagenes),
            new { id = entidad.InmuebleId }
        );
    }
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
            
            return View(inmueble);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id, Inmueble i)
        {
            i.IdInmueble = id;
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(i);
                return RedirectToAction(nameof(Index));
            }
            
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






    }

}