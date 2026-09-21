using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Domain.Entities
{
    public class Admission
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public int DepartmentId { get; set; }

        public int RoomId { get; set; }

        public int BedId { get; set; }

        [MaxLength(1000)]
        public string? Reason { get; set; }

        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;

        public DateTime? DischargeDate { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "Active";

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties

        public Patient? Patient { get; set; }

        public Doctor? Doctor { get; set; }

        public Department? Department { get; set; }

        public Room? Room { get; set; }

        public Bed? Bed { get; set; }
    }
}