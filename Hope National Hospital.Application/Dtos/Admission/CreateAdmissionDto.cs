using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Admission
{
    public class CreateAdmissionDto
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Patient is required.")]
        public int PatientId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Doctor is required.")]
        public int DoctorId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Room is required.")]
        public int RoomId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Bed is required.")]
        public int BedId { get; set; }

        [MaxLength(1000)]
        public string? Reason { get; set; }

        public DateTime? AdmissionDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}