using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Inmobiliaria_BarrosoEsteban.Models;

namespace Inmobiliaria_BarrosoEsteban.Controllers
{
    public class LoginController : Controller
    {
        private readonly IRepositorioUsuario repositorio;
        private readonly PasswordHasher<Usuario> hasher = new PasswordHasher<Usuario>();

        // Inyección de dependencias, igual que tus otros controladores
        public LoginController(IRepositorioUsuario repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Login/Index
        [AllowAnonymous]
        public IActionResult Index(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Login/Index
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string email, string clave, string? returnUrl = null)
        {
            var usuario = repositorio.ObtenerPorEmail(email);

            if (usuario == null || !usuario.Estado)
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos");
                return View();
            }

            PasswordVerificationResult resultado;
            try
            {
                resultado = hasher.VerifyHashedPassword(usuario, usuario.Clave, clave);
            }
            catch (FormatException)
            {
                // Captura claves con mal formato Base64 o texto plano sin romper la app
                resultado = PasswordVerificationResult.Failed;
            }

            if (resultado == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol) // "Administrador" o "Empleado"
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
        // POST: Login/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Login/AccesoDenegado
        [AllowAnonymous]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}