using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Admission
{
    public class UpdateAdmissionDto
    {
        [Range(1, int.MaxValue)]
        public int DoctorId { get; set; }

        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }

        [MaxLength(1000)]
        public string? Reason { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}