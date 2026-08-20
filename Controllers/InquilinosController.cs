using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly IRepositorioInquilino repositorio;

        public InquilinosController(IRepositorioInquilino repositorio)
        {
            this.repositorio = repositorio;
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
                repositorio.alta(inquilino);
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }
    }
}