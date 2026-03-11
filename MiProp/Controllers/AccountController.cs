using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiProp.Models;
using MiProp.Models.ViewModels;
using MiProp.Services;

namespace MiProp.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
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
                    return RedirectToAction("Dashboard", "InquilinoPanel");

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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ResetPasswordVM model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return RedirectToAction("ForgotPasswordConfirmation");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = Url.Action(
                "ResetPassword",
                "Account",
                new { token, email = user.Email },
                Request.Scheme
            );

            await _emailService.SendAsync(
                user.Email,
                "Recuperar contraseña",
                $"""
                <h2>Recuperar contraseña</h2>
                <p>Haz click en el siguiente enlace:</p>
                <a href="{link}">Cambiar contraseña</a>
                <p>Este enlace expirará pronto.</p>
                """
            );
            TempData["SuccessMessage"] = "Si el email existe en el sistema, se ha enviado un enlace para recuperar la contraseña.";
            return RedirectToAction("ForgotPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordVM
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.Token,
                model.Password
            );

            if (result.Succeeded)
                TempData["SuccessMessage"] = "Contraseña modificada con exito.";

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
    }
}
