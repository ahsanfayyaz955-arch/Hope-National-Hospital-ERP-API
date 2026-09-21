using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Doctor
{
    public class DoctorResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string? Specialization { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public decimal ConsultationFee { get; set; }
        public bool IsActive { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

    }
}
