using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollSystem.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AttendanceDate { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? TimeIn { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? TimeOut { get; set; }

        public int LateMinutes { get; set; }

        public int BreakMinutes { get; set; } = 60;

        public int MinutesWorked { get; set; }

        // Navigation property
        public Employee? Employee { get; set; }
    }
}