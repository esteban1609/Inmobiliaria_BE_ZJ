using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_BarrosoEsteban.Models;
using Inmobiliaria_BarrosoEsteban;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly IRepositorioInquilino repositorio;

        public InquilinosController(IRepositorioInquilino repositorio)
        {
            this.repositorio = repositorio;
        }

        public IActionResult Index()
        {
            var lista = repositorio.Listar();
            return View(lista);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(inquilino);
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        public IActionResult Edit(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                inquilino.IdInquilino = id;

                if (ModelState.IsValid)
                {
                    repositorio.Modificacion(inquilino);
                    return RedirectToAction(nameof(Index));
                }

                return View(inquilino);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(inquilino);
            }
        }

        // GET: Inquilinos/Delete/5
        public IActionResult Delete(int id)
        {
            var i = repositorio.ObtenerPorId(id);
            if (i == null) return NotFound();
            return View(i);
        }

        // POST: Inquilinos/Delete/5
        [HttpPost, ActionName("Delete")] // <-- Mapea la acción para que responda a Delete
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}