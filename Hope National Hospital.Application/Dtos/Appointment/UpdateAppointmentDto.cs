using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Appointment
{
    public class UpdateAppointmentDto
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Doctor is required.")]
        public int DoctorId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Patient is required.")]
        public int PatientId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Invalid receptionist ID.")]
        public int? ReceptionistId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Appointment title is required.")]
        [StringLength(150, MinimumLength = 2,
            ErrorMessage = "Appointment title must be between 2 and 150 characters.")]
        public string AppointmentTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Appointment status is required.")]
        [StringLength(30,
            ErrorMessage = "Status cannot exceed 30 characters.")]
        public string Status { get; set; } = "Scheduled";

        [StringLength(500,
            ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string? Reason { get; set; }

        [StringLength(1000,
            ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }
    }
}