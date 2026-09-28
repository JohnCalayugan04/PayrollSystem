using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollSystem.Models
{
    public class Payroll
    {
        [Key]
        public int PayrollId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PayrollPeriodStart { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PayrollPeriodEnd { get; set; }

        public int DaysWorked { get; set; }

        public int MinutesWorked { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DailyRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal RatePerMinute { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasicSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OvertimePay { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CashAdvance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Grocery { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalSalary { get; set; }

        // Relationship
        public Employee? Employee { get; set; }
    }
}
