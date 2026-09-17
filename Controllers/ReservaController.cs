using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repositorio;
        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioPago repositorioPago;

        public ReservaController(
            IRepositorioReserva repositorio,
            IRepositorioInquilino repositorioInquilino,
            IRepositorioInmueble repositorioInmueble,
            IRepositorioPago repositorioPago)
        {
            this.repositorio = repositorio;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPago = repositorioPago;
        }

        private int IdUsuarioActual => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private void CargarListas()
        {
            ViewBag.Inquilinos = repositorioInquilino.Listar();
            ViewBag.Inmuebles = repositorioInmueble.Listar();
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

            var lista =repositorio.ListarQueTerminanEnDias(dias);

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

        public IActionResult Index(int paginaNro = 1, int tamPagina = 10, string? busqueda = null)
        {
            var lista = repositorio.Listar(paginaNro, tamPagina, busqueda);
            ViewBag.PaginaNro = paginaNro;
            ViewBag.TamPagina = tamPagina;
            ViewBag.Busqueda = busqueda;
            return View(lista);
        }

        public IActionResult Details(int id)
        {
            var r = repositorio.ObtenerPorId(id);
            if (r == null) return NotFound();

            ViewBag.Pagos = repositorioPago.ListarPorReserva(id);
            return View(r);
        }

        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Reserva r)
        {
            if (!ModelState.IsValid)
            {
                CargarListas();
                return View(r);
            }
        
            if (r.FechaHasta < r.FechaDesde)
            {
                ModelState.AddModelError(nameof(r.FechaHasta), "La fecha hasta no puede ser anterior a la fecha desde.");
                CargarListas();
                return View(r);
            }
        
            if (repositorio.ExisteSolapamiento(r.IdInmueble, r.FechaDesde, r.FechaHasta))
            {
                ModelState.AddModelError(string.Empty, "El inmueble seleccionado ya está reservado en esas fechas.");
                CargarListas();
                return View(r);
            }
        
            repositorio.Alta(r, IdUsuarioActual);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var r = repositorio.ObtenerPorId(id);
            if (r == null) return NotFound();

            CargarListas();
            return View(r);
        }
        

        [HttpPost]
        public IActionResult Edit(int id, Reserva r)
        {
            if (!ModelState.IsValid)
            {
                CargarListas();
                return View(r);
            }
        
            if (r.FechaHasta < r.FechaDesde)
            {
                ModelState.AddModelError(nameof(r.FechaHasta), "La fecha hasta no puede ser anterior a la fecha desde.");
                CargarListas();
                return View(r);
            }
        
            // idReservaExcluir: no choca contra sí misma al validar
            if (repositorio.ExisteSolapamiento(r.IdInmueble, r.FechaDesde, r.FechaHasta, id))
            {
                ModelState.AddModelError(string.Empty, "El inmueble seleccionado ya está reservado en esas fechas.");
                CargarListas();
                return View(r);
            }
        
            r.IdReserva = id;
            repositorio.Modificacion(r);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id, IdUsuarioActual);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Reactivar(int id)
        {
            repositorio.Reactivar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}