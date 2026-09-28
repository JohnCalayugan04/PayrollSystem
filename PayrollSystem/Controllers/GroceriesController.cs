using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayrollSystem.Data;
using PayrollSystem.Models;

namespace PayrollSystem.Controllers
{
    public class GroceriesController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        // GET: Groceries
        public async Task<IActionResult> Index()
        {
            var groceries = await _context.Groceries
                .Include(g => g.Employee)
                .ToListAsync();

            return View(groceries);
        }

        // GET: Groceries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grocery = await _context.Groceries
                .Include(g => g.Employee)
                .FirstOrDefaultAsync(g => g.GroceryId == id);

            if (grocery == null)
            {
                return NotFound();
            }

            return View(grocery);
        }

        // GET: Groceries/Create
        public IActionResult Create()
        {
            var employees = _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FullName)
                .ToList();

            ViewData["EmployeeId"] = new SelectList(
                employees,
                "EmployeeId",
                "FullName"
            );

            return View();
        }

        // POST: Groceries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("GroceryId,EmployeeId,Date,Amount,Description")]
            Grocery grocery)
        {
            if (ModelState.IsValid)
            {
                _context.Groceries.Add(grocery);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var employees = _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FullName)
                .ToList();

            ViewData["EmployeeId"] = new SelectList(
                employees,
                "EmployeeId",
                "FullName",
                grocery.EmployeeId
            );

            return View(grocery);
        }

        // GET: Groceries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grocery = await _context.Groceries.FindAsync(id);

            if (grocery == null)
            {
                return NotFound();
            }

            var employees = _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FullName)
                .ToList();

            ViewData["EmployeeId"] = new SelectList(
                employees,
                "EmployeeId",
                "FullName",
                grocery.EmployeeId
            );

            return View(grocery);
        }

        // POST: Groceries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("GroceryId,EmployeeId,Date,Amount,Description")]
            Grocery grocery)
        {
            if (id != grocery.GroceryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grocery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroceryExists(grocery.GroceryId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            var employees = _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FullName)
                .ToList();

            ViewData["EmployeeId"] = new SelectList(
                employees,
                "EmployeeId",
                "FullName",
                grocery.EmployeeId
            );

            return View(grocery);
        }

        // GET: Groceries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grocery = await _context.Groceries
                .Include(g => g.Employee)
                .FirstOrDefaultAsync(g => g.GroceryId == id);

            if (grocery == null)
            {
                return NotFound();
            }

            return View(grocery);
        }

        // POST: Groceries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grocery = await _context.Groceries.FindAsync(id);

            if (grocery != null)
            {
                _context.Groceries.Remove(grocery);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GroceryExists(int id)
        {
            return _context.Groceries.Any(g => g.GroceryId == id);
        }
    }
}