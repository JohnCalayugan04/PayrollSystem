using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollSystem.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Position { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DailyRate { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        public ICollection<CashAdvance> CashAdvances { get; set; } = new List<CashAdvance>();

        public ICollection<Grocery> Groceries { get; set; } = new List<Grocery>();
    }
}