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
    public class PayrollsController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        // GET: Payrolls
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Payrolls
                .Include(p => p.Employee);

            return View(await appDbContext.ToListAsync());
        }

        // GET: Payrolls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // GET: Payrolls/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(
                _context.Employees
                    .Where(e => e.IsActive),
                "EmployeeId",
                "FullName");

            return View();
        }

        // POST: Payrolls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payroll payroll)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payroll);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeId"] = new SelectList(
                _context.Employees
                    .Where(e => e.IsActive),
                "EmployeeId",
                "FullName",
                payroll.EmployeeId);

            return View(payroll);
        }

        //GET: Payrolls/Calculate
        public IActionResult Calculate()
        {
            ViewBag.Employees = new SelectList(
                _context.Employees
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.FullName),
                "EmployeeId",
                "FullName");

            return View();
        }

        //POST: Calculate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(int employeeId,DateTime periodStart,DateTime periodEnd,decimal overtimePay)
        {
            if (employeeId <= 0)
            {
                ModelState.AddModelError(
                    "",
                    "Please select an employee.");
            }

            if (periodStart > periodEnd)
            {
                ModelState.AddModelError(
                    "",
                    "Start date cannot be after end date.");
            }

            if (overtimePay < 0)
            {
                ModelState.AddModelError(
                    "",
                    "Overtime pay cannot be negative.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Employees = new SelectList(
                    _context.Employees
                        .Where(e => e.IsActive)
                        .OrderBy(e => e.FullName),
                    "EmployeeId",
                    "FullName",
                    employeeId);

                return View();
            }

            // Get employee
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                return NotFound();
            }

            // Check if payroll already exists for this employee and period
            var existingPayroll = await _context.Payrolls
                .FirstOrDefaultAsync(p =>
                    p.EmployeeId == employeeId &&
                    p.PayrollPeriodStart == periodStart &&
                    p.PayrollPeriodEnd == periodEnd);

            if (existingPayroll != null)
            {
                TempData["ErrorMessage"] =
                    "Payroll already exists for this employee and period.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = existingPayroll.PayrollId });
            }

            // Get attendance records
            var attendanceRecords = await _context.Attendances
                .Where(a =>
                    a.EmployeeId == employeeId &&
                    a.AttendanceDate >= periodStart &&
                    a.AttendanceDate <= periodEnd)
                .ToListAsync();

            // Get cash advances within payroll period
            var cashAdvance = await _context.CashAdvances
                .Where(c =>
                    c.EmployeeId == employeeId &&
                    c.Date >= periodStart &&
                    c.Date <= periodEnd)
                .SumAsync(c => (decimal?)c.Amount) ?? 0m;

            // Get grocery deductions within payroll period
            var grocery = await _context.Groceries
                .Where(g =>
                    g.EmployeeId == employeeId &&
                    g.Date >= periodStart &&
                    g.Date <= periodEnd)
                .SumAsync(g => (decimal?)g.Amount) ?? 0m;

            // Days worked
            int daysWorked = attendanceRecords
                .Count(a =>
                    a.TimeIn.HasValue &&
                    a.TimeOut.HasValue);

            // Total minutes worked
            int minutesWorked = attendanceRecords
                .Sum(a => a.MinutesWorked);

            // Rate per minute
            decimal ratePerMinute =
                employee.DailyRate / 480m;

            // Basic salary
            decimal basicSalary =
                minutesWorked * ratePerMinute;

            // Final salary
            decimal finalSalary =
                basicSalary
                + overtimePay
                - cashAdvance
                - grocery;

            // Prevent negative salary
            if (finalSalary < 0)
            {
                finalSalary = 0;
            }

            var payroll = new Payroll
            {
                EmployeeId = employeeId,

                PayrollPeriodStart = periodStart,
                PayrollPeriodEnd = periodEnd,

                DaysWorked = daysWorked,
                MinutesWorked = minutesWorked,

                DailyRate = employee.DailyRate,
                RatePerMinute = ratePerMinute,

                BasicSalary = basicSalary,

                OvertimePay = overtimePay,
                CashAdvance = cashAdvance,
                Grocery = grocery,

                FinalSalary = finalSalary
            };

            _context.Payrolls.Add(payroll);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Details),
                new { id = payroll.PayrollId });
        }

        // GET: Payrolls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeCode", payroll.EmployeeId);
            return View(payroll);
        }

        // POST: Payrolls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayrollId,EmployeeId,PayrollPeriodStart,PayrollPeriodEnd,DaysWorked,MinutesWorked,DailyRate,RatePerMinute,BasicSalary,OvertimePay,CashAdvance,Grocery,FinalSalary")] Payroll payroll)
        {
            if (id != payroll.PayrollId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payroll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollExists(payroll.PayrollId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeCode", payroll.EmployeeId);
            return View(payroll);
        }

        // GET: Payrolls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // POST: Payrolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll != null)
            {
                _context.Payrolls.Remove(payroll);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayrollExists(int id)
        {
            return _context.Payrolls.Any(e => e.PayrollId == id);
        }
    }
}
