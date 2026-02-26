using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiProp.Models;

namespace MiProp.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public AccountController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user == null || !user.Activo)
            {
                ModelState.AddModelError("", "Usuario inválido o inactivo.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                false
            );

            if(result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                    return RedirectToAction("Dashboard", "SuperAdmin");

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Dashboard", "Admin");

                if (await _userManager.IsInRoleAsync(user, "Inquilino"))
                    return RedirectToAction("Dashboard", "Inquilino");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Credenciales incorrectas");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
