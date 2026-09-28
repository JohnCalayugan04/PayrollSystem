using Microsoft.EntityFrameworkCore;
using PayrollSystem.Models;

namespace PayrollSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<Payroll> Payrolls { get; set; }

        public DbSet<CashAdvance> CashAdvances { get; set; }

        public DbSet<Grocery> Groceries { get; set; }
    }
}