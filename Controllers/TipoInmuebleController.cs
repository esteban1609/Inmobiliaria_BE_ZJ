
using Microsoft.AspNetCore.Mvc;            
using Microsoft.AspNetCore.Authorization;  
using Inmobiliaria_BarrosoEsteban.Models;  

namespace Inmobiliaria_BarrosoEsteban


{
    [Authorize]
    public class TipoInmuebleController : Controller
    {
        private readonly IRepositorioTipoInmueble repositorio;


        public TipoInmuebleController(IRepositorioTipoInmueble repositorio)
        {
            this.repositorio = repositorio;
        }

        public IActionResult Index(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = repositorio.Listar(paginaNro, tamPagina);
            ViewBag.PaginaNro = paginaNro;
            ViewBag.TamPagina = tamPagina;
            return View(lista);
        }

        //CREATE GET
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        //CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(TipoInmueble tipoInmueble)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(tipoInmueble);
                return RedirectToAction(nameof(Index));
            }
            return View(tipoInmueble);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public IActionResult Edit(int id, TipoInmueble tipo)
        {
            tipo.id_tipo = id;

            if (ModelState.IsValid)
            {
                repositorio.Modificacion(tipo);
                return RedirectToAction(nameof(Index));
            }

            return View(tipo);
        }


        // EDIT GET
        public IActionResult Edit(int id)
        {
            var tipo = repositorio.ObtenerPorId(id);

            if (tipo == null)
            {
                return NotFound();
            }

            return View(tipo);
        }

        // DETAILS
        public IActionResult Details(int id)
        {
            var tipo = repositorio.ObtenerPorId(id);

            if (tipo == null)
            {
                return NotFound();
            }

            return View(tipo);
        }

        // BAJA LOGICA
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DarDeBaja(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // REACTIVAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            repositorio.Reactivar(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult BuscarJson(string term)
        {
            var lista = repositorio.Buscar(term ?? "");
        
            var resultado = lista.Select(t => new
            {
                id = t.id_tipo,
                text = t.Nombre
            });
        
            return Json(new { results = resultado });
        }

    }



}