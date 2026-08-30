using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_BarrosoEsteban.Models;
using Inmobiliaria_BarrosoEsteban;


namespace Inmobiliaria_BarrosoEsteban.Controllers
{

    public class InmuebleController : Controller
    {

        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repoPropietario;

        public InmuebleController(IRepositorioInmueble repositorio, IRepositorioPropietario repoPropietario)
        {
            this.repositorio = repositorio;
            this.repoPropietario = repoPropietario;
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
            return View();
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