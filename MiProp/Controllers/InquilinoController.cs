using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            _context = context;
            _userManager = userManager;
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
                EdificioId = admin.EdificioId,
                EmailConfirmed = true
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

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var admin = await _userManager.GetUserAsync(User);

            var inquilino = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == id &&
                    u.EdificioId == admin.EdificioId);

            if (inquilino == null)
                return NotFound();

            var departamentos = await _context.Departamentos
                .Where(d => d.EdificioId == admin.EdificioId &&
                           (d.InquilinoId == null || d.InquilinoId == id))
                .ToListAsync();

            var departamentoActual = await _context.Departamentos
                .FirstOrDefaultAsync(d => d.InquilinoId == id);

            ViewBag.Departamentos = new SelectList(
                departamentos,
                "Id",
                "NumeroPiso",
                departamentoActual?.Id   // 👈 este es el seleccionado
            );

            return View(inquilino);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Usuario model, int? departamentoId)
        {
            if (id != model.Id)
                return NotFound();

            var admin = await _userManager.GetUserAsync(User);

            var inquilino = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == id &&
                    u.EdificioId == admin.EdificioId);

            if (inquilino == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Departamentos = await _context.Departamentos
                    .Where(d => d.EdificioId == admin.EdificioId &&
                               (d.InquilinoId == null || d.InquilinoId == id))
                    .ToListAsync();

                return View(model);
            }

            // Datos personales
            inquilino.Nombre = model.Nombre;
            inquilino.Apellido = model.Apellido;
            inquilino.Email = model.Email;
            inquilino.UserName = model.Email;
            inquilino.Activo = model.Activo;

            // Buscar departamento actual
            var departamentoActual = await _context.Departamentos
                .FirstOrDefaultAsync(d => d.InquilinoId == id);

            if (departamentoActual != null && departamentoActual.Id != departamentoId)
            {
                departamentoActual.InquilinoId = null;
            }

            if (departamentoId.HasValue)
            {
                var nuevoDepartamento = await _context.Departamentos
                    .FirstOrDefaultAsync(d => d.Id == departamentoId);

                if (nuevoDepartamento != null)
                {
                    nuevoDepartamento.InquilinoId = id;
                }
            }

            var result = await _userManager.UpdateAsync(inquilino);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ActivarInactivar(string id)
        {
            var inquilino = await _context.Users.FindAsync(id);

            if (inquilino == null)
                return NotFound();

            inquilino.Activo = !inquilino.Activo;

            var departamento = await _context.Departamentos
                .FirstOrDefaultAsync(d => d.InquilinoId == id);

            if (departamento != null)
                departamento.InquilinoId = null;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
