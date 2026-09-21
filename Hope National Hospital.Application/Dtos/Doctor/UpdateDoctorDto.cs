using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Doctor
{
    public class UpdateDoctorDto
    {
        [Required(ErrorMessage = "Doctor name is required.")]
        [StringLength(150, MinimumLength = 2,
            ErrorMessage = "Doctor name must be between 2 and 150 characters.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20,
            ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string? Phone { get; set; }

        [StringLength(150,
            ErrorMessage = "Specialization cannot exceed 150 characters.")]
        public string? Specialization { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(150,
            ErrorMessage = "Email cannot exceed 150 characters.")]
        public string? Email { get; set; }

        [Range(0, 1000000,
            ErrorMessage = "Consultation fee must be between 0 and 1,000,000.")]
        public decimal ConsultationFee { get; set; }

        public bool IsActive { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }
    }
}