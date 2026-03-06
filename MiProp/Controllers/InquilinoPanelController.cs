using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProp.Data;
using MiProp.Models;

namespace MiProp.Controllers
{
    public class InquilinoPanelController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly AppDbContext _context;

        public InquilinoPanelController(UserManager<Usuario> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var inquilino = await _userManager.GetUserAsync(User);

            var departamento = await _context.Departamentos
            .Include(d => d.Edificio)
            .FirstOrDefaultAsync(d => d.InquilinoId == inquilino.Id);

            var pagos = await _context.Pagos
                .Where(p => p.InquilinoId == inquilino.Id)
                .OrderByDescending(p => p.FechaVencimiento)
                .ToListAsync();

            // 🔥 Detectar vencidos automáticamente
            foreach (var pago in pagos)
            {
                if (pago.Estado == EstadoPago.Pendiente &&
                    pago.FechaVencimiento.Date < DateTime.Today)
                {
                    pago.Estado = EstadoPago.Vencido;
                }
            }

            ViewBag.Departamento = departamento;
            ViewBag.Pagos = pagos;

            return View(inquilino);
        }
    }
}
