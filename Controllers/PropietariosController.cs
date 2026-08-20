
using Inmobiliaria_BarrosoEsteban.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly IRepositorioPropietario repositorio;

        public PropietarioController(IConfiguration configuration)
        {
            repositorio = new RepositorioPropietario(configuration);
        }

        // GET: Propietario
        public IActionResult Index()
        {
            var lista = repositorio.Listar();
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
            p.IdPropietario = id;
            repositorio.Modificacion(p);
            return RedirectToAction(nameof(Index));
        }

        // GET: Propietario/Delete/5
        public IActionResult Delete(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: Propietario/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}