using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiProp.Services;

namespace MiProp.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly IEmailService _emailService;

        public SuperAdminController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMailPrueba()
        {
            await _emailService.SendAsync(
                "nicolasrios.dev@gmail.com",
                "Prueba de Email",
                "<h2>Email enviado correctamente</h2><p>El sistema de emails funciona.</p>"
            );

            TempData["Mensaje"] = "Email enviado correctamente";
            return RedirectToAction("Dashboard");
        }

    }
}
