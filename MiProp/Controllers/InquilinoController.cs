using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProp.Data;
using MiProp.Models;

namespace MiProp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InquilinoController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly AppDbContext _context;

        public InquilinoController(UserManager<Usuario> userManager, AppDbContext context)
        {
            _context= context;
            _userManager= userManager;
        }
        // LISTAR
        public async Task<IActionResult> Index()
        {
            var admin = await _userManager.GetUserAsync(User);

            var inquilinos = await _context.Users
                .Where(u => u.EdificioId == admin.EdificioId)
                .Where(u => _context.UserRoles
                    .Any(r => r.UserId == u.Id &&
                              _context.Roles
                                  .Any(ro => ro.Id == r.RoleId && ro.Name == "Inquilino")))
                .ToListAsync();

            var departamentos = await _context.Departamentos
                .Where(d => d.EdificioId == admin.EdificioId)
                .ToListAsync();

            ViewBag.Departamentos = departamentos;

            return View(inquilinos);
        }

        public async Task<IActionResult> Create()
        {
            var admin = await _userManager.GetUserAsync(User);

            var departamentos = await _context.Departamentos
                .Where(d => d.EdificioId == admin.EdificioId)
                .Where(d => d.InquilinoId == null)
                .ToListAsync();

            ViewBag.Departamentos = departamentos;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string nombre, string apellido, string email, string password, int departamentoId)
        {
            var admin = await _userManager.GetUserAsync(User);

            var usuario = new Usuario
            {
                UserName = email,
                Email = email,
                Nombre = nombre,
                Apellido = apellido,
                EdificioId = admin.EdificioId
            };

            var result = await _userManager.CreateAsync(usuario, password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error creando usuario");
                return View();
            }

            await _userManager.AddToRoleAsync(usuario, "Inquilino");

            var departamento = await _context.Departamentos.FindAsync(departamentoId);
            departamento.InquilinoId = usuario.Id;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
