
using Inmobiliaria_BarrosoEsteban.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly IRepositorioPropietario repositorio;

        public PropietarioController(IRepositorioPropietario repositorio)
        {
            this.repositorio = repositorio;
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
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(p);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar: " + ex.Message);
            }

            return View(p);
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
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario p)
        {
            try
            {
                p.IdPropietario = id;

                if (ModelState.IsValid)
                {
                    repositorio.Modificacion(p);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al modificar: " + ex.Message);
            }

            return View(p);
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
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar el propietario: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Propietario/Reactivar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            try
            {
                repositorio.Reactivar(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo reactivar el propietario: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}