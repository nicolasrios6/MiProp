using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProp.Data;
using MiProp.Models;

namespace MiProp.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class EdificioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EdificioController(AppDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var edificios = _context.Edificios
                .Include(e => e.Admin)
                .ToList();

            return View(edificios);
        }

        // CREATE GET
        public async Task<IActionResult> Create()
        {
            var admins = await ObtenerAdminsDisponibles();
            ViewBag.Admins = admins;
            return View();
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Create(Edificio edificio)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Admins = await ObtenerAdminsDisponibles();
                return View(edificio);
            }

            _context.Add(edificio);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(edificio.AdminId))
            {
                var admin = await _userManager.FindByIdAsync(edificio.AdminId);
                admin.EdificioId = edificio.Id;
                await _userManager.UpdateAsync(admin);
            }

            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var edificio = await _context.Edificios.FindAsync(id);
            if (edificio == null) return NotFound();

            ViewBag.Admins = await ObtenerAdminsDisponibles(edificio.AdminId);
            return View(edificio);
        }

        // EDIT POST
        [HttpPost]
        public async Task<IActionResult> Edit(Edificio edificio)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Admins = await ObtenerAdminsDisponibles(edificio.AdminId);
                return View(edificio);
            }

            _context.Update(edificio);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(edificio.AdminId))
            {
                var admin = await _userManager.FindByIdAsync(edificio.AdminId);
                admin.EdificioId = edificio.Id;
                await _userManager.UpdateAsync(admin);
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var edificio = await _context.Edificios.FindAsync(id);
            if (edificio == null) return NotFound();

            _context.Edificios.Remove(edificio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Método privado para traer Admins sin edificio
        private async Task<List<Usuario>> ObtenerAdminsDisponibles(string? adminActualId = null)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            return admins
                .Where(a => a.EdificioId == null || a.Id == adminActualId)
                .ToList();
        }
    }
}
