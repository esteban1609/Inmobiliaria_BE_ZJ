using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_BarrosoEsteban.Models;
using Inmobiliaria_BarrosoEsteban;


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
        public IActionResult Index()
        {
            var lista = repositorio.Listar();
            return View(lista);
        }

        // CREATE GET
        public IActionResult Create()
        {
            ViewBag.Propietarios = repoPropietario.Listar();
            ViewBag.TiposInmueble = repoTipoInmueble.Listar();
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
		public ActionResult Portada(Imagen entidad, [FromServices] IWebHostEnvironment environment)
		{
			try
			{
				//Recuperar el inmueble y eliminar la imagen anterior
				var inmueble = repositorio.ObtenerPorId(entidad.InmuebleId);
				if (inmueble != null && inmueble.Portada != null)
				{
					string rutaEliminar = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", Path.GetFileName(inmueble.Portada));
					System.IO.File.Delete(rutaEliminar);
				}
				if (entidad.Archivo != null)
				{
					string wwwPath = environment.WebRootPath;
					string path = Path.Combine(wwwPath, "Uploads");
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
					path = Path.Combine(path, "Inmuebles");
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
					//string fileName = Path.GetFileName(entidad.Archivo.FileName);//este nombre se puede repetir
					string fileName = "portada_" + entidad.InmuebleId + Path.GetExtension(entidad.Archivo.FileName);
					string rutaFisicaCompleta = Path.Combine(path, fileName);
					using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
					{
						entidad.Archivo.CopyTo(stream);
					}
					entidad.Url = Path.Combine("/Uploads/Inmuebles", fileName);
				}
				else //sin imagen
				{
					entidad.Url = string.Empty;
				}
				repositorio.ModificarPortada(entidad.InmuebleId, entidad.Url);
				TempData["Mensaje"] = "Portada actualizada correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["Error"] = ex.Message;
				return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
			}
		}



        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(inmueble);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Propietarios = repoPropietario.Listar();
            ViewBag.TiposInmueble = repoTipoInmueble.Listar();
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
            ViewBag.Propietarios = repoPropietario.Listar();
            ViewBag.TiposInmueble = repoTipoInmueble.Listar();
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
            ViewBag.Propietarios = repoPropietario.Listar();
            ViewBag.TiposInmueble = repoTipoInmueble.Listar();
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
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // REACTIVAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            repositorio.Reactivar(id);
            return RedirectToAction(nameof(Index));
        }


    }

}