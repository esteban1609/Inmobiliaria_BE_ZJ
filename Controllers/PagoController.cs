using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorio;

        public PagoController(IRepositorioPago repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Pago?idReserva=5
        public IActionResult Index(int idReserva)
        {
            ViewBag.IdReserva = idReserva;
            var lista = repositorio.ListarPorReserva(idReserva);
            return View(lista);
        }

        // GET: Pago/Create?idReserva=5
        public IActionResult Create(int idReserva)
        {
            var pago = new Pago { IdReserva = idReserva };
            return View(pago);
        }

        // POST: Pago/Create
        [HttpPost]
        public IActionResult Create(Pago p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            repositorio.Alta(p);
            return RedirectToAction(nameof(Index), new { idReserva = p.IdReserva });
        }

        // GET: Pago/Edit/5
        public IActionResult Edit(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (p == null) return NotFound();

            return View(p);
        }

        // POST: Pago/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, string concepto)
        {
            if (string.IsNullOrWhiteSpace(concepto))
            {
                ModelState.AddModelError("Concepto", "El concepto es obligatorio");
                var p = repositorio.ObtenerPorId(id);
                return View(p);
            }

            repositorio.ModificarConcepto(id, concepto);

            var pago = repositorio.ObtenerPorId(id);
            return RedirectToAction(nameof(Index), new { idReserva = pago?.IdReserva });
        }

        // GET: Pago/Anular/5 -- SOLO Administrador
        [Authorize(Roles = "Administrador")]
        public IActionResult Anular(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (p == null) return NotFound();

            return View(p);
        }

        // POST: Pago/Anular/5 -- SOLO Administrador
        [HttpPost, ActionName("Anular")]
        [Authorize(Roles = "Administrador")]
        public IActionResult AnularConfirmado(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            repositorio.Anular(id);
            return RedirectToAction(nameof(Index), new { idReserva = p?.IdReserva });
        }
    }
}