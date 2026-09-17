
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{
    [Authorize]
    public class PropietarioController : Controller
    {
        private readonly IRepositorioPropietario repositorio;

        public PropietarioController(IRepositorioPropietario repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Propietario
        public IActionResult Index(int paginaNro = 1, int tamPagina = 10, string? busqueda = null)
        {
            var lista = repositorio.Listar(paginaNro, tamPagina, busqueda);
            ViewBag.PaginaNro = paginaNro;
            ViewBag.TamPagina = tamPagina;
            ViewBag.Busqueda = busqueda;
            return View(lista);
        }

        // GET: Propietario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietario/Create
        [HttpPost]
        public IActionResult Create(Propietario p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            repositorio.Alta(p);
            return RedirectToAction(nameof(Index));
        }

        // GET: Propietario/Edit/5
        public IActionResult Edit(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Propietario p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            p.IdPropietario = id;
            repositorio.Modificacion(p);
            return RedirectToAction(nameof(Index));
        }

        // POST: Propietario/Delete/5 (baja lógica) -- SOLO Administrador
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Propietario/Reactivar/5 -- SOLO Administrador
        [HttpPost]
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
        
            var resultado = lista.Select(p => new
            {
                id = p.IdPropietario,
                text = $"{p.Nombre} {p.Apellido}"
            });
        
            return Json(new { results = resultado });
        }
    }
    
}