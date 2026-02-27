using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProp.Data;
using MiProp.Models;

namespace MiProp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartamentoController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly AppDbContext _context;

        public DepartamentoController(UserManager<Usuario> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var departamentos = await _context.Departamentos
                .Where(d => d.EdificioId == user.EdificioId)
                .ToListAsync();

            return View(departamentos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Departamento departamento)
        {
            var user = await _userManager.GetUserAsync(User);

            if(!ModelState.IsValid)
                return View(departamento);

            departamento.EdificioId = user.EdificioId.Value;

            _context.Add(departamento);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var departamento = await _context.Departamentos
                .FirstOrDefaultAsync(d => d.Id == id && d.EdificioId == user.EdificioId);

            if (departamento == null)
                return NotFound();

            return View(departamento);
        }

        // EDIT POST
        [HttpPost]
        public async Task<IActionResult> Edit(Departamento departamento)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
                return View(departamento);

            departamento.EdificioId = user.EdificioId.Value;

            _context.Update(departamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var departamento = await _context.Departamentos
                .FirstOrDefaultAsync(d => d.Id == id && d.EdificioId == user.EdificioId);

            if (departamento == null)
                return NotFound();

            _context.Departamentos.Remove(departamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
