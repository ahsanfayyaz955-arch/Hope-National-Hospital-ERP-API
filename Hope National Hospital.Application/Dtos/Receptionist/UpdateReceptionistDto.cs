using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Receptionist
{
    public class UpdateReceptionistDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(150,
            ErrorMessage = "Full name cannot exceed 150 characters.")]
        public string FullName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20,
            ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;


        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(150,
            ErrorMessage = "Email cannot exceed 150 characters.")]
        public string? Email { get; set; }


        [Range(1, int.MaxValue,
            ErrorMessage = "Department ID must be greater than zero.")]
        public int DepartmentId { get; set; }


        public bool IsActive { get; set; }
    }
}