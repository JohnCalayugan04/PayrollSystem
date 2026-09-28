using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayrollSystem.Data;
using PayrollSystem.Models;

namespace PayrollSystem.Controllers
{
    public class CashAdvancesController : Controller
    {
        private readonly AppDbContext _context;

        public CashAdvancesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CashAdvances
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.CashAdvances.Include(c => c.Employee);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CashAdvances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashAdvance = await _context.CashAdvances
                .Include(c => c.Employee)
                .FirstOrDefaultAsync(m => m.CashAdvanceId == id);
            if (cashAdvance == null)
            {
                return NotFound();
            }

            return View(cashAdvance);
        }

        // GET: CashAdvances/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(
                _context.Employees
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.FullName),
                "EmployeeId",
                "FullName");

            return View();
        }

        // POST: CashAdvances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CashAdvanceId,EmployeeId,Date,Amount,Description")] CashAdvance cashAdvance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cashAdvance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeCode", cashAdvance.EmployeeId);
            return View(cashAdvance);
        }

        // GET: CashAdvances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashAdvance = await _context.CashAdvances.FindAsync(id);
            if (cashAdvance == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeCode", cashAdvance.EmployeeId);
            return View(cashAdvance);
        }

        // POST: CashAdvances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CashAdvanceId,EmployeeId,Date,Amount,Description")] CashAdvance cashAdvance)
        {
            if (id != cashAdvance.CashAdvanceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cashAdvance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CashAdvanceExists(cashAdvance.CashAdvanceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeCode", cashAdvance.EmployeeId);
            return View(cashAdvance);
        }

        // GET: CashAdvances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashAdvance = await _context.CashAdvances
                .Include(c => c.Employee)
                .FirstOrDefaultAsync(m => m.CashAdvanceId == id);
            if (cashAdvance == null)
            {
                return NotFound();
            }

            return View(cashAdvance);
        }

        // POST: CashAdvances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cashAdvance = await _context.CashAdvances.FindAsync(id);
            if (cashAdvance != null)
            {
                _context.CashAdvances.Remove(cashAdvance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CashAdvanceExists(int id)
        {
            return _context.CashAdvances.Any(e => e.CashAdvanceId == id);
        }
    }
}
