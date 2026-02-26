using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MiProp.Models;
using MiProp.Models.ViewModels;

namespace MiProp.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdministradoresController : Controller
    {
        private readonly UserManager<Usuario> _userManager;

        public AdministradoresController(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return View(admins);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminCreateVM model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Activo = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return RedirectToAction(nameof(Index));
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new AdminEditVM
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                Activo = user.Activo
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminEditVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            user.Nombre = model.Nombre;
            user.Apellido = model.Apellido; 
            user.Email = model.Email;
            user.UserName = model.Email;
            user.Activo = model.Activo;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
