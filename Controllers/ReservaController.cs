using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inmobiliaria_BarrosoEsteban.Models;
using Microsoft.AspNetCore.Authorization;

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


        //Inmuebles mas reservados en los ultimos 365 dias
        public IActionResult MasReservados()
        {
            var lista =
                repositorio.MasReservadosUltimos365Dias();

            return View(lista);
        }

        //inmuebles sin reservas
        public IActionResult SinReservas(int dias = 30)
        {
            if (dias <= 0)
            {
                dias = 30;
            }

            var lista =
                repositorio.SinReservasUltimosDias(dias);

            ViewBag.Dias = dias;

            return View(lista);
        }


        //Reservas Vigentes
        public IActionResult Vigentes()
        {
            var lista =
                repositorio.ListarVigentes();

            return View(lista);
        }

        //Reservas que estan por finalizar
        public IActionResult ProximasAFinalizar(int dias = 30)
        {
            if (dias <= 0)
            {
                dias = 30;
            }

            var lista =
                repositorio.ListarQueTerminanEnDias(dias);

            ViewBag.Dias = dias;

            return View(lista);
        }

        //Reservas disponibles
        public IActionResult Disponibles(DateTime? fechaDesde,DateTime? fechaHasta)
        {
            IList<Inmueble> lista =
                new List<Inmueble>();

            if (fechaDesde.HasValue &&
                fechaHasta.HasValue)
            {
                if (fechaHasta.Value < fechaDesde.Value)
                {
                    ViewBag.Error ="La fecha hasta no puede ser anterior a la fecha desde.";
                }
                else
                {
                    lista =
                        repositorio.ListarInmueblesDisponibles(fechaDesde.Value,fechaHasta.Value);
                }
            }

            ViewBag.FechaDesde = fechaDesde;
            ViewBag.FechaHasta = fechaHasta;

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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Reserva/Reactivar/5
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Reactivar(int id)
        {
            repositorio.Reactivar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}