using Inmobiliaria.Models;
using Inmobiliaria.Repositories;
using Inmobiliaria.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IUsuarioRepository _repo;

        public AuthController(IAuthService auth, IUsuarioRepository repo)
        {
            _auth = auth;
            _repo = repo;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            var (ok, user, error) = await _auth.ValidateCredentialsAsync(email, password);
            if (!ok || user is null)
            {
                ModelState.AddModelError(string.Empty, error ?? "Email o contraseña inválidos.");
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var principal = _auth.CreatePrincipal(user);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
            return View(new Usuario { IsActive = true, RolId = 3 });
        }

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario u, string password)
        {
            if (await _repo.ExistsByEmailAsync(u.Email))
                ModelState.AddModelError(nameof(u.Email), "Ya existe un usuario con ese email.");
            if (!ModelState.IsValid) return View(u);

            u.PasswordHash = _auth.HashPassword(password);
            u.CreatedAt = DateTime.UtcNow;
            u.UpdatedAt = DateTime.UtcNow;

            var id = await _repo.CreateAsync(u);
            u.Id = id;

            var principal = _auth.CreatePrincipal(u);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
    }
}
