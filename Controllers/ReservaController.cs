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

            var lista = repositorio.ListarQueTerminanEnDias(dias);

            ViewBag.Dias = dias;

            return View(lista);
        }

        //Reservas disponibles
        public IActionResult Disponibles(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            IList<Inmueble> lista =
                new List<Inmueble>();

            if (fechaDesde.HasValue &&
                fechaHasta.HasValue)
            {
                if (fechaHasta.Value < fechaDesde.Value)
                {
                    ViewBag.Error = "La fecha hasta no puede ser anterior a la fecha desde.";
                }
                else
                {
                    lista =
                        repositorio.ListarInmueblesDisponibles(fechaDesde.Value, fechaHasta.Value);
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

            // ---- Generación automática de la seña, según el % que tiene cargado el inmueble ----
            var inmueble = repositorioInmueble.ObtenerPorId(r.IdInmueble);
            if (inmueble != null && inmueble.PorcentajeReserva > 0)
            {
                int cantidadDias = (r.FechaHasta - r.FechaDesde).Days;
                if (cantidadDias <= 0) cantidadDias = 1; // por si la reserva es de un solo día

                decimal montoTotal = r.MontoDia * cantidadDias;
                decimal montoSena = montoTotal * (inmueble.PorcentajeReserva / 100m);

                var sena = new Pago
                {
                    IdReserva = r.IdReserva,
                    Concepto = "Seña inicial",
                    FechaPago = DateTime.Today,
                    Importe = montoSena
                };

                repositorioPago.Alta(sena, IdUsuarioActual);
            }
            // ------------------------------------------------------------------------------------

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

        // GET: Reserva/Terminar/5 -- muestra la multa calculada, permite ajustar la fecha efectiva
        public IActionResult Terminar(int id, DateTime? fechaEfectiva)
        {
            var r = repositorio.ObtenerPorId(id);
            if (r == null) return NotFound();
            if (!r.Estado)
            {
                TempData["Error"] = "Esta reserva ya no está activa.";
                return RedirectToAction(nameof(Details), new { id });
            }

            DateTime fecha = fechaEfectiva ?? DateTime.Today;
            if (fecha < r.FechaDesde) fecha = r.FechaDesde;
            if (fecha > r.FechaHasta) fecha = r.FechaHasta;

            decimal multa = CalcularMulta(r, fecha);

            ViewBag.FechaEfectiva = fecha;
            ViewBag.Multa = multa;
            return View(r);
        }

        // POST: Reserva/Terminar/5 -- recalcula la multa en el servidor (nunca confía en un monto
        // que venga del formulario) y recién ahí genera el pago y termina la reserva.
        [HttpPost, ActionName("Terminar")]
        public IActionResult TerminarConfirmado(int id, DateTime fechaEfectiva)
        {
            var r = repositorio.ObtenerPorId(id);
            if (r == null) return NotFound();
            if (!r.Estado)
            {
                TempData["Error"] = "Esta reserva ya no está activa.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (fechaEfectiva < r.FechaDesde || fechaEfectiva > r.FechaHasta)
            {
                ModelState.AddModelError(string.Empty, "La fecha de terminación debe estar dentro del período original de la reserva.");
                ViewBag.FechaEfectiva = fechaEfectiva;
                ViewBag.Multa = CalcularMulta(r, fechaEfectiva);
                return View("Terminar", r);
            }

            decimal multa = CalcularMulta(r, fechaEfectiva);

            // "Si el inquilino no paga en el momento, no puede finalizarse" -- por eso el pago
            // de la multa y la terminación de la reserva se hacen juntos, en la misma operación.
            var pagoMulta = new Pago
            {
                IdReserva = r.IdReserva,
                Concepto = "Multa por cancelación anticipada",
                FechaPago = DateTime.Today,
                Importe = multa
            };
            repositorioPago.Alta(pagoMulta, IdUsuarioActual);

            repositorio.Terminar(id, IdUsuarioActual, fechaEfectiva);

            return RedirectToAction(nameof(Details), new { id });
        }

        // Cálculo de la multa según la narrativa:
        // - Menos de la mitad del tiempo original cumplido -> 50% del alquiler restante
        // - La mitad o más cumplido -> 25% del alquiler restante
        // "Alquiler restante" = lo que faltaba cobrar de los días que no se van a usar.
        private decimal CalcularMulta(Reserva r, DateTime fechaEfectiva)
        {
            int diasOriginales = (r.FechaHasta - r.FechaDesde).Days;
            if (diasOriginales <= 0) diasOriginales = 1;

            int diasCumplidos = (fechaEfectiva - r.FechaDesde).Days;
            if (diasCumplidos < 0) diasCumplidos = 0;
            if (diasCumplidos > diasOriginales) diasCumplidos = diasOriginales;

            int diasRestantes = diasOriginales - diasCumplidos;
            decimal montoRestante = r.MontoDia * diasRestantes;

            decimal porcentajeMulta = diasCumplidos < (diasOriginales / 2.0)
                ? 0.50m
                : 0.25m;

            return Math.Round(montoRestante * porcentajeMulta, 2);
        }

        [Authorize(Roles ="Administrador")]
        public IActionResult Renovar(int id)
        {
            var reservaOriginal = repositorio.ObtenerPorId(id);

            if (reservaOriginal == null)
                return NotFound();

            var nuevaReserva = new Reserva
            {
                IdInquilino = reservaOriginal.IdInquilino,
                IdInmueble = reservaOriginal.IdInmueble,

                // La renovación comienza después de finalizar la anterior
                FechaDesde = reservaOriginal.FechaHasta.AddDays(1),

                // Esto lo deberá elegir el usuario
                FechaHasta = reservaOriginal.FechaHasta.AddDays(1),

                // Podemos sugerir el monto anterior, pero se puede modificar
                MontoDia = reservaOriginal.MontoDia
            };

            // Buscar los datos completos
            ViewBag.Inquilino = repositorioInquilino.ObtenerPorId(reservaOriginal.IdInquilino);
            ViewBag.Inmueble = repositorioInmueble.ObtenerPorId(reservaOriginal.IdInmueble);

            return View(nuevaReserva);
        }


        [HttpPost]
        [Authorize(Roles ="Administrador")]
        public IActionResult Renovar(Reserva nuevaReserva)
        {
            if (!ModelState.IsValid)
            {
                return View(nuevaReserva);
            }

            repositorio.Alta(nuevaReserva, IdUsuarioActual);

            TempData["Mensaje"] = "La renovación se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}