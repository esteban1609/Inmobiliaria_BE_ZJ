using System.Security.Claims;
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

        private int IdUsuarioActual => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public IActionResult Index(int idReserva,int paginaNro = 1, int tamPagina = 10)
        {
            ViewBag.IdReserva = idReserva;
            ViewBag.PaginaNro = paginaNro;
            ViewBag.TamPagina = tamPagina;
            return View(repositorio.ListarPorReserva(idReserva, paginaNro,tamPagina));
        }

        public IActionResult Create(int idReserva)
        {
            return View(new Pago { IdReserva = idReserva });
        }

        [HttpPost]
        public IActionResult Create(Pago p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            repositorio.Alta(p, IdUsuarioActual);
            return RedirectToAction(nameof(Index), new { idReserva = p.IdReserva });
        }

        public IActionResult Edit(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        public IActionResult Edit(int id, string concepto)
        {
            if (string.IsNullOrWhiteSpace(concepto))
            {
                ModelState.AddModelError("Concepto", "El concepto es obligatorio");
                return View(repositorio.ObtenerPorId(id));
            }

            repositorio.ModificarConcepto(id, concepto);

            var pago = repositorio.ObtenerPorId(id);
            return RedirectToAction(nameof(Index), new { idReserva = pago?.IdReserva });
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Anular(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Anular")]
        [Authorize(Roles = "Administrador")]
        public IActionResult AnularConfirmado(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            repositorio.Anular(id, IdUsuarioActual);
            return RedirectToAction(nameof(Index), new { idReserva = p?.IdReserva });
        }
    }
}