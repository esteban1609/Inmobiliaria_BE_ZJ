using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IRepositorioUsuario repositorio;
        private readonly PasswordHasher<Usuario> hasher = new PasswordHasher<Usuario>();

        public UsuarioController(IRepositorioUsuario repositorio)
        {
            this.repositorio = repositorio;
        }

        // ===================== GESTIÓN DE OTROS USUARIOS (SOLO ADMINISTRADOR) =====================

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View(repositorio.Listar());
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Usuario u, string claveTextoPlano)
        {
            if (!ModelState.IsValid)
            {
                return View(u);
            }

            u.Clave = hasher.HashPassword(u, claveTextoPlano);
            repositorio.Alta(u);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var u = repositorio.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id, Usuario u)
        {
            if (!ModelState.IsValid)
            {
                return View(u);
            }

            u.IdUsuario = id;
            repositorio.Modificacion(u); // esta sí puede tocar el Rol
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Reactivar(int id)
        {
            repositorio.Reactivar(id);
            return RedirectToAction(nameof(Index));
        }

        // ===================== PERFIL PROPIO (CUALQUIER USUARIO LOGUEADO) =====================

        // GET: Usuario/MiPerfil
        public IActionResult MiPerfil()
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var u = repositorio.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        // POST: Usuario/MiPerfil (solo nombre/apellido/avatar -- NO email ni rol)
        [HttpPost]
        public IActionResult MiPerfil(Usuario u)
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            u.IdUsuario = id; // fuerza el id de la sesión, ignora cualquier otro id que llegue del form

            repositorio.ModificarPerfilPropio(u);
            return RedirectToAction(nameof(MiPerfil));
        }

        // GET: Usuario/CambiarClave
        public IActionResult CambiarClave()
        {
            return View();
        }

        // POST: Usuario/CambiarClave
        [HttpPost]
        public IActionResult CambiarClave(string claveActual, string claveNueva)
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var u = repositorio.ObtenerPorId(id);
            if (u == null) return NotFound();

            var verificacion = hasher.VerifyHashedPassword(u, u.Clave, claveActual);
            if (verificacion == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "La contraseña actual no es correcta");
                return View();
            }

            string nuevoHash = hasher.HashPassword(u, claveNueva);
            repositorio.ActualizarClave(id, nuevoHash);

            return RedirectToAction(nameof(MiPerfil));
        }
    }
}