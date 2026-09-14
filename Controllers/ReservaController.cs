using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repositorio;
        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioPago repositorioPago;

        public ReservaController(IConfiguration configuration)
        {
            repositorio = new RepositorioReserva(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioPago = new RepositorioPago(configuration);
        }

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

        // GET: Reserva/Details/5
        public IActionResult Details(int id)
        {
            var r = repositorio.ObtenerPorId(id);
            if (r == null) return NotFound();

            ViewBag.Pagos = repositorioPago.ListarPorReserva(id);
            return View(r);
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
            if (!ModelState.IsValid)
            {
                CargarListas();
                return View(r);
            }

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
            if (!ModelState.IsValid)
            {
                CargarListas();
                return View(r);
            }

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