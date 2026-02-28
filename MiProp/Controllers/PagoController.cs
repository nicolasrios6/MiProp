using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProp.Data;
using MiProp.Models;

namespace MiProp.Controllers
{
    public class PagoController : Controller
    {

        private readonly UserManager<Usuario> _userManager;
        private readonly AppDbContext _context;

        public PagoController(UserManager<Usuario> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var admin = await _userManager.GetUserAsync(User);

            var pagos = await _context.Pagos
                .Include(p => p.Inquilino)
                .Where(p => p.Inquilino.EdificioId == admin.EdificioId)
                .OrderByDescending(p => p.FechaVencimiento)
                .ToListAsync();

            return View(pagos);
        }

        public async Task<IActionResult> Create()
        {
            var admin = await _userManager.GetUserAsync(User);

            var listaInquilinos = await _userManager.GetUsersInRoleAsync("Inquilino");
            var inquilinos = await _context.Users
                .Where(u => u.EdificioId == admin.EdificioId && listaInquilinos.Contains(u))
                .ToListAsync();

            ViewBag.Inquilinos = inquilinos;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago)
        {
            if (!ModelState.IsValid)
                return View(pago);

            pago.Estado = EstadoPago.Pendiente;

            _context.Add(pago);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MarcarPagado(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);

            if (pago == null)
                return NotFound();

            pago.Estado = EstadoPago.Pagado;
            pago.FechaPago = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Inquilino)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null)
                return NotFound();

            return View(pago);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);

            if (pago != null)
            {
                _context.Pagos.Remove(pago);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
