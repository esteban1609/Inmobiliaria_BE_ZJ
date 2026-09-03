using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repositorio;
        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioInmueble repositorioInmueble;

        public ReservaController(IConfiguration configuration)
        {
            repositorio = new RepositorioReserva(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
        }

        // Carga los combos de Inquilino e Inmueble para Create/Edit
        private void CargarListas()
        {
            ViewBag.Inquilinos = repositorioInquilino.Listar();
            ViewBag.Inmuebles = repositorioInmueble.Listar();
        }

        // GET: Reserva
        public IActionResult Index()
        {
            var lista = repositorio.Listar();
            return View(lista);
        }

        // GET: Reserva/Create
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        // POST: Reserva/Create
        [HttpPost]
        public IActionResult Create(Reserva r)
        {
            repositorio.Alta(r);
            return RedirectToAction(nameof(Index));
        }

        // GET: Reserva/Edit/5
        public IActionResult Edit(int id)
        {
            var r = repositorio.ObtenerPorId(id);
            if (r == null) return NotFound();
            CargarListas();
            return View(r);
        }

        // POST: Reserva/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Reserva r)
        {
            r.IdReserva = id;
            repositorio.Modificacion(r);
            return RedirectToAction(nameof(Index));
        }

        // GET: Reserva/Delete/5
        public IActionResult Delete(int id)
        {
            var r = repositorio.ObtenerPorId(id);
            if (r == null) return NotFound();
            return View(r);
        }

        // POST: Reserva/Delete/5 (baja lógica)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Reserva/Reactivar/5
        [HttpPost]
        public IActionResult Reactivar(int id)
        {
            repositorio.Reactivar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}